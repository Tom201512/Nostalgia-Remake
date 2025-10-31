using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Array;
using ReelSpinGame_Reels.Spin;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

namespace ReelSpinGame_Reels
{
    public class ReelObject : MonoBehaviour
    {
        // ���[���I�u�W�F�N�g

        // var
        // 1���Ԃ̉�]�� (Rotate Per Minute)
        [Range(0f, 80f), SerializeField] private float rotateRPM;

        // ���[�����
        [SerializeField] ReelDatabase reelDatabaseFile;

        // ���[������~�������̃C�x���g(�ʂ��Ƃ̃��[��)
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // ���[�����o�p�}�l�[�W���[
        public ReelEffect ReelEffectManager { get; private set; }

        // ���[����]�p�̃v���[���^�[
        private ReelSpinPresenter reelSpinPresenter;
        // ���[���z��p�̃v���[���^�[
        private ReelArrayPresenter reelArrayPresenter;

        // ���[�����
        private ReelData reelData;
        // JAC���̓_�������邩
        public bool HasJacModeLight { get; set; }
        // ���[�V�����u���[
        private PostProcessVolume postVolume;
        // �u���[�����̃v���t�@�C��
        private MotionBlur motionBlur;

        private void Awake()
        {
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
            ReelEffectManager = GetComponent<ReelEffect>();

            // �u���[�̎擾
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);

            reelSpinPresenter.SetReelSpinPresenter(rotateRPM);
        }

        private void Start()
        {
            ChangeBlurSetting(false);
        }

        // func

        // ���l�𓾂�
        // ���[����ID
        public int GetReelID() => reelData.ReelID;
        // ���݂̃��[�����
        public ReelStatus GetCurrentReelStatus() => reelSpinPresenter.GetCurrentReelStatus();
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

        // ���x
        // ���ݑ��x��Ԃ�
        public float GetCurrentSpeed() => reelSpinPresenter.GetCurrentSpeed();
        // ���݂̊p�x��Ԃ�
        public float GetCurrentDegree() => reelSpinPresenter.GetCurrentDegree();
        // ���[��������n��
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // ���[���f�[�^��n��
        public void SetReelData(int reelID, int initialLowerPos)
        {
            reelArrayPresenter = GetComponent<ReelArrayPresenter>();
            reelArrayPresenter.SetReelArrayPresenter(initialLowerPos, reelDatabaseFile.Array);
        }

        // ���[���ʒu�ύX(�Z�[�u�f�[�^�ǂݍ��ݗp)
        public void SetReelPos(int lowerPos) => reelArrayPresenter.SetCurrentLower(lowerPos);

        //�@���[���n��
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
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

            // JAC���Ȃ烉�C�g������
            if (HasJacModeLight)
            {
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center,SymbolLight.TurnOnValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
            }

            //ReelSpinPresenter.StopReelSpeed();
        }

        /*
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
        }*/
    }
}
