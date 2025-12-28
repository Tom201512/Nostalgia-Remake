using ReelSpinGame_Datas;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels.Spin
{
    // リール回転用のプレゼンター
    public class ReelSpinPresenter : MonoBehaviour
    {
        // var
        // 回転時のRPM
        [SerializeField][Range(0, 80.0f)] private float defaultReelSpinRPM;
        // リール配列データ
        [SerializeField] ReelArrayData reelArrayDataFile;

        // 図柄位置が変わったことを伝えるイベント
        public delegate void ReelPositionChanged();
        public event ReelPositionChanged OnReelPositionChanged;

        // リール角度が変わったことを伝えるイベント
        public delegate void ReelDegreeChanged();
        public event ReelDegreeChanged OnReelDegreeChanged;

        // リールが停止したことを伝えるイベント
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        ReelSpinModel reelSpinModel;        // データ
        SymbolManager symbolManager;        // 図柄マネージャー
        PostProcessVolume postVolume;       // モーションブラー
        MotionBlur motionBlur;              // ブラー部分のプロファイル

        private void Awake()
        {
            // ブラーの取得
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);
            symbolManager = GetComponentInChildren<SymbolManager>();
            reelSpinModel = new ReelSpinModel(defaultReelSpinRPM);
            reelSpinModel.ReelArray = reelArrayDataFile.Array;
        }

        private void Update()
        {
            if (reelSpinModel.MaxSpeed != 0)
            {
                reelSpinModel.AccelerateReelSpeed();
                RotateReel();

                if (reelSpinModel.CurrentReelStatus == ReelStatus.Stopping)
                {
                    SlowDownReelSpeed();
                }
            }
        }

        // リール配列情報を渡す
        public byte[] GetReelArray() => reelSpinModel.ReelArray;

        // 現在のリール情報を返す
        public ReelStatus GetCurrentReelStatus() => reelSpinModel.CurrentReelStatus;
        // 現在の速度を返す
        public float GetCurrentSpeed() => reelSpinModel.RotateSpeed;
        // 現在の最高速度を返す
        public float GetMaxSpeed() => reelSpinModel.MaxSpeed;
        // 現在の角度を返す
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;

        // 最高速度状態か返す
        public bool IsMaximumSpeed() => reelSpinModel.RotateSpeed == reelSpinModel.MaxSpeed;

        // 現在の下段位置を得る
        public int GetCurrentLower() => reelSpinModel.CurrentLower;
        // 最後に停止させた位置を得る
        public int GetLastPushedPos() => reelSpinModel.LastPushedPos;
        // 停止予定位置を得る
        public int GetWillStopLowerPos() => reelSpinModel.WillStopLowerPos;
        // 停止したときの押し順を得る
        public int GetLastStoppedOrder() => reelSpinModel.LastStoppedOrder;
        // 最後に止めたときのスベリコマ数を得る
        public int GetLastStoppedDelay() => reelSpinModel.LastStoppedDelay;

        // 現在の下段位置を設定する
        public void SetCurrentLower(int lowerPos) => reelSpinModel.CurrentLower = lowerPos;
        // 最後に押した位置を設定する
        public void SetLastPushedPos(int pushedPos) => reelSpinModel.LastPushedPos = pushedPos;
        // 停止予定位置を設定する
        public void SetWillStopLowerPos(int delay)
        {
            reelSpinModel.WillStopLowerPos = ReelObjectPresenter.OffsetReelPos(reelSpinModel.CurrentLower, delay);
            reelSpinModel.LastStoppedDelay = delay;
        }

        // ブラー設定
        public void ChangeBlurSetting(bool value) => motionBlur.enabled.value = value;

        // 指定位置からのリール位置を得る
        public int GetReelPos(int currentLower, sbyte posID) => ReelObjectPresenter.OffsetReelPos(currentLower, posID);
        // 指定位置からのリール図柄を得る
        public ReelSymbols GetReelSymbol(int currentLower, sbyte posID) => symbolManager.ReturnSymbol(reelSpinModel.ReelArray[ReelObjectPresenter.OffsetReelPos(currentLower, posID)]);
        // 指定位置からのリール図柄画像を得る
        public Sprite GetReelSymbolSprite(int reelPos) => symbolManager.GetSymbolImage(GetReelSymbol(reelPos, (int)ReelPosID.Lower));

        // リールの回転を開始
        public void StartReelSpin(float maxSpeed, bool cutAccelerate)
        {
            reelSpinModel.MaxSpeed = maxSpeed;

            // 加速をカットする場合はすぐに速度を上げる
            if (cutAccelerate)
            {
                reelSpinModel.RotateSpeed = maxSpeed;
            }

            reelSpinModel.CurrentReelStatus = ReelStatus.Spinning;
        }

        // リールの停止処理を開始する
        public void StartStopReelSpin(int pushedPos, int pushOrder, int delay)
        {
            reelSpinModel.LastPushedPos = pushedPos;
            reelSpinModel.WillStopLowerPos = ReelObjectPresenter.OffsetReelPos(pushedPos, delay);
            reelSpinModel.LastStoppedOrder = pushOrder;
            reelSpinModel.LastStoppedDelay = delay;
            reelSpinModel.CurrentReelStatus = ReelStatus.RecieveStop;
        }

        // リールを強制停止させる
        public void StopReelImmediately(int pushedPos, int pushOrder, int delay)
        {
            reelSpinModel.LastPushedPos = pushedPos;
            reelSpinModel.WillStopLowerPos = ReelObjectPresenter.OffsetReelPos(pushedPos, delay);
            reelSpinModel.LastStoppedOrder = pushOrder;
            reelSpinModel.LastStoppedDelay = delay;
            reelSpinModel.CurrentLower = reelSpinModel.WillStopLowerPos;
            FinishReelSpin();
        }


        // 図柄位置の更新
        public void UpdateReelSymbols(int currentLower) => symbolManager.UpdateSymbolsObjects(currentLower, reelSpinModel.ReelArray);

        // リールの回転
        void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((reelSpinModel.ReturnDegreePerSecond()) * Time.deltaTime * reelSpinModel.RotateSpeed, -360, 360);
            transform.Rotate(rotationAngle * Vector3.left);

            OnReelDegreeChanged?.Invoke();
            ChangeReelPos();
        }

        // 図柄位置を変更する
        void ChangeReelPos()
        {
            // 一定角度に達したら図柄の更新(17.14286度)
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                //Debug.Log("Symbol changed");
                reelSpinModel.CurrentLower = ReelObjectPresenter.OffsetReelPos(reelSpinModel.CurrentLower, -1);
                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ChangeAngle * -1);

                // 位置変更を伝える
                OnReelPositionChanged?.Invoke();
                UpdateReelSymbols(reelSpinModel.CurrentLower);

                // 停止位置になったら停止処理
                if (reelSpinModel.CurrentLower == reelSpinModel.WillStopLowerPos && reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    FinishReelSpin();
                }
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                reelSpinModel.CurrentLower = ReelObjectPresenter.OffsetReelPos(reelSpinModel.CurrentLower, 1);
                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ChangeAngle);
                // 位置変更を伝える
                OnReelPositionChanged?.Invoke();
                UpdateReelSymbols(reelSpinModel.CurrentLower);

                // 停止位置になったら停止処理
                if (reelSpinModel.CurrentLower == reelSpinModel.WillStopLowerPos && reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    FinishReelSpin();
                }
            }
        }

        // 回転を遅くする
        void SlowDownReelSpeed()
        {
            // 一定角度に達したらリール速度を落とす(15.14286度)
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > StopAngle)
            {
                // 停止状態にする
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.1f;
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - StopAngle)
            {
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.1f;
            }
        }

        // リールの回転を終了する
        void FinishReelSpin()
        {
            transform.rotation = Quaternion.identity;
            reelSpinModel.FinishReelSpin();
            HasReelStopped?.Invoke();
        }
    }
}
