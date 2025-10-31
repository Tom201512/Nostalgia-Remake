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
    public class ReelObjectPresenter : MonoBehaviour
    {
        // ���[���I�u�W�F�N�g�v���[���^�[

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

        // ���[���I�u�W�F�N�g�̃��f��
        private ReelObjectModel reelObjectModel;
        // ���[����]�p�̃v���[���^�[
        private ReelSpinPresenter reelSpinPresenter;
        // ���[���z��p�̃v���[���^�[
        private ReelArrayPresenter reelArrayPresenter;

        // JAC���̓_�������邩
        public bool HasJacModeLight { get; set; }

        private void Awake()
        {
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
            ReelEffectManager = GetComponent<ReelEffect>();
            reelArrayPresenter = GetComponent<ReelArrayPresenter>();

            reelSpinPresenter.SetReelSpinPresenter(rotateRPM);
            reelArrayPresenter.SetReelArrayPresenter(reelDatabaseFile.Array);
        }

        private void Start()
        {
            reelSpinPresenter.ChangeBlurSetting(false);
            reelSpinPresenter.OnReelPositionChanged += OnReelPosChangedCallback;
            reelSpinPresenter.HasReelStopped += OnReelHasStoppedCallback;
            reelArrayPresenter.UpdateReelSymbols(reelObjectModel.CurrentLower);
        }

        private void OnDestroy()
        {
            reelSpinPresenter.OnReelPositionChanged -= OnReelPosChangedCallback;
            reelSpinPresenter.HasReelStopped -= OnReelHasStoppedCallback;
        }

        // func

        // ���l�𓾂�
        // ���[����ID
        public int GetReelID() => reelObjectModel.ReelID;
        // ���݂̃��[�����
        public ReelStatus GetCurrentReelStatus() => reelSpinPresenter.GetCurrentReelStatus();
        // �Ō�Ɏ~�߂����i�ʒu
        public int GetLastPushedLowerPos() => reelObjectModel.LastPushedLowerPos;
        // ��~�\��ʒu
        public int GetWillStopLowerPos() => reelObjectModel.WillStopLowerPos;
        // �Ō�Ɏ~�߂��Ƃ��̃f�B���C��
        public int GetLastDelay() => reelSpinPresenter.GetLastStoppedDelay();
        // �w�肵���ʒu�ɂ��郊�[���̔ԍ���Ԃ�
        public int GetReelPos(ReelPosID posID) => reelArrayPresenter.GetReelPos(reelObjectModel.CurrentLower, (sbyte)posID);

        // ���ݑ��x��Ԃ�
        public float GetCurrentSpeed() => reelSpinPresenter.GetCurrentSpeed();
        // ���݂̊p�x��Ԃ�
        public float GetCurrentDegree() => reelSpinPresenter.GetCurrentDegree();

        // ���[��������n��
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // ���[���f�[�^��n��
        public void SetReelData(int reelID, int initialLowerPos) => reelObjectModel = new ReelObjectModel(reelID, initialLowerPos);

        // func

        // ���i�̈ʒu����~�\��ʒu�ɂȂ�������Ԃ�
        public bool HasReachedStopPos() => reelObjectModel.CurrentLower == reelObjectModel.WillStopLowerPos;

        // �w�肵�����[���ʒu�ɂ���
        public void SetReelPos(int lowerPos) => reelObjectModel.CurrentLower = lowerPos;
        // �Ō�ɉ������ʒu��ݒ肷��
        public void SetLastPushedLowerPos(int lastPushedLowerPos) => reelObjectModel.LastPushedLowerPos = lastPushedLowerPos;
        // ��~�\��ʒu��ݒ肷��
        public void SetWillStopLowerPos(int willStoplowerPos) => reelObjectModel.WillStopLowerPos = willStoplowerPos;
        // ��~�ʒu�֘A�̐��l�����Z�b�g
        public void ResetStopValues()
        {
            reelObjectModel.LastPushedLowerPos = 0;
            reelObjectModel.WillStopLowerPos = 0;
        }

        //�@���[���n��
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
            reelSpinPresenter.ChangeBlurSetting(true);
        }

        // ���[����~
        public void StopReel(int pushedMiddlePos, int delay)
        {
            Debug.Log("Pushed at: " + pushedMiddlePos);
            Debug.Log("Delay:" + delay);
            reelObjectModel.LastPushedLowerPos = pushedMiddlePos;
            reelObjectModel.WillStopLowerPos = OffsetReelPos(pushedMiddlePos, delay);
            reelObjectModel.LastStoppedDelay = delay;
            reelSpinPresenter.StartStopReelSpin(delay);
        }

        // ���[����~(������)
        public void StopReelFast(int pushedMiddlePos, int delay)
        {
            // ������~
            reelObjectModel.LastPushedLowerPos = pushedMiddlePos;
            reelObjectModel.WillStopLowerPos = OffsetReelPos(pushedMiddlePos, delay);
            reelObjectModel.LastStoppedDelay = delay;
            reelObjectModel.CurrentLower = reelObjectModel.WillStopLowerPos;

            reelSpinPresenter.FinishReelSpin();
            reelArrayPresenter.UpdateReelSymbols(reelObjectModel.CurrentLower);

            // JAC���Ȃ烉�C�g������
            if (HasJacModeLight)
            {
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center,SymbolLight.TurnOnValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
            }

            //ReelSpinPresenter.StopReelSpeed();
        }

        // ���[���ʒu�ύX (��]���x�̕����ɍ��킹�ĕύX)
        public void ChangeReelPos(float rotateSpeed)
        {
            // �t��]�̏ꍇ
            if (Math.Sign(rotateSpeed) == -1)
            {
                if (reelObjectModel.CurrentLower - 1 < 0)
                {
                    reelObjectModel.CurrentLower = MaxReelArray - 1;
                }
                else
                {
                    reelObjectModel.CurrentLower -= 1;
                }
            }
            // �O��]�̏ꍇ
            else if (Math.Sign(rotateSpeed) == 1)
            {
                if (reelObjectModel.CurrentLower + 1 > MaxReelArray)
                {
                    reelObjectModel.CurrentLower = 0;
                }
                else
                {
                    reelObjectModel.CurrentLower += 1;
                }
            }
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

        // ���[���ʒu���ς�����Ƃ��̃R�[���o�b�N
        private void OnReelPosChangedCallback()
        {
            //Debug.Log("Reel pos changed");
            ChangeReelPos(reelSpinPresenter.GetCurrentSpeed());
            reelArrayPresenter.UpdateReelSymbols(reelObjectModel.CurrentLower);
        }

        // ���[������~�����Ƃ��̃R�[���o�b�N
        private void OnReelHasStoppedCallback()
        {
            Debug.Log("StoppedEvent called");
            HasReelStopped?.Invoke();
        }

        // ���[���ʒu���I�[�o�[�t���[���Ȃ����l�ŕԂ�
        public static int OffsetReelPos(int reelPos, int offset)
        {
            if (reelPos + offset < 0)
            {
                return MaxReelArray + reelPos + offset;
            }

            else if (reelPos + offset > MaxReelArray - 1)
            {
                return reelPos + offset - MaxReelArray;
            }
            // �I�[�o�[�t���[���Ȃ��Ȃ炻�̂܂ܕԂ�
            return reelPos + offset;
        }
    }
}
