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

        // ���[���p�x���ς�������Ƃ�`����C�x���g
        public delegate void ReelDegreeChanged();
        public event ReelDegreeChanged OnReelDegreeChanged;

        // ���[������~�������Ƃ�`����C�x���g
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // ���f��
        private ReelSpinModel reelSpinModel;

        // ���[�V�����u���[
        private PostProcessVolume postVolume;
        // �u���[�����̃v���t�@�C��
        private MotionBlur motionBlur;

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

                if (reelSpinModel.CurrentReelStatus == ReelStatus.Stopping)
                {
                   SlowDownReelSpeed();
                }
            }
        }

        // ���݂̃��[������Ԃ�
        public ReelStatus GetCurrentReelStatus() => reelSpinModel.CurrentReelStatus;
        // ���݂̑��x��Ԃ�
        public float GetCurrentSpeed() => reelSpinModel.RotateSpeed;
        // ���݂̍ō����x��Ԃ�
        public float GetMaxSpeed() => reelSpinModel.MaxSpeed;
        // ���݂̊p�x��Ԃ�
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;

        // �ō����x��Ԃ��Ԃ�
        public bool IsMaximumSpeed() => reelSpinModel.RotateSpeed == reelSpinModel.MaxSpeed;

        // ���݂̉��i�ʒu�𓾂�
        public int GetCurrentLower() => reelSpinModel.CurrentLower;
        // �Ō�ɒ�~�������ʒu�𓾂�
        public int GetLastPushedPos() => reelSpinModel.LastPushedPos;
        // ��~�\��ʒu�𓾂�
        public int GetWillStopLowerPos() => reelSpinModel.WillStopLowerPos;
        // �Ō�Ɏ~�߂��Ƃ��̃X�x���R�}���𓾂�
        public int GetLastStoppedDelay() => reelSpinModel.LastStoppedDelay;

        // ���݂̉��i�ʒu��ݒ肷��
        public void SetCurrentLower(int lowerPos) => reelSpinModel.CurrentLower = lowerPos;
        // �Ō�ɉ������ʒu��ݒ肷��
        public void SetLastPushedPos(int pushedPos) => reelSpinModel.LastPushedPos = pushedPos;
        // ��~�\��ʒu��ݒ肷��
        public void SetWillStopLowerPos(int delay)
        {
            reelSpinModel.WillStopLowerPos = ReelObjectPresenter.OffsetReelPos(reelSpinModel.CurrentLower, delay);
            reelSpinModel.LastStoppedDelay = delay;
        }

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
        public void StartStopReelSpin(int pushedPos, int delay)
        {
            Debug.Log("Received ReelStop");
            Debug.Log("Delay:" + delay);
            reelSpinModel.LastPushedPos = pushedPos;
            reelSpinModel.WillStopLowerPos = ReelObjectPresenter.OffsetReelPos(pushedPos, delay);
            reelSpinModel.LastStoppedDelay = delay;
            reelSpinModel.CurrentReelStatus = ReelStatus.RecieveStop;

            Debug.Log("WillStop:" + (reelSpinModel.WillStopLowerPos + 1));
        }

        // ���[����������~������
        public void StopReelImmediately(int pushedPos, int delay)
        {
            Debug.Log("Received ReelStop");
            Debug.Log("Delay:" + delay);
            reelSpinModel.LastPushedPos = pushedPos;
            reelSpinModel.WillStopLowerPos = ReelObjectPresenter.OffsetReelPos(pushedPos, delay);
            reelSpinModel.LastStoppedDelay = delay;
            reelSpinModel.CurrentLower = reelSpinModel.WillStopLowerPos;
            FinishReelSpin();
        }

        // ���[���̉�]
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((reelSpinModel.ReturnDegreePerSecond()) * Time.deltaTime * reelSpinModel.RotateSpeed, -360, 360);
            transform.Rotate(rotationAngle * Vector3.left);

            OnReelDegreeChanged?.Invoke();
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
                reelSpinModel.CurrentLower = ReelObjectPresenter.OffsetReelPos(reelSpinModel.CurrentLower, -1);
                // �p�x�����Ƃɖ߂�
                transform.Rotate(Vector3.right, ChangeAngle * -1);

                // �ʒu�ύX��`����
                OnReelPositionChanged?.Invoke();

                // ��~�ʒu�ɂȂ������~����
                if (reelSpinModel.CurrentLower == reelSpinModel.WillStopLowerPos && reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    FinishReelSpin();
                }
            }
            // �O��]�̏ꍇ
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                reelSpinModel.CurrentLower = ReelObjectPresenter.OffsetReelPos(reelSpinModel.CurrentLower, 1);
                // �p�x�����Ƃɖ߂�
                transform.Rotate(Vector3.right, ChangeAngle);
                // �ʒu�ύX��`����
                OnReelPositionChanged?.Invoke();

                // ��~�ʒu�ɂȂ������~����
                if (reelSpinModel.CurrentLower == reelSpinModel.WillStopLowerPos && reelSpinModel.CurrentReelStatus == ReelStatus.RecieveStop)
                {
                    Debug.Log("Reached stop pos");
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
                reelSpinModel.MaxSpeed = 0.1f;
            }
            // �O��]�̏ꍇ
            else if (Math.Sign(reelSpinModel.RotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - StopAngle)
            {
                reelSpinModel.CurrentReelStatus = ReelStatus.Stopping;
                reelSpinModel.MaxSpeed = 0.1f;
            }
        }

        // ���[���̉�]���I������
        private void FinishReelSpin()
        {
            Debug.Log("Finished ReelSpin");
            transform.rotation = Quaternion.identity;
            reelSpinModel.FinishReelSpin();
            HasReelStopped?.Invoke();
        }
    }
}
