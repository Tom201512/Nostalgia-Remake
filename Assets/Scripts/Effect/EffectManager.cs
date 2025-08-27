using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;
using static ReelSpinGame_Reels.Payout.PayoutChecker;

namespace ReelSpinGame_Effect
{
    public class EffectManager : MonoBehaviour
    {
        // ���[���t���b�V����T�E���h�̊Ǘ�

        // const
        // ���v���C���ɑҋ@�����鎞��(�b)
        const float ReplayWaitTime = 1.0f;
        // V�t���b�V�����̑ҋ@����(�b)
        const float VFlashWaitTime = 2.0f;

        // var
        // �t���b�V���@�\
        private FlashManager flashManager;
        // �T�E���h�@�\
        private SoundManager soundManager;

        // �X�^�[�g���ɗ\���������邩
        [SerializeField] private bool hasSPStartSound;
        // �����o���O���o��������
        public bool HasBeforePayoutEffect { get; private set; }
        // �����o�����o���J�n������
        public bool HasPayoutEffectStart { get; private set; }
        // �����o���㉉�o����������
        public bool HasAfterPayoutEffect { get; private set; }

        // �r�b�O�`�����X���̐F
        public BigColor BigChanceColor { get; private set; }
        // ���O�̃{�[�i�X���(����BGM���Đ�����Ă��Ȃ����`�F�b�N�p)
        private BonusStatus lastBonusStatus;

        // �{�[�i�X���J�n���ꂽ��
        public bool HasBonusStart { get; private set; }
        // �{�[�i�X���I��������
        public bool HasBonusFinished { get; private set; }

        // ���[���̃I�u�W�F�N�g
        [SerializeField] private List<ReelObject> reelObjects;

        // func 
        private void Awake()
        {
            flashManager = GetComponent<FlashManager>();
            soundManager = GetComponent<SoundManager>();
            // ���[���I�u�W�F�N�g���蓖��
            flashManager.SetReelObjects(reelObjects);
            HasBonusStart = false;
            HasBonusFinished = false;
            HasBeforePayoutEffect = false;
            HasAfterPayoutEffect = false;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // �t���b�V���̑ҋ@����
        public bool GetHasFlashWait() => flashManager.HasFlashWait;
        // ����, BGM���~�܂��Ă��邩
        public bool GetAllSoundStopped() => soundManager.GetBGMStopped() && soundManager.GetSoundEffectStopped();

        // ���l�ύX
        public void SetHasPayoutEffectStart() => HasPayoutEffectStart = true;
        // �{�[�i�X�J�n���ꂽ��
        public void SetHasBonusStarted() => HasBonusStart = true;
        // �{�[�i�X���I��������
        public void SetHasBonusFinished() => HasBonusFinished = true;

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
                reel.HasJacModeLight = isJacGame;
            }
        }

        // ���[�����C�g�S����
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();

