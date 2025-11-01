using System;
using UnityEngine;

namespace ReelSpinGame_Reels.Spin
{
    public class ReelSpinModel
    {
        // const
        // �}���ύX���̊p�x (360�x��21����)
        public const float ChangeAngle = 360.0f / 21.0f;
        // �}����~���̊p�x (�ύX���p�x����1�x�������p�x)
        public const float StopAngle = ChangeAngle - 0.5f;
        // ���[�����a(cm)
        const float ReelRadius = 12.75f;
        // �ō����x�܂ł̌o�ߎ���(�b)
        const float MaxSpeedReelTime = 0.3f;

        // ���[���̏��(��~�A��]�A��~�M���󗝁A��~��)
        public enum ReelStatus { Stopped, Spinning, RecieveStop, Stopping}

        // var
        // ���݂̉�]���x
        public float RotateSpeed { get; set; }
        // �ō����x
        public float MaxSpeed { get; set; }
        // 1�b�Ԃ�RPS
        public float RotateRPS { get; private set; }
        // JAC���̓_�������邩
        public bool HasJacModeLight { get; set; }
        // ���݂̃��[�����
        public ReelStatus CurrentReelStatus { get; set; }

        // ���݂̈ʒu(���i�)
        public int CurrentLower { get; set; }
        // �������ʒu(���i�)
        public int LastPushedPos { get; set; }
        // �����I�Ɏ~�܂�ʒu(���i�)
        public int WillStopLowerPos { get; set; }
        // �Ō�Ɏ~�߂����̃X�x���R�}��
        public int LastStoppedDelay { get; set; }

        public ReelSpinModel(float rotateRPM)
        {
            RotateSpeed = 0.0f;
            MaxSpeed = 0.0f;
            RotateRPS = rotateRPM / 60.0f;
            CurrentLower = 0;
            HasJacModeLight = false;
            CurrentReelStatus = ReelStatus.Stopped;
        }

        // RPS�̕ύX
        public void ChangeRotateRPS(float rotateRPM) => RotateRPS = rotateRPM / 60.0f;

        // 1�b�ɉ��x��]�����邩�v�Z����
        public float ReturnDegreePerSecond()
        {
            // ���W�A�������߂�
            float radian = RotateRPS * 2.0f * MathF.PI;

            // ���W�A�����疈�b�������p�x���v�Z

            float result = 180.0f / MathF.PI * radian;
            //Debug.Log("deg/s:" + result);
            return result;
        }

        // ���x����
        public void AccelerateReelSpeed()
        {
            // ��]���x���ō����x���Ⴏ��Ή����A������Ό���������B
            // �t��]�̏ꍇ
            if (Math.Sign(MaxSpeed) == -1)
            {
                if (RotateSpeed > MaxSpeed)
                {
                    RotateSpeed = Mathf.Clamp(RotateSpeed += ReturnReelAccerateSpeed() * Time.deltaTime * -1, MaxSpeed, 0);
                }
                else if (RotateSpeed < MaxSpeed)
                {
                    RotateSpeed = Mathf.Clamp(RotateSpeed -= ReturnReelAccerateSpeed() * Time.deltaTime * -1, MaxSpeed, 0);
                }
            }
            // �O��]�̏ꍇ
            else
            {
                if (RotateSpeed < MaxSpeed)
                { 
                    RotateSpeed = Mathf.Clamp(RotateSpeed += ReturnReelAccerateSpeed() * Time.deltaTime * 1, 0, MaxSpeed);
                }
                else if (RotateSpeed > MaxSpeed)
                {
                    Debug.Log("Speed down");
                    RotateSpeed = Mathf.Clamp(RotateSpeed -= ReturnReelAccerateSpeed() * Time.deltaTime * 1, 0, MaxSpeed);
                }
            }
        }

        // ���[����]���I������
        public void FinishReelSpin()
        {
            Debug.Log("Finish ReelSpin");
            RotateSpeed = 0f;
            MaxSpeed = 0f;
            CurrentReelStatus = ReelStatus.Stopped;
        }

        // �����x��Ԃ�
        private float ReturnReelAccerateSpeed()
        {
            // ���W�A�������߂�
            float radian = RotateRPS * 2.0f * MathF.PI;
            // �ڐ����x(m/s)�����߂�
            float tangentalVelocity = ReelRadius * radian / 100f;
            //Debug.Log("TangentalVelocity:" + tangentalVelocity);

            // �K�v�o�ߎ��Ԃ��瑬�x���o��
            float speed = tangentalVelocity / MaxSpeedReelTime;
            //Debug.Log("Speed:" + speed);
            return speed;
        }
    }
}