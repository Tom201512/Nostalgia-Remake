using System;
using TreeEditor;
using UnityEngine;
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

        private void Update()
        {
            if (reelSpinModel.MaxSpeed != 0)
            {
                reelSpinModel.ChangeReelSpeed();
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

        // リールの回転
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((reelSpinModel.ReturnDegreePerSecond()) * Time.deltaTime * reelSpinModel.RotateSpeed, -360, 360);
            Debug.Log("RotationAngle:" + rotationAngle);

            Debug.Log(rotationAngle * Vector3.left);
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

            Debug.Log("Euler:" + transform.rotation.eulerAngles.x);
            //Debug.Log("ChangeAngle:" + (360f - ChangeAngle));

            // 一定角度に達したら図柄の更新(17.14286度)

            // 回転速度に合わせて変更
            // 逆回転の場合
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                Debug.Log("Symbol changed");
                OnReelPositionChanged?.Invoke();

                // 図柄位置変更

                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ReelSpinModel.ChangeAngle * -1);

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

                /*
                // 停止する位置になったら
                if (reelSpinModel.CurrentReelStatus == ReelSpinModel.ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }*/
            }
            // 前回転の場合
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                Debug.Log("Symbol changed");
                // 図柄位置変更
                OnReelPositionChanged?.Invoke();

                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ReelSpinModel.ChangeAngle * -1);

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
                }

                // 停止する位置になったら
                if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }*/
            }
        }
    }
}