        // �T�E���h
        // �x�b�g���Đ�
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Bet);
        // �E�F�C�g���Đ�
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundDB.SE.Wait);

        // �X�^�[�g���̉��o
        public void StartLeverOnEffect(FlagId flag, BonusTypeID holding, BonusStatus bonusStatus)
        {
            if (hasSPStartSound)
            {
                // �ʏ펞�̂ݓ�����ʉ��Đ�
                if (bonusStatus == BonusStatus.BonusNone)
                {
                    // �ȉ��̊m���ō��m���ōĐ�(�����O)
                    // BIG/REG�������A�����㏬�������s���1/4
                    // �X�C�J�A1/8
                    // �`�F���[�A�������Ȃ�
                    // �x���A1/32
                    // ���v���C�A�������Ȃ�
                    // �͂���A1/128

                    if (holding == BonusTypeID.BonusNone)
                    {
                        // BIG, REG
                        switch (flag)
                        {
                            case FlagId.FlagBig:
                            case FlagId.FlagReg:
                                LotStartSound(4);
                                break;

                            case FlagId.FlagMelon:
                                LotStartSound(8);
                                break;

                            case FlagId.FlagBell:
                                LotStartSound(32);
                                break;

                            case FlagId.FlagNone:
                                LotStartSound(128);
                                break;

                            default:
                                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
                                break;
                        }
                    }
                    // �������1/4�ōĐ�
                    else
                    {
                        LotStartSound(4);
                    }

                }
                // ���̑��̏�Ԃł͖炳�Ȃ�
                else
                {
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
                }
            }
            else
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
        }

        // ���[����~���̉��o
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

        // �����o���O���o�J�n
        public void StartBeforePayoutEffect(FlagId flagID, BonusTypeID holdingBonusID, BonusStatus bonusStatus, bool hasBita)
        {
            // �S�Ẳ��o�����Z�b�g
            HasBeforePayoutEffect = false;
            HasPayoutEffectStart = false;
            HasAfterPayoutEffect = false;

            // �����Ƃ̃t���b�V���𔭐�
            switch (flagID)
            {
                // BIG���AREG��, �܂��̓{�[�i�X���I��̂͂���̂Ƃ�1/6��V�t���b�V������
                case FlagId.FlagBig:
                case FlagId.FlagReg:
                case FlagId.FlagNone:
                    if (holdingBonusID != BonusTypeID.BonusNone && OriginalRandomLot.LotRandomByNum(6))
                    {
                        flashManager.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
                        HasBeforePayoutEffect = true;
                    }
                    break;


                // �`�F���[2���̏ꍇ
                case FlagId.FlagCherry2:
                    break;

                // �`�F���[4���̏ꍇ
                case FlagId.FlagCherry4:
                    break;

                // �x���̏ꍇ:
                case FlagId.FlagBell:
                    break;

                // �X�C�J�̏ꍇ
                case FlagId.FlagMelon:
                    break;

                // ���v���C�̏ꍇ
                case FlagId.FlagReplayJacIn:
                    // �����Q�[������JACIN���������r�^�n�Y�V�������ꍇ
                    if (bonusStatus == BonusStatus.BonusBIGGames && hasBita)
                    {
                        flashManager.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
                        HasBeforePayoutEffect = true;
                    }

                    break;

                default:
                    break;
            }

            if(HasBeforePayoutEffect)
            {
            StartCoroutine(nameof(UpdateBeforePayoutEffect));
            }
        }

        // �����o�����o�J�n
        public void StartPayoutEffect(FlagId flagID, BonusStatus bonusStatus, PayoutResultBuffer payoutResultData, List<PayoutLineData> lastPayoutLines)
        {
            // �����o��������΍Đ�
            if (payoutResultData.Payout > 0)
            {
                // �t���b�V����~
                flashManager.ForceStopFlash();

                // �T�E���h�Đ�(��Ԃɍ��킹�ĕύX)
                // 15���̕����o����
                if (payoutResultData.Payout >= 15)
                {
                    // JAC���Ȃ�ύX
                    if(flagID == FlagId.FlagJac)
                    {
                        TurnOnAllReels(true);
                        soundManager.PlaySoundLoop(soundManager.SoundDB.SE.JacPayout);
                    }
                    else
                    {
                        TurnOnAllReels(false);
                        soundManager.PlaySoundLoop(soundManager.SoundDB.SE.MaxPayout);
                    }

                }
                //�@����ȊO�͒ʏ�̕����o��
                else
                {
                    TurnOnAllReels(false);
                    soundManager.PlaySoundLoop(soundManager.SoundDB.SE.NormalPayout);
                }

                flashManager.StartPayoutFlash(0f, lastPayoutLines);
            }

            // �ʏ펞�̃��v���C�Ȃ�t���b�V���Đ�
            else if (flagID == FlagId.FlagReplayJacIn && bonusStatus == BonusStatus.BonusNone)
            {
                flashManager.StartPayoutFlash(ReplayWaitTime, lastPayoutLines);
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Replay);
            }

            HasPayoutEffectStart = true;
        }

        // �����o���㉉�o�J�n
        public void StartAfterPayoutEffect(FlagId flagID, BonusStatus bonusStatus)
        {
            // �{�[�i�X�J�n�A�I�����Ă���Ή��o���s��
            if (HasBonusStart)
            {
                StartCoroutine(nameof(UpdateBonusFanfare));
                HasAfterPayoutEffect = true;
            }
            else if (HasBonusFinished)
            {
                StartCoroutine(nameof(UpdateBonusEndFanfare));
                HasAfterPayoutEffect = true;
            }

            // ���v���C�������͑ҋ@������
            else if(flagID == FlagId.FlagReplayJacIn && bonusStatus == BonusStatus.BonusNone)
            {
                HasAfterPayoutEffect = true;
            }

            if (HasAfterPayoutEffect)
            {
                StartCoroutine(nameof(UpdateAfterPayoutEffect));
            }
        }

        // �r�b�O���̐F�����蓖�Ă�
        public void SetBigColor(BigColor color) => BigChanceColor = color;
        // �t���b�V����~
        public void StopReelFlash() => flashManager.ForceStopFlash();
        // ���[�v���Ă��鉹���~�߂�
        public void StopLoopSound() => soundManager.StopLoopSE();

        // SE�{�����[���ύX (0.0 ~ 1.0)
        public void ChangeSEVolume(float volume) => soundManager.ChangeSEVolume(volume);
        // BGM�{�����[���ύX(0.0 ~ 1.0)
        public void ChangeBGMVolume(float volume) => soundManager.ChangeBGMVolume(volume);

        // �I�[�g�@�\���̌��ʉ��A���y����
        public void ChangeSoundSettingByAuto(bool hasAuto, int autoSpeedID)
        {
            if (hasAuto && autoSpeedID > (int)AutoPlaySpeed.Normal)
            {
                // �����ȏ��SE�Đ��s�\��
                soundManager.ChangeMuteSEPlayer(true);
                soundManager.ChangeLockSEPlayer(true);

                // �I�[�g���x���������Ȃ�BGM�̓~���[�g
                if (autoSpeedID == (int)AutoPlaySpeed.Quick)
                {
                    soundManager.ChangeMuteBGMPlayer(true);
                }
            }
            else
            {
                soundManager.ChangeMuteSEPlayer(false);
                soundManager.ChangeMuteBGMPlayer(false);
                soundManager.ChangeLockSEPlayer(false);
            }
        }

        // BGM���Đ�
        public void PlayBonusBGM(BonusStatus status, bool hasAutoFinished)
        {
            // �O��ƃ{�[�i�X��Ԃ��ς���Ă����BGM�Đ�(�I�[�g�I�������Đ�)
            if (hasAutoFinished || lastBonusStatus != status)
            {
                switch (status)
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

        // �w�肵���m���ōĐ����̒��I������
        private void LotStartSound(int probability)
        {
            // �m����0�ȉ��͒ʏ�X�^�[�g��
            if (probability <= 0)
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
            // �m����1�ȏ�Ȃ璊�I
            else if (OriginalRandomLot.LotRandomByNum(probability))
            {
                //Debug.Log("SP SOUND PLAYED");
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.SpStart);
            }
            else
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
        }

        // �t�@���t�@�[���Đ�
        private void PlayFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.RedStart);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BlueStart);
                    break;
                case BigColor.Black:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BlackStart);
                    break;
                default:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.RegStart);
                    break;
            }
        }

        // �����Q�[������BGM�Đ�
        private void PlayBigGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedBGM);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueBGM);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackBGM);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegJAC);
                    break;
            }
        }

        // �{�[�i�X�Q�[������BGM�Đ�
        private void PlayBonusGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedJAC);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueJAC);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackJAC);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegJAC);
                    break;
            }
        }

        // �I���W���O���Đ�(BIG�̂�)
        private void PlayBigEndFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.RedEnd);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BlueEnd);
                    break;
                case BigColor.Black:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BlackEnd);
                    break;
            }
        }

        // �R���[�`��

        // �����o���O���o����
        private IEnumerator UpdateBeforePayoutEffect()
        {
            // ���炵�Ă�����ʉ��ƃt���b�V�����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped() || flashManager.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasBeforePayoutEffect = false;
        }

        // �����o���㉉�o����
        private IEnumerator UpdateAfterPayoutEffect()
        {
            // ���炵�Ă�����ʉ��ƃt���b�V�����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped() || flashManager.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasAfterPayoutEffect = false;
        }

        // �{�[�i�X���I�t�@���t�@�[���Đ�����
        private IEnumerator UpdateBonusFanfare()
        {
            // ���炵�Ă�����ʉ����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // �t�@���t�@�[����炷
            PlayFanfare();
            // ���炵�Ă���t�@���t�@�[�����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            HasBonusStart = false;
        }

        // �{�[�i�X�I���t�@���t�@�[���Đ�����
        private IEnumerator UpdateBonusEndFanfare()
        {
            // ���炵�Ă�����ʉ����~�܂�̂�҂�
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }

            // BIG�̎��̂݃t�@���t�@�[����炷
            if (BigChanceColor != BigColor.None)
            {
                PlayBigEndFanfare();
                BigChanceColor = BigColor.None;
                // ���炵�Ă���t�@���t�@�[�����~�܂�̂�҂�
                while (!soundManager.GetSoundEffectStopped())
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            HasBonusFinished = false;
            soundManager.StopBGM();
        }
    }
}
