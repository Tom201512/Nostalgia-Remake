using ReelSpinGame_Datas;
using ReelSpinGame_Effect;
using ReelSpinGame_Reels.Array;
using ReelSpinGame_Reels.Spin;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels
{
    public class ReelObjectPresenter : MonoBehaviour
    {
        // ���[���I�u�W�F�N�g�v���[���^�[

        // const
        // ���[�����ʗpID
        public enum ReelID { ReelLeft, ReelMiddle, ReelRight };

        // var
        // 1���Ԃ̉�]�� (Rotate Per Minute)
        [Range(0f, 80f), SerializeField] private float rotateRPM;

        // ���[�����
        [SerializeField] ReelDatabase reelDatabaseFile;
        // ���[������ID
        [SerializeField] private ReelID reelID;

        // ���[������~�������̃C�x���g(�ʂ��Ƃ̃��[��)
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // ���[�����o�p�}�l�[�W���[
        public ReelEffect ReelEffectManager { get; private set; }

        // ���[����]�p�̃v���[���^�[
        private ReelSpinPresenter reelSpinPresenter;
        // ���[���z��p�̃v���[���^�[
        private ReelArrayPresenter reelArrayPresenter;

        // JAC���̓_�������邩
        public bool HasJacModeLight { get; set; }

        private void Awake()
        {
            ReelEffectManager = GetComponent<ReelEffect>();
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
            reelArrayPresenter = GetComponent<ReelArrayPresenter>();

            reelSpinPresenter.SetReelSpinPresenter(rotateRPM);
            reelArrayPresenter.SetReelArrayPresenter(reelDatabaseFile.Array);
        }

        private void Start()
        {
            reelSpinPresenter.ChangeBlurSetting(false);
            reelSpinPresenter.OnReelPositionChanged += OnReelPosChangedCallback;
            reelSpinPresenter.OnReelDegreeChanged += OnReelDegreeChangedCallback;
            reelSpinPresenter.HasReelStopped += OnReelHasStoppedCallback;
        }

        private void OnDestroy()
        {
            reelSpinPresenter.OnReelPositionChanged -= OnReelPosChangedCallback;
            reelSpinPresenter.OnReelDegreeChanged -= OnReelDegreeChangedCallback;
            reelSpinPresenter.HasReelStopped -= OnReelHasStoppedCallback;
        }

        // func

        // ���l�𓾂�
        // ���[����ID
        public ReelID GetReelID() => reelID;

        // ���݂̃��[�����
        public ReelStatus GetCurrentReelStatus() => reelSpinPresenter.GetCurrentReelStatus();
        // ���݂̉��i�ʒu
        public int GetCurrentLower() => reelSpinPresenter.GetCurrentLower();
        // �Ō�Ɏ~�߂����i�ʒu
        public int GetLastPushedLowerPos() => reelSpinPresenter.GetLastPushedPos();
        // ��~�\��ʒu
        public int GetWillStopLowerPos() => reelSpinPresenter.GetWillStopLowerPos();
        // �Ō�Ɏ~�߂��Ƃ��̃f�B���C��
        public int GetLastDelay() => reelSpinPresenter.GetLastStoppedDelay();

        // �w�肵���ʒu�ɂ��郊�[���̔ԍ���Ԃ�
        public int GetReelPos(sbyte posID) => reelArrayPresenter.GetReelPos(reelSpinPresenter.GetCurrentLower(), posID);
        // �w�肵���ʒu�ɂ��郊�[���̐}����Ԃ�
        public ReelSymbols GetReelSymbol(sbyte posID) => reelArrayPresenter.GetReelSymbol(reelSpinPresenter.GetCurrentLower(), posID);
        // ��~����ʒu����w��ʒu�̐}����Ԃ�
        public ReelSymbols GetSymbolFromWillStop(sbyte posID) => reelArrayPresenter.GetReelSymbol(reelSpinPresenter.GetWillStopLowerPos(), posID);

        // �w�肵���ʒu�̐}���𓾂�
        public Sprite GetReelSymbolSprite(int reelPos) => reelArrayPresenter.GetReelSymbolSprite(reelPos);

        // ���ݑ��x��Ԃ�
        public float GetCurrentSpeed() => reelSpinPresenter.GetCurrentSpeed();
        // ���݂̊p�x��Ԃ�
        public float GetCurrentDegree() => reelSpinPresenter.GetCurrentDegree();

        // ���[��������n��
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // ���[���̏�����
        public void InitializeReel(int initialLowerPos)
        {
            reelSpinPresenter.SetCurrentLower(initialLowerPos);
            reelArrayPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // ���i�ʒu�̕ύX
        public void ChangeCurrentLower(int lowerPos) => reelSpinPresenter.SetCurrentLower(lowerPos);

        // JAC���̖��邳�v�Z�̐ݒ�
        public void SetJacBrightnessCalculate(bool value) => ReelEffectManager.SetJacBrightnessCalculate(value);

        //�@���[���n��
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
            reelSpinPresenter.ChangeBlurSetting(true);
        }

        // ���[����~
        public void StopReel(int pushedPos, int delay)
        {
            reelSpinPresenter.StartStopReelSpin(pushedPos, delay);
        }

        // ���[����~(������)
        public void StopReelFast(int pushedPos, int delay)
        {
            reelArrayPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // ���[���ʒu���ς�����Ƃ��̃R�[���o�b�N
        private void OnReelPosChangedCallback()
        {
            // JAC���ł���Ή�]���̖��邳�v�Z�����Z�b�g
            if(ReelEffectManager.HasJacBrightnessCalculate)
            {
                ReelEffectManager.ResetJacBrightnessCalculate(reelSpinPresenter.GetMaxSpeed());
            }
            reelArrayPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // ���[���p�x���ς�����Ƃ��̃R�[���o�b�N
        private void OnReelDegreeChangedCallback()
        {
            if (ReelEffectManager.HasJacBrightnessCalculate)
            {
                Debug.Log("Calculating");
                ReelEffectManager.CalculateJacBrightness(reelSpinPresenter.GetMaxSpeed());
            }
        }

        // ���[������~�����Ƃ��̃R�[���o�b�N
        private void OnReelHasStoppedCallback()
        {
            //Debug.Log("StoppedEvent called");
            // JAC���Ȃ烉�C�g������
            if (ReelEffectManager.HasJacBrightnessCalculate)
            {
                ReelEffectManager.FinishJacBrightnessCalculate();
            }
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
