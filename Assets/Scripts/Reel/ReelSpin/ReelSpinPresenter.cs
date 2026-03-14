using ReelSpinGame_Reel.Symbol;
using ReelSpinGame_Reel.Util;
using ReelSpinGame_Scriptable;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ReelSpinGame_Reel.Spin
{
    // リール回転用のプレゼンター
    public class ReelSpinPresenter : MonoBehaviour
    {
        [SerializeField][Range(0, 80.0f)] float defaultReelSpinRPM;        // 回転時のRPM
        [SerializeField] SymbolManager symbolManager;                      // 図柄部分
        [SerializeField] ReelArrayData reelArrayDataFile;                  // リール配列データ

        // プロパティ
        public int[] ReelArray { get => reelSpinModel.ReelArray; }                  // リール配列            
        public ReelStatus ReelStatus { get => reelSpinModel.CurrentReelStatus; }    // 現在のリール情報
        public float RotateSpeed { get => reelSpinModel.RotateSpeed; }              // 回転速度
        public float MaxSpeed { get => reelSpinModel.MaxSpeed; }                    // 現在の最高速度
        public float CurrentDegree { get => transform.rotation.eulerAngles.x; }     // 現在の角度               
        public int WillStopLowerPos { get => reelSpinModel.WillStopLowerPos; }      // 停止予定位置
        public int LastStoppedOrder { get => reelSpinModel.LastStoppedOrder; }      // 停止したときの押し順
        public int LastStoppedDelay { get => reelSpinModel.LastStoppedDelay; }      // 最後に止めたときのスベリコマ数

        public int CurrentLower { get => reelSpinModel.CurrentLower; }          // 現在の下段位置
        public int CurrentMarkerPos { get => symbolManager.CurrentMarkerPos; }  // マーカー位置

        // 停止位置
        public int LastPushedPos
        {
            get => reelSpinModel.LastPushedPos;
            set => reelSpinModel.LastPushedPos = value;
        }

        // ブラーの設定
        public bool HasBlur
        {
            get => motionBlur.enabled.value;
            set => motionBlur.enabled.value = value;
        }

        private ReelSpinModel reelSpinModel;        // データ
        private PostProcessVolume postVolume;       // モーションブラー
        private MotionBlur motionBlur;              // ブラー部分のプロファイル

        // 図柄位置が変わったことを伝えるイベント
        public event Action<int> ReelPositionChanged;

        // リール角度が変わったことを伝えるイベント
        public event Action ReelDegreeChanged;

        // リールが停止したことを伝えるイベント
        public event Action HasReelStopped;

        private void Awake()
        {
            reelSpinModel = new ReelSpinModel(defaultReelSpinRPM);
            reelSpinModel.ReelArray = reelArrayDataFile.Array;
            // ブラーの取得
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);
            motionBlur.enabled.value = false;
        }

        private void Update()
        {
            if (reelSpinModel.MaxSpeed != 0)
            {
                if(reelSpinModel.RotateSpeed != reelSpinModel.MaxSpeed)
                {
                    reelSpinModel.AccelerateReelSpeed();
                }

                RotateReel();

                if (reelSpinModel.CurrentReelStatus == ReelStatus.Stopping)
                {
                    CheckSlowDown();
                }
            }
        }

        // 最高速度状態か返す
        public bool IsMaximumSpeed() => reelSpinModel.RotateSpeed == reelSpinModel.MaxSpeed;

        // 指定位置からのリール位置を得る
        public int GetReelPos(int lowerPos, ReelPosID posID) => ReelSymbolPosCalc.OffsetReelPos(lowerPos, (int)posID);
        // 指定位置からのリール図柄を得る
        public ReelSymbols GetReelSymbol(int lowerPos, ReelPosID posID) => symbolManager.ReturnSymbol(reelSpinModel.ReelArray[ReelSymbolPosCalc.OffsetReelPos(lowerPos, (int)posID)]);
        // 指定位置からのリール図柄画像を得る
        public Sprite GetReelSymbolSprite(int reelPos) => symbolManager.GetSymbolImage(GetReelSymbol(reelPos, (int)ReelPosID.Lower));

        // リールの下段を指定位置に移動
        public void ChangeLowerPos(int lowerPos)
        {
            reelSpinModel.CurrentLower = lowerPos;
            UpdateReelSymbols();
            ReelPositionChanged?.Invoke(reelSpinModel.CurrentLower);
        }

        // マーカーの位置を変更
        public void ChangeMarkerPos(int markerPos)
        {
            symbolManager.CurrentMarkerPos = markerPos;
            UpdateReelSymbols();
            ReelPositionChanged?.Invoke(reelSpinModel.CurrentLower);
        }

        // スベリコマ数から停止位置予定を作成
        public void SetStopPosFromDelay(int delay)
        {
            reelSpinModel.WillStopLowerPos = ReelSymbolPosCalc.OffsetReelPos(reelSpinModel.CurrentLower, delay);
            reelSpinModel.LastStoppedDelay = delay;
        }

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
            RecordStopData(pushedPos, pushOrder, delay);
            reelSpinModel.CurrentReelStatus = ReelStatus.ReceiveStop;
        }

        // リールを強制停止させる
        public void StopReelImmediately(int pushedPos, int pushOrder, int delay)
        {
            RecordStopData(pushedPos, pushOrder, delay);
            reelSpinModel.CurrentLower = reelSpinModel.WillStopLowerPos;
            UpdateReelSymbols();
            FinishSpin();
        }

        // 図柄位置の更新
        private void UpdateReelSymbols() => symbolManager.UpdateSymbolsObjects(reelSpinModel.CurrentLower, reelSpinModel.ReelArray);

        // リールの回転
        private void RotateReel()
        {
            transform.Rotate(reelSpinModel.GetDegreePerFrame() * Vector3.left);
            ReelDegreeChanged?.Invoke();
            ChangeReelPos();
        }

        // 停止時の情報記録
        private void RecordStopData(int pushedPos, int pushOrder, int delay)
        {
            reelSpinModel.LastPushedPos = pushedPos;
            reelSpinModel.WillStopLowerPos = ReelSymbolPosCalc.OffsetReelPos(pushedPos, delay);
            reelSpinModel.LastStoppedOrder = pushOrder;
            reelSpinModel.LastStoppedDelay = delay;
        }

        // 図柄位置を変更する
        private void ChangeReelPos()
        {
            // 一定角度に達したら図柄の更新(17.14286度)
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ReelSpinModel.ChangeAngle)
            {
                reelSpinModel.CurrentLower = ReelSymbolPosCalc.OffsetReelPos(reelSpinModel.CurrentLower, -1);
                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ReelSpinModel.ChangeAngle * -1);

                // 位置変更を伝える
                ReelPositionChanged?.Invoke(reelSpinModel.CurrentLower);

                UpdateReelSymbols();
                if(CheckReachedStoppedPos())
                {
                    FinishSpin();
                }
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ReelSpinModel.ChangeAngle)
            {
                reelSpinModel.CurrentLower = ReelSymbolPosCalc.OffsetReelPos(reelSpinModel.CurrentLower, 1);
                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ReelSpinModel.ChangeAngle);
                // 位置変更を伝える
                ReelPositionChanged?.Invoke(reelSpinModel.CurrentLower);

                UpdateReelSymbols();
                if (CheckReachedStoppedPos())
                {
                    FinishSpin();
                }
            }
        }

        // 回転を遅くするかチェックする
        private void CheckSlowDown()
        {
            // 一定角度に達したらリール速度を落とす(15.14286度)
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ReelSpinModel.StopAngle)
            {
                reelSpinModel.StartSlowDown();
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ReelSpinModel.StopAngle)
            {
                reelSpinModel.StartSlowDown();
            }
        }

        // 停止位置になったか確認する
        private bool CheckReachedStoppedPos() => reelSpinModel.CurrentLower == reelSpinModel.WillStopLowerPos && reelSpinModel.CurrentReelStatus == ReelStatus.ReceiveStop;

        // 回転の終了
        private void FinishSpin()
        {
            transform.rotation = Quaternion.identity;
            reelSpinModel.FinishReelSpin();
            HasReelStopped?.Invoke();
        }
    }
}