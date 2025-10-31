using System;
using TreeEditor;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels.Spin
{
    public class ReelSpinPresenter : MonoBehaviour
    {
        // ���[����]�p�̃v���[���^�[

        // var
        // �}���ʒu���ς�������Ƃ�`����C�x���g
        public delegate void ReelPositionChanged();
        public event ReelPositionChanged OnReelPositionChanged;

        // ���f��
        private ReelSpinModel reelSpinModel;

        private void Update()
        {
            if (reelSpinModel.MaxSpeed != 0)
            {
                reelSpinModel.ChangeReelSpeed();
                RotateReel();
            }
        }

        // ���݂̃��[������Ԃ�
        public ReelStatus GetCurrentReelStatus() => reelSpinModel.CurrentReelStatus;
        // ���݂̑��x��Ԃ�
        public float GetCurrentSpeed() => reelSpinModel.RotateSpeed;
        // ���݂̊p�x��Ԃ�
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;

        // �ō����x��Ԃ��Ԃ�
        public bool IsMaximumSpeed() => reelSpinModel.RotateSpeed == reelSpinModel.MaxSpeed;

        // �f�[�^�̃Z�b�g
        public void SetReelSpinPresenter(float RotateRPM)
        {
            reelSpinModel = new ReelSpinModel(RotateRPM);
        }

        // ���[���̉�]���J�n
        public void StartReelSpin(float maxSpeed, bool cutAccelerate)
        {
            reelSpinModel.MaxSpeed = maxSpeed;

            // �������J�b�g����ꍇ�͂����ɑ��x���グ��
            if (cutAccelerate)
            {
                reelSpinModel.RotateSpeed = maxSpeed;
            }

            reelSpinModel.CurrentReelStatus = ReelStatus.Spinning;
        }

        // ���[���̉�]
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((reelSpinModel.ReturnDegreePerSecond()) * Time.deltaTime * reelSpinModel.RotateSpeed, -360, 360);
            Debug.Log("RotationAngle:" + rotationAngle);

            Debug.Log(rotationAngle * Vector3.left);
            transform.Rotate(rotationAngle * Vector3.left);

            /*
            // JAC���ł���΃��C�g�̒���������
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

            // ���p�x�ɒB������}���̍X�V(17.14286�x)

            // ��]���x�ɍ��킹�ĕύX
            // �t��]�̏ꍇ
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                Debug.Log("Symbol changed");
                OnReelPositionChanged?.Invoke();

                // �}���ʒu�ύX

                // �p�x�����Ƃɖ߂�
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
                // ��~����ʒu�ɂȂ�����
                if (reelSpinModel.CurrentReelStatus == ReelSpinModel.ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }*/
            }
            // �O��]�̏ꍇ
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                Debug.Log("Symbol changed");
                // �}���ʒu�ύX
                OnReelPositionChanged?.Invoke();

                // �p�x�����Ƃɖ߂�
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

                // ��~����ʒu�ɂȂ�����
                if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }*/
            }
        }
    }
}
