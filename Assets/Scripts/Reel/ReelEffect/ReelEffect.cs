using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels
{
    // ���[���G�t�F�N�g(�o�b�N���C�g�̓_�ŁA�V���{���_���Ȃ�)
    public class ReelEffect : MonoBehaviour
    {
        // JAC���̌��x�������l
        const float JacLightOffset = 0.4f;

        // ���[���{��
        [SerializeField] ReelBase reelBase;
        [SerializeField] SymbolLightManager symbolLight;

        // JAC���̖��邳�v�Z�����邩
        public bool HasJacBrightnessCalculate {  get; private set; }

        // func
        // ���[���{�̖̂��邳�ύX
        public void ChangeReelBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);

        // �w�肵���ʒu�̐}���̖��邳�ύX
        public void ChangeSymbolBrightness(int posID, byte brightness) => symbolLight.ChangeSymbolBrightness(posID, brightness);

        // JAC���̖��邳�v�Z�̐ݒ�
        public void SetJacBrightnessCalculate(bool value) => HasJacBrightnessCalculate = value;

        // JAC���̖��邳�v�Z������
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

        // JAC���̖��邳�v�Z�̃��Z�b�g
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

        // JAC���̖��邳�v�Z���I������
        public void FinishJacBrightnessCalculate()
        {
            ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
            ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
            ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
        }

        // JAC���̖��邳�v�Z(
        private byte CalculateJACBrightness(float maxSpeed, bool isNegative)
        {
            float brightnessTest = 0;
            float currentDistance = 0;

            // �������߂Ɍ��点�邽�ߋ����͒Z������
            float distanceRotation = ChangeAngle * JacLightOffset;

            // �����ɍ��킹�ċ������v�Z
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

