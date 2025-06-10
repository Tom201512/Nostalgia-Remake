using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;

namespace ReelSpinGame_Effect
{
    public class EffectManager : MonoBehaviour
    {
        // ���[���t���b�V����T�E���h�̊Ǘ�

        // const
        // ���v���C���ɑҋ@�����鎞��(�b)
        const float ReplayWaitTime = 1.0f;
        // V�t���b�V���m��(1/n)
        const int VFlashProb = 6;

        // var
        // �t���b�V���@�\
        private FlashManager flashManager;
        // �T�E���h�@�\
        private SoundManager soundManager;
        // �t���b�V������
        public bool HasFlash { get; set; }
        // �t���b�V���őҋ@����
        public bool HasFlashWait { get; set; }
        // ���݂̃t���b�V��ID
        public int CurrentFlashID { get; set; }

        // ���[���̃I�u�W�F�N�g
        [SerializeField] private List<ReelObject> reelObjects;

        // func 
        private void Awake()
        {
            flashManager = GetComponent<FlashManager>();
            soundManager = GetComponent<SoundManager>();
        }

        private void Start()
        {
            // ���[���I�u�W�F�N�g���蓖��
            flashManager.SetReelObjects(reelObjects);
        }

        // ���o���I�����Ă��邩(�T�E���h�A�t���b�V���A�{�[�i�X�t�@���t�@�[���̂��ׂĂ���~��)
        public bool HasEffectFinished() => !flashManager.HasFlashWait && !soundManager.GetSoundEffectHasLoop();

        // �t���b�V���֘A
        // ���[���S�_��
        public void TurnOnAllReels(bool isJacGame)
        {
            // JAC GAME���͒��i�̂݌��点��
            if (isJacGame)
            {
                flashManager.EnableJacGameLight();
            }
            else
            {
                flashManager.TurnOnAllReels();
            }

            // JAC���̃��C�g����������
            foreach (ReelObject reel in reelObjects)
            {
                if (reel.HasJacModeLight != isJacGame)
                {
                    reel.HasJacModeLight = isJacGame;
                }
            }
        }

        // ���[�����C�g�S����
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();
        // ���[���t���b�V�����J�n������
        public void StartReelFlash(FlashID flashID) => flashManager.StartReelFlash(flashID);

        // �T�E���h
        // �x�b�g���Đ�
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundEffectList.Bet);
        // �E�F�C�g���Đ�
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundEffectList.Wait);
        // �X�^�[�g��
        public void StartLeverOnEffect() => soundManager.PlaySoundOneShot(soundManager.SoundEffectList.Start);
        // ��~��
        public void StartReelStopEffect() => soundManager.PlaySoundOneShot(soundManager.SoundEffectList.Stop);

        // ���[�`�����o
        public void StartRiichiEffect(BigColor color)
        {
            switch (color)
            {
                case BigColor.Red:
                    soundManager.PlaySoundOneShot(soundManager.SoundEffectList.RedRiichiSound);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySoundOneShot(soundManager.SoundEffectList.BlueRiichiSound);
                    break;
                case BigColor.Black:
                    soundManager.PlaySoundOneShot(soundManager.SoundEffectList.BB7RiichiSound);
                    break;
            }
        }

        // �����o�����o�J�n
        public void StartPayoutReelFlash(List<PayoutLineData> lastPayoutLines, BonusStatus status, int payouts)
        {
            // �t���b�V���Đ�
            flashManager.StartPayoutFlash(0f, lastPayoutLines);

            // �T�E���h�Đ�(��Ԃɍ��킹�ĕύX)
            // JAC���̕����o����
            if (status == BonusStatus.BonusJACGames)
            {
                soundManager.PlaySoundLoop(soundManager.SoundEffectList.JacPayout);
            }
            // 15���̕����o����
            else if (payouts >= 15)
            {
                soundManager.PlaySoundLoop(soundManager.SoundEffectList.MaxPayout);
            }
            //�@����ȊO�͒ʏ�̕����o����
            else
            {
                soundManager.PlaySoundLoop(soundManager.SoundEffectList.NormalPayout);
            }
        }

        // ���v���C�̉��o
        public void StartReplayEffect(List<PayoutLineData> lastPayoutLines)
        {
            //���Đ�
            soundManager.PlaySoundLoop(soundManager.SoundEffectList.Replay);
            // �t���b�V��������
            flashManager.StartPayoutFlash(ReplayWaitTime, lastPayoutLines);
        }

        // ���[�`�ڏo�����̉��o
        public void StartRiichiPatternEffect()
        {
            if (Random.Range(0, VFlashProb - 1) == 0)
            {
                flashManager.StartReelFlash(FlashID.V_Flash);
            }
        }

        // �t���b�V����~
        public void StopReelFlash() => flashManager.StopFlash();

        // ���[�v���Ă��鉹���~�߂�
        public void StopLoopSound() => soundManager.StopLoopSound();
    }
}
