using ReelSpinGame_Datas;
using ReelSpinGame_Lots;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;
using static ReelSpinGame_Lots.FlagBehaviour;

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
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Bet);
        // �E�F�C�g���Đ�
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundDB.SE.Wait);
        // �X�^�[�g��
        public void StartLeverOnEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);

        // �e�X�g�p(����X�^�[�g�@�\)
        public void StartLeverOnEffect(FlagId flag, BonusType holding, BonusStatus bonusStatus)
        {
            // BIG��
            if (bonusStatus == BonusStatus.BonusBIGGames)
            {
                // ���v���C�A�͂��ꎞ��1/6�ōĐ�
                if(flag == FlagId.FlagNone || flag == FlagId.FlagReplayJacIn)
                {
                    LotStartSound(6);
                }
                else
                {
                    LotStartSound(0);
                }
            }
            // �ʏ펞
            else if (bonusStatus == BonusStatus.BonusNone)
            {
                // �ȉ��̊m���ō��m���ōĐ�
                // BIG/REG�������A�����㏬�������s���1/6
                // �X�C�J�A1/8
                // �x���A�`�F���[�A1/16
                // ���v���C�A�������Ȃ�
                // �͂���A1/128

                if(holding != BonusType.BonusNone)
                {
                    // BIG, REG
                    switch(flag)
                    {
                        case FlagId.FlagBig:
                        case FlagId.FlagReg:
                            LotStartSound(6);
                            break;

                        case FlagId.FlagMelon:
                            LotStartSound(8);
                            break;

                        case FlagId.FlagCherry2:
                        case FlagId.FlagCherry4:
                        case FlagId.FlagBell:
                            LotStartSound(16);
                            break;

                        case FlagId.FlagReplayJacIn:
                            LotStartSound(0);
                            break;

                        default:
                            LotStartSound(128);
                            break;
                    }
                }

                // �������1/6�ōĐ�
                else
                {
                    LotStartSound(6);
                }

            }
            // JAC��(�炳�Ȃ�)
            else
            {
                LotStartSound(0);
            }
        }

        // �w�肵���m���ōĐ����̒��I������
        private void LotStartSound(int probability)
        {
            // �m����0���Ⴂ�ꍇ�͒ʏ�X�^�[�g��
            if(probability > 0)
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
            // �m����1�ȏ�Ȃ璊�I
            else if (Random.Range(0, probability - 1) == 0)
            {
                Debug.Log("SP SOUND PLAYED");
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.SpStart);
            }
            else
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
        }

        // ��~��
        public void StartReelStopEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Stop);

        // ���[�`�����o
        public void StartRiichiEffect(BigColor color)
        {
            switch (color)
            {
                case BigColor.Red:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.RedRiichiSound);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BlueRiichiSound);
                    break;
                case BigColor.Black:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BB7RiichiSound);
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
                soundManager.PlaySoundLoop(soundManager.SoundDB.SE.JacPayout);
            }
            // 15���̕����o����
            else if (payouts >= 15)
            {
                soundManager.PlaySoundLoop(soundManager.SoundDB.SE.MaxPayout);
            }
            //�@����ȊO�͒ʏ�̕����o����
            else
            {
                soundManager.PlaySoundLoop(soundManager.SoundDB.SE.NormalPayout);
            }
        }

        // ���v���C�̉��o
        public void StartReplayEffect(List<PayoutLineData> lastPayoutLines)
        {
            //���Đ�
            soundManager.PlaySoundAndWait(soundManager.SoundDB.SE.Replay);
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
        public void StartBonusEndEffect()
        {
            // �r�b�O�`�����X���͑Ή������F�̃t�@���t�@�[�����Đ�
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
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedStart, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueStart, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackStart, false);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegStart, false);
                    break;
            }
        }

        // �����Q�[������BGM�Đ�
        private void PlayBigGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedBGM, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueBGM, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackBGM, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegJAC, true);
                    break;
            }
        }

        // �{�[�i�X�Q�[������BGM�Đ�
        private void PlayBonusGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedJAC, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueJAC, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackJAC, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegJAC, true);
                    break;
            }
        }

        // �I���W���O���Đ�(BIG�̂�)
        private void PlayBigEndFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedEnd, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueEnd, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackEnd, false);
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
