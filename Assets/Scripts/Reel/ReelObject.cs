using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_Reels
{
    public class ReelObject : MonoBehaviour
    {
        // ���[���I�u�W�F�N�g

        // const
        // �}���ύX���̊p�x (360�x��21����)
        const float ChangeAngle = 360.0f / 21.0f;
        // ���[�����a(cm)
        const float ReelRadius = 12.75f;
        // JAC���̌��x�������l
        const float JacLightOffset = 0.4f;
        // �ō����x�܂ł̌o�ߎ���(�b)
        const float MaxSpeedReelTime = 0.3f;

        // var
        // 1���Ԃ̉�]�� (Rotate Per Minute)
        [Range(0f, 80f), SerializeField] private float rotateRPM;

        // ���݂̉�]���x
        private float rotateSpeed;
        // �ō����x
        private float maxSpeed;
        // �}���}�l�[�W���[
        private SymbolManager symbolManager;

        // ���[�����
        private ReelData reelData;
        // JAC���̓_�������邩
        public bool HasJacModeLight { get; set; }
        // ���[�V�����u���[
        private PostProcessVolume postVolume;
        // �u���[�����̃v���t�@�C��
        private MotionBlur motionBlur;

        // 1�b�Ԃ̉�]�� (Rotate Per Second)
        private float rotateRPS;

        // ���[������~�������̃C�x���g(�ʂ��Ƃ̃��[��)
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // ���[�����o�p�}�l�[�W���[
        private ReelEffect reelEffectManager;

        // ���[�����
        [SerializeField] ReelDatabase reelDatabaseFile;

        // ������
        private void Awake()
        {
            rotateSpeed = 0.0f;
            maxSpeed = 0.0f;
            rotateRPS = rotateRPM / 60.0f;
            HasJacModeLight = false;

            symbolManager = GetComponentInChildren<SymbolManager>();
            reelEffectManager = GetComponentInChildren<ReelEffect>();

            // �u���[�̎擾
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);
        }

        private void Start()
        {
            symbolManager.UpdateSymbolsObjects(reelData);
            ChangeBlurSetting(false);
        }

        private void Update()
        {
            if (maxSpeed != 0)
            {
                ChangeReelSpeed();
                RotateReel();
            }
        }

        // func

        // ���l�𓾂�
        // ���݂̃X�s�[�h
        public float GetCurrentSpeed() => rotateSpeed;
        // ���݂̊p�x
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;
        // ���[����ID
        public int GetReelID() => reelData.ReelID;
        // ���݂̃��[�����
        public ReelStatus GetCurrentReelStatus() => reelData.CurrentReelStatus;
        // �Ō�Ɏ~�߂����i�ʒu
        public int GetLastPushedPos() => reelData.LastPushedPos;
        // ��~�\��ʒu
        public int GetWillStopPos() => reelData.WillStopPos;
        // �Ō�Ɏ~�߂��Ƃ��̃f�B���C��
        public int GetLastDelay() => reelData.LastDelay;
        // �w�肵���ʒu�ɂ��郊�[���̔ԍ���Ԃ�
        public int GetReelPos(ReelPosID posID) => reelData.GetReelPos((sbyte)posID);
        // sbyte�œǂޏꍇ
        public int GetReelPos(sbyte posID) => reelData.GetReelPos(posID);

        // �w�肵���ʒu�̐}����Ԃ�
        public ReelSymbols GetReelSymbol(ReelPosID posID) => reelData.GetReelSymbol((sbyte)posID);
        // sbyte�œǂޏꍇ
        public ReelSymbols GetReelSymbol(sbyte posID) => reelData.GetReelSymbol(posID);
        // ��~�\��ʒu���烊�[���̐}����Ԃ�
        public ReelSymbols GetSymbolFromWillStop(ReelPosID posID) => reelData.GetSymbolFromWillStop((sbyte)posID);
        // sbyte�œǂޏꍇ
        public ReelSymbols GetSymbolFromWillStop(sbyte posID) => reelData.GetSymbolFromWillStop(posID);
        // ���[��������n��
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // �w��ԍ��̐}���摜��Ԃ�
        public Sprite GetSymbolImageAtPos(int pos) => symbolManager.GetSymbolImage(reelDatabaseFile.Array[pos]);

        // ���[���f�[�^��n��
        public void SetReelData(int reelID, int initialLowerPos)
        {
            reelData = new ReelData(reelID, initialLowerPos, reelDatabaseFile);
        }

        // ���[���ʒu�ύX(�Z�[�u�f�[�^�ǂݍ��ݗp)
        public void SetReelPos(int lowerPos) => reelData.SetReelPos(lowerPos);

        // �ō����x��Ԃ��Ԃ�
        public bool IsMaximumSpeed() => rotateSpeed == maxSpeed;

        //�@���[���n��
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            this.maxSpeed = maxSpeed;

            // �������J�b�g����ꍇ�͂����ɑ��x���グ��
            if (cutAccelerate)
            {
                rotateSpeed = maxSpeed;
            }
            reelData.BeginStartReel();
            ChangeBlurSetting(true);
        }

        // ���x�ύX
        public void AdjustMaxSpeed(float maxSpeed)
        {
            Debug.Log("Max speed changed:" + maxSpeed);
            if (Math.Sign(this.maxSpeed) != Math.Sign(maxSpeed))
            {
                rotateSpeed = 0f;
            }
            this.maxSpeed = maxSpeed;
        }

        // �u���[�ݒ�
        public void ChangeBlurSetting(bool value) => motionBlur.enabled.value = value;

        // ���[����~
        public void StopReel(int pushedPos, int delay) => reelData.BeginStopReel(pushedPos, delay, false);

        // ���[����~(������)
        public void StopReelFast(int pushedPos, int delay)
        {
            // ������~
            reelData.BeginStopReel(pushedPos, delay, true);
            // �}���ʒu�ύX
            symbolManager.UpdateSymbolsObjects(reelData);

            // JAC���Ȃ烉�C�g������
            if (HasJacModeLight)
            {
                reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center,SymbolLight.TurnOnValue);
                reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
                reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
            }

            StopReelSpeed();
        }

        // ���x�ύX
        private void ChangeReelSpeed()
        {
            // ��]���x���ō����x���Ⴏ��Ή����A������Ό���������B
            // �t��]�̏ꍇ
            if(Math.Sign(maxSpeed) == -1)
            {
                if (rotateSpeed > maxSpeed)
                {
                    Debug.Log("Speed Up Reverse");
                    rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * -1, maxSpeed, 0);
                }
                else if (rotateSpeed < maxSpeed)
                {
                    Debug.Log("Speed Down Reverse");
                    rotateSpeed = Mathf.Clamp(rotateSpeed -= ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * -1, maxSpeed, 0);
                }
            }
            // �O��]�̏ꍇ
            else
            {
                if (rotateSpeed < maxSpeed)
                {
                    rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * 1, 0, maxSpeed);
                }
                else if (rotateSpeed > maxSpeed)
                {
                    Debug.Log("Speed down");
                    rotateSpeed = Mathf.Clamp(rotateSpeed -= ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * 1, 0, maxSpeed);
                }
            }

            Debug.Log("RotateSpeed:" + rotateSpeed);
        }

        // JAC���̖��邳�v�Z(
        private byte CalculateJACBrightness(bool isNegative)
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

            if (isNegative)
            {
                CenterBright = Math.Clamp(SymbolLight.TurnOnValue - (distance * brightnessTest), 0, 255);
            }
            else
            {
                CenterBright = Math.Clamp(SymbolLight.TurnOffValue + (distance * brightnessTest), 0, 255);
            }
            return (byte)Math.Clamp(CenterBright, 0, 255);
        }

        // ���[����]
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((ReturnDegreePerSecond(rotateRPS)) * Time.deltaTime * rotateSpeed, -360, 360);
            Debug.Log("RotationAngle:" + rotationAngle);

            Debug.Log(rotationAngle * Vector3.left);
            transform.Rotate(rotationAngle * Vector3.left);

            // JAC���ł���΃��C�g�̒���������
            if (HasJacModeLight)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(false));
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, CalculateJACBrightness(true));
                }
                else
                {
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, CalculateJACBrightness(false));
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(true));
                }
            }

            Debug.Log("Euler:" + transform.rotation.eulerAngles.x);
            //Debug.Log("ChangeAngle:" + (360f - ChangeAngle));

            // ���p�x�ɒB������}���̍X�V(17.14286�x)

            // ��]���x�ɍ��킹�ĕύX
            // �t��]�̏ꍇ
            if(Math.Sign(rotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                Debug.Log("Symbol changed");
                // �}���ʒu�ύX
                reelData.ChangeReelPos(rotateSpeed);
                symbolManager.UpdateSymbolsObjects(reelData);

                // �p�x�����Ƃɖ߂�
                transform.Rotate(Vector3.right, ChangeAngle * -1);

                if (HasJacModeLight)
                {
                    if (Math.Sign(maxSpeed) == -1)
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
                    }
                }

                // ��~����ʒu�ɂȂ�����
                if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }
            }
            // �O��]�̏ꍇ
            else if(Math.Sign(rotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                Debug.Log("Symbol changed");
                // �}���ʒu�ύX
                reelData.ChangeReelPos(rotateSpeed);
                symbolManager.UpdateSymbolsObjects(reelData);

                // �p�x�����Ƃɖ߂�
                transform.Rotate(Vector3.right, ChangeAngle);


                if (HasJacModeLight)
                {
                    if (Math.Sign(maxSpeed) == -1)
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
                    }
                }

                // ��~����ʒu�ɂȂ�����
                if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }
            }
        }

        // ���[���̉�]���~������
        private void StopReelSpeed()
        {
            // �ēx���[���̊p�x�𒲐����Ē�~������
            transform.rotation = Quaternion.identity;
            rotateSpeed = 0;
            maxSpeed = 0;
            reelData.FinishStopReel();
            HasReelStopped.Invoke();
        }

        // 1�b�ɉ��x��]�����邩�v�Z
        private float ReturnDegreePerSecond(float rpsValue)
        {
            // ���W�A�������߂�
            float radian = rpsValue * 2.0f * MathF.PI;

            // ���W�A�����疈�b�������p�x���v�Z

            float result = 180.0f / MathF.PI * radian;
            //Debug.Log("deg/s:" + result);
            return result;
        }

        // �����x��Ԃ�
        private float ReturnReelAccerateSpeed(float rpsValue)
        {
            // ���W�A�������߂�
            float radian = rpsValue * 2.0f * MathF.PI;
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
