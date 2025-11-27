using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels
{
    // リールエフェクト(バックライトの点滅、シンボル点灯など)
    public class ReelEffect : MonoBehaviour
    {
        // JAC時の光度調整数値
        const float JacLightOffset = 0.4f;

        // リール本体
        [SerializeField] ReelBase reelBase;
        [SerializeField] SymbolLightManager symbolLight;

        // JAC中の明るさ計算をするか
        public bool HasJacBrightnessCalculate {  get; private set; }

        // func
        // リール本体の明るさ変更
        public void ChangeReelBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);

        // 指定した位置の図柄の明るさ変更
        public void ChangeSymbolBrightness(int posID, byte brightness) => symbolLight.ChangeSymbolBrightness(posID, brightness);

        // JAC時の明るさ計算の設定
        public void SetJacBrightnessCalculate(bool value) => HasJacBrightnessCalculate = value;

        // JAC中の明るさ計算をする
        public void CalculateJacBrightness(float maxSpeed)
        {
            if (Math.Sign(maxSpeed) == -1)
            {
                ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(maxSpeed, false));
                ChangeSymbolBrightness((int)ReelPosID.Lower, CalculateJACBrightness(maxSpeed, true));
            }
            else
            {
                ChangeSymbolBrightness((int)ReelPosID.Upper, CalculateJACBrightness(maxSpeed, false));
                ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(maxSpeed, true));
            }
        }

        // JAC中の明るさ計算のリセット
        public void ResetJacBrightnessCalculate(float maxSpeed)
        {
            if (Math.Sign(maxSpeed) == -1)
            {
                ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
            }
            else
            {
                ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
            }
        }

        // JAC中の明るさ計算を終了する
        public void FinishJacBrightnessCalculate()
        {
            ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
            ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
            ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
        }

        // JAC時の明るさ計算(
        private byte CalculateJACBrightness(float maxSpeed, bool isNegative)
        {
            float brightnessTest = 0;
            float currentDistance = 0;

            // 少し早めに光らせるため距離は短くする
            float distanceRotation = ChangeAngle * JacLightOffset;

            // 符号に合わせて距離を計算
            //Debug.Log("Current Euler:" + transform.rotation.eulerAngles.x);
            if (transform.rotation.eulerAngles.x > 0f)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    currentDistance = transform.rotation.eulerAngles.x;
                }
                else
                {
                    currentDistance = 360.0f - transform.rotation.eulerAngles.x;
                }
            }
            brightnessTest = Math.Clamp(currentDistance / distanceRotation, 0, 1);

            int distance = SymbolLight.TurnOnValue - SymbolLight.TurnOffValue;

            float CenterBright = 0;

            if (Math.Sign(maxSpeed) == -1)
            {
                CenterBright = Math.Clamp(SymbolLight.TurnOnValue - (distance * brightnessTest), 0, 255);
            }
            else
            {
                CenterBright = Math.Clamp(SymbolLight.TurnOffValue + (distance * brightnessTest), 0, 255);
            }
            return (byte)Math.Clamp(CenterBright, 0, 255);
        }
    }
}

