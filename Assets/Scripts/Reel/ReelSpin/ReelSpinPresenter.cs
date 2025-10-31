using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels.Spin
{
    public class ReelSpinPresenter : MonoBehaviour
    {
        // リール回転用のプレゼンター

        // var
        // 図柄位置が変わったことを伝えるイベント
        public delegate void ReelPositionChanged();
        public event ReelPositionChanged OnReelPositionChanged;

        // モデル
        private ReelSpinModel reelSpinModel;

        // モーションブラー
        private PostProcessVolume postVolume;
        // ブラー部分のプロファイル
        private MotionBlur motionBlur;

        // リールが停止したことを伝えるイベント
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        private void Awake()
        {
            // ブラーの取得
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);
        }

        private void Update()
        {
            if (reelSpinModel.MaxSpeed != 0)
            {
                reelSpinModel.AccelerateReelSpeed();
                RotateReel();
            }
        }

        // 現在のリール情報を返す
        public ReelStatus GetCurrentReelStatus() => reelSpinModel.CurrentReelStatus;
        // 現在の速度を返す
        public float GetCurrentSpeed() => reelSpinModel.RotateSpeed;
        // 現在の角度を返す
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;

        // 最高速度状態か返す
        public bool IsMaximumSpeed() => reelSpinModel.RotateSpeed == reelSpinModel.MaxSpeed;

        // 最後に止めたときのスベリコマ数を得る
        public int GetLastStoppedDelay() => reelSpinModel.LastStoppedDelay;

        // ブラー設定
        public void ChangeBlurSetting(bool value) => motionBlur.enabled.value = value;

        // データのセット
        public void SetReelSpinPresenter(float RotateRPM)
        {
            reelSpinModel = new ReelSpinModel(RotateRPM);
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
        public void StartStopReelSpin(int delay)
        {
            Debug.Log("Received ReelStop");
            reelSpinModel.CurrentReelStatus = ReelStatus.RecieveStop;
            reelSpinModel.RemainingDelay = delay;
        }

        // リールの回転を終了する
        public void FinishReelSpin()
        {
            Debug.Log("Finished ReelSpin");
            transform.rotation = Quaternion.identity;
            reelSpinModel.FinishReelSpin();
            HasReelStopped?.Invoke();
        }

        // リールの回転
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((reelSpinModel.ReturnDegreePerSecond()) * Time.deltaTime * reelSpinModel.RotateSpeed, -360, 360);
            transform.Rotate(rotationAngle * Vector3.left);

            /*
            // JAC中であればライトの調整をする
            if (HasJacModeLight)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(false));
                    ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, CalculateJACBrightness(true));
                }
                else
                {
                    ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, CalculateJACBrightness(false));
                    ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(true));
                }
            }*/
            //Debug.Log("ChangeAngle:" + (360f - ChangeAngle));

            ChangeReelPos();
        }

        // 図柄位置を変更する
        private void ChangeReelPos()
        {
            // 一定角度に達したら図柄の更新(17.14286度)
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                //Debug.Log("Symbol changed");
                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ChangeAngle * -1);

                // 位置変更を伝える
                OnReelPositionChanged?.Invoke();
                /*
                if (HasJacModeLight)
                {
                    if (Math.Sign(maxSpeed) == -1)
                    {
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
                    }
                }*/

                // 残りスベリコマ数が0になったら停止処理
                if (reelSpinModel.RemainingDelay > 0 && reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    reelSpinModel.RemainingDelay -= 1;
                    Debug.Log("Remaining Delay:" + reelSpinModel.RemainingDelay);
                }
                else if(reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    FinishReelSpin();
                }
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                //Debug.Log("Symbol changed");
                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ChangeAngle);
                // 位置変更を伝える
                OnReelPositionChanged?.Invoke();
                /*
                if (HasJacModeLight)
                {
                    if (Math.Sign(maxSpeed) == -1)
                    {
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                        ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
                    }
                }*/

                // 残りスベリコマ数が0になったら停止処理
                if (reelSpinModel.RemainingDelay > 0 && reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    reelSpinModel.RemainingDelay -= 1;
                    Debug.Log("Remaining Delay:" + reelSpinModel.RemainingDelay);
                }
                else if (reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    FinishReelSpin();
                }
            }
        }

        // 回転を遅くする
        private void SlowDownReelSpeed()
        {
            // 一定角度に達したらリール速度を落とす(15.14286度)
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > StopAngle)
            {
                // 停止状態にする
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.0f;
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - StopAngle)
            {
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.0f;
            }
        }
    }
}
