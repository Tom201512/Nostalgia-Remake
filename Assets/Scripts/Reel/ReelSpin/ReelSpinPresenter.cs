using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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

        // ���[�V�����u���[
        private PostProcessVolume postVolume;
        // �u���[�����̃v���t�@�C��
        private MotionBlur motionBlur;

        // ���[������~�������Ƃ�`����C�x���g
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        private void Awake()
        {
            // �u���[�̎擾
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

        // ���݂̃��[������Ԃ�
        public ReelStatus GetCurrentReelStatus() => reelSpinModel.CurrentReelStatus;
        // ���݂̑��x��Ԃ�
        public float GetCurrentSpeed() => reelSpinModel.RotateSpeed;
        // ���݂̊p�x��Ԃ�
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;

        // �ō����x��Ԃ��Ԃ�
        public bool IsMaximumSpeed() => reelSpinModel.RotateSpeed == reelSpinModel.MaxSpeed;

        // �Ō�Ɏ~�߂��Ƃ��̃X�x���R�}���𓾂�
        public int GetLastStoppedDelay() => reelSpinModel.LastStoppedDelay;

        // �u���[�ݒ�
        public void ChangeBlurSetting(bool value) => motionBlur.enabled.value = value;

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

        // ���[���̒�~�������J�n����
        public void StartStopReelSpin(int delay)
        {
            Debug.Log("Received ReelStop");
            reelSpinModel.CurrentReelStatus = ReelStatus.RecieveStop;
            reelSpinModel.RemainingDelay = delay;
        }

        // ���[���̉�]���I������
        public void FinishReelSpin()
        {
            Debug.Log("Finished ReelSpin");
            transform.rotation = Quaternion.identity;
            reelSpinModel.FinishReelSpin();
            HasReelStopped?.Invoke();
        }

        // ���[���̉�]
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((reelSpinModel.ReturnDegreePerSecond()) * Time.deltaTime * reelSpinModel.RotateSpeed, -360, 360);
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
            //Debug.Log("ChangeAngle:" + (360f - ChangeAngle));

            ChangeReelPos();
        }

        // �}���ʒu��ύX����
        private void ChangeReelPos()
        {
            // ���p�x�ɒB������}���̍X�V(17.14286�x)
            // �t��]�̏ꍇ
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                //Debug.Log("Symbol changed");
                // �p�x�����Ƃɖ߂�
                transform.Rotate(Vector3.right, ChangeAngle * -1);

                // �ʒu�ύX��`����
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

                // �c��X�x���R�}����0�ɂȂ������~����
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
            // �O��]�̏ꍇ
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                //Debug.Log("Symbol changed");
                // �p�x�����Ƃɖ߂�
                transform.Rotate(Vector3.right, ChangeAngle);
                // �ʒu�ύX��`����
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

                // �c��X�x���R�}����0�ɂȂ������~����
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

        // ��]��x������
        private void SlowDownReelSpeed()
        {
            // ���p�x�ɒB�����烊�[�����x�𗎂Ƃ�(15.14286�x)
            // �t��]�̏ꍇ
            if (Math.Sign(reelSpinModel.RotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > StopAngle)
            {
                // ��~��Ԃɂ���
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.0f;
            }
            // �O��]�̏ꍇ
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - StopAngle)
            {
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.0f;
            }
        }
    }
}
