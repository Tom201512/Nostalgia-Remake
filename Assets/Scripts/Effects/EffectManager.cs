using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        public bool HasFlash { get; private set; }
        // �t���b�V���őҋ@����
        public bool HasFlashWait { get; private set; }
        // ���݂̃t���b�V��ID
        public int CurrentFlashID { get; private set; }
        // �{�[�i�X�����őҋ@����
        public bool HasFanfareUpdate { get; private set; }
        // �r�b�O�`�����X���̐F
        public BigColor BigChanceColor { get; private set; }
        // ���O�̃{�[�i�X���(����BGM���Đ�����Ă��Ȃ����`�F�b�N�p)
        private BonusStatus lastBonusStatus;

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
        public bool HasEffectFinished() => !flashManager.HasFlashWait && !soundManager.GetSoundEffectHasLoop() && !HasFanfareUpdate;

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

        // �{�[�i�X�J�n���̉��o
        public void StartBonusStartEffect(BigColor color)
        {
            // �r�b�O�`�����X���͑Ή������F�̃t�@���t�@�[�����Đ�
            BigChanceColor = color;
            StartCoroutine(nameof(UpdateBonusFanfare));
        }

        // �{�[�i�X�I�����̉��o
        public void StartBonusEndEffect(BigColor color)
        {
            // �r�b�O�`�����X���͑Ή������F�̃t�@���t�@�[�����Đ�
            BigChanceColor = color;
            StartCoroutine(nameof(UpdateEndFanfare));
        }


        // �t���b�V����~
        public void StopReelFlash() => flashManager.StopFlash();

        // ���[�v���Ă��鉹���~�߂�
        public void StopLoopSound() => soundManager.StopLoopSound();

        // BGM���Đ�
        public void PlayBonusBGM(BonusStatus status)
        {
            if(lastBonusStatus != status)
            {
                switch(status)
                {
                    case BonusStatus.BonusBIGGames:
                        PlayBigGameBGM();
                        break;
                    case BonusStatus.BonusJACGames:
                        PlayBonusGameBGM();
                        break;
                    case BonusStatus.BonusNone:
                        soundManager.StopBGM();
                        break;
                }
                lastBonusStatus = status;
            }
        }

        // �t�@���t�@�[���Đ�
        private void PlayFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedStart, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueStart, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackStart, false);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.BGMList.RegStart, false);
                    break;
            }
        }

        // �����Q�[������BGM�Đ�
        private void PlayBigGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedBGM, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueBGM, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackBGM, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.BGMList.RegJAC, true);
                    break;
            }
        }

        // �{�[�i�X�Q�[������BGM�Đ�
        private void PlayBonusGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedJAC, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueJAC, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackJAC, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.BGMList.RegJAC, true);
                    break;
            }
        }

        // �I���W���O���Đ�(BIG�̂�)
        private void PlayBigEndFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedEnd, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueEnd, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackEnd, false);
                    break;
            }
        }

        // �R���[�`��

        // �{�[�i�X���I�t�@���t�@�[���Đ�����
        private IEnumerator UpdateBonusFanfare()
        {
            HasFanfareUpdate = true;
            // ���炵�Ă�����ʉ����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // �t�@���t�@�[����炷
            PlayFanfare();
            // ���炵�Ă���t�@���t�@�[�����~�܂�̂�҂�
            while (!soundManager.GetBGMStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            HasFanfareUpdate = false;
        }

        // �{�[�i�X�I���t�@���t�@�[���Đ�����
        private IEnumerator UpdateEndFanfare()
        {
            HasFanfareUpdate = true;
            // ���炵�Ă�����ʉ����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // BIG�̎��̂݃t�@���t�@�[����炷
            if (BigChanceColor != BigColor.None)
            {
                PlayBigEndFanfare();
                // ���炵�Ă���t�@���t�@�[�����~�܂�̂�҂�
                while (!soundManager.GetBGMStopped())
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            HasFanfareUpdate = false;
            BigChanceColor = BigColor.None;
            soundManager.StopBGM();
        }
    }
}
