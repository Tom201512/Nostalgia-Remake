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
        // リールフラッシュやサウンドの管理

        // const
        // リプレイ時に待機させる時間(秒)
        const float ReplayWaitTime = 1.0f;
        // Vフラッシュ時の待機時間(秒)
        const float VFlashWaitTime = 2.0f;

        // var
        // フラッシュ機能
        private FlashManager flashManager;
        // サウンド機能
        private SoundManager soundManager;

        // スタート音に予告音をつけるか
        [SerializeField] private bool hasSPStartSound;
        // 払い出し前演出処理中か
        public bool HasBeforePayoutEffect { get; private set; }
        // 払い出し演出を開始したか
        public bool HasPayoutEffectStart { get; private set; }
        // 払い出し後演出が処理中か
        public bool HasAfterPayoutEffect { get; private set; }

        // ビッグチャンス時の色
        public BigColor BigChanceColor { get; private set; }
        // 直前のボーナス状態(同じBGMが再生されていないかチェック用)
        private BonusStatus lastBonusStatus;

        // ボーナスが開始されたか
        public bool HasBonusStart { get; private set; }
        // ボーナスが終了したか
        public bool HasBonusFinished { get; private set; }

        // リールのオブジェクト
        [SerializeField] private List<ReelObject> reelObjects;

        // func 
        private void Awake()
        {
            flashManager = GetComponent<FlashManager>();
            soundManager = GetComponent<SoundManager>();
            // リールオブジェクト割り当て
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

        // フラッシュの待機中か
        public bool GetHasFlashWait() => flashManager.HasFlashWait;
        // 音声, BGMが止まっているか
        public bool GetAllSoundStopped() => soundManager.GetBGMStopped() && soundManager.GetSoundEffectStopped();

        // 数値変更
        public void SetHasPayoutEffectStart() => HasPayoutEffectStart = true;
        // ボーナス開始されたか
        public void SetHasBonusStarted() => HasBonusStart = true;
        // ボーナスが終了したか
        public void SetHasBonusFinished() => HasBonusFinished = true;

        // フラッシュ関連
        // リール全点灯
        public void TurnOnAllReels(bool isJacGame)
        {
            // JAC GAME中は中段のみ光らせる
            if (isJacGame)
            {
                flashManager.EnableJacGameLight();
            }
            else
            {
                flashManager.TurnOnAllReels();
            }

            // JAC中のライト処理をする
            foreach (ReelObject reel in reelObjects)
            {
                reel.HasJacModeLight = isJacGame;
            }
        }

        // リールライト全消灯
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();

        // サウンド
        // ベット音再生
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Bet);
        // ウェイト音再生
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundDB.SE.Wait);

        // スタート時の演出
        public void StartLeverOnEffect(FlagId flag, BonusTypeID holding, BonusStatus bonusStatus)
        {
            if (hasSPStartSound)
            {
                // 通常時のみ特殊効果音再生
                if (bonusStatus == BonusStatus.BonusNone)
                {
                    // 以下の確率で告知音で再生(成立前)
                    // BIG/REG成立時、成立後小役条件不問で1/4
                    // スイカ、1/8
                    // チェリー、発生しない
                    // ベル、1/32
                    // リプレイ、発生しない
                    // はずれ、1/128

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
                    // 成立後は1/4で再生
                    else
                    {
                        LotStartSound(4);
                    }

                }
                // その他の状態では鳴らさない
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

        // リール停止時の演出
        public void StartReelStopEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Stop);

        // リーチ時演出
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

        // 払い出し前演出開始
        public void StartBeforePayoutEffect(FlagId flagID, BonusTypeID holdingBonusID, BonusStatus bonusStatus, bool hasBita)
        {
            // 全ての演出をリセット
            HasBeforePayoutEffect = false;
            HasPayoutEffectStart = false;
            HasAfterPayoutEffect = false;

            // 役ごとのフラッシュを発生
            switch (flagID)
            {
                // BIG時、REG時, またはボーナス当選後のはずれのとき1/6でVフラッシュ発生
                case FlagId.FlagBig:
                case FlagId.FlagReg:
                case FlagId.FlagNone:
                    if (holdingBonusID != BonusTypeID.BonusNone && OriginalRandomLot.LotRandomByNum(6))
                    {
                        flashManager.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
                        HasBeforePayoutEffect = true;
                    }
                    break;


                // チェリー2枚の場合
                case FlagId.FlagCherry2:
                    break;

                // チェリー4枚の場合
                case FlagId.FlagCherry4:
                    break;

                // ベルの場合:
                case FlagId.FlagBell:
                    break;

                // スイカの場合
                case FlagId.FlagMelon:
                    break;

                // リプレイの場合
                case FlagId.FlagReplayJacIn:
                    // 小役ゲーム中にJACINが成立しビタハズシをした場合
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

        // 払い出し演出開始
        public void StartPayoutEffect(FlagId flagID, BonusStatus bonusStatus, PayoutResultBuffer payoutResultData, List<PayoutLineData> lastPayoutLines)
        {
            // 払い出しがあれば再生
            if (payoutResultData.Payout > 0)
            {
                // フラッシュ停止
                flashManager.ForceStopFlash();

                // サウンド再生(状態に合わせて変更)
                // 15枚の払い出し音
                if (payoutResultData.Payout >= 15)
                {
                    // JAC役なら変更
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
                //　それ以外は通常の払い出し
                else
                {
                    TurnOnAllReels(false);
                    soundManager.PlaySoundLoop(soundManager.SoundDB.SE.NormalPayout);
                }

                flashManager.StartPayoutFlash(0f, lastPayoutLines);
            }

            // 通常時のリプレイならフラッシュ再生
            else if (flagID == FlagId.FlagReplayJacIn && bonusStatus == BonusStatus.BonusNone)
            {
                flashManager.StartPayoutFlash(ReplayWaitTime, lastPayoutLines);
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Replay);
            }

            HasPayoutEffectStart = true;
        }

        // 払い出し後演出開始
        public void StartAfterPayoutEffect(FlagId flagID, BonusStatus bonusStatus)
        {
            // ボーナス開始、終了していれば演出を行う
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

            // リプレイ発生時は待機させる
            else if(flagID == FlagId.FlagReplayJacIn && bonusStatus == BonusStatus.BonusNone)
            {
                HasAfterPayoutEffect = true;
            }

            if (HasAfterPayoutEffect)
            {
                StartCoroutine(nameof(UpdateAfterPayoutEffect));
            }
        }

        // ビッグ時の色を割り当てる
        public void SetBigColor(BigColor color) => BigChanceColor = color;
        // フラッシュ停止
        public void StopReelFlash() => flashManager.ForceStopFlash();
        // ループしている音を止める
        public void StopLoopSound() => soundManager.StopLoopSE();

        // SEボリューム変更 (0.0 ~ 1.0)
        public void ChangeSEVolume(float volume) => soundManager.ChangeSEVolume(volume);
        // BGMボリューム変更(0.0 ~ 1.0)
        public void ChangeBGMVolume(float volume) => soundManager.ChangeBGMVolume(volume);

        // オート機能時の効果音、音楽解除
        public void ChangeSoundSettingByAuto(bool hasAuto, int autoSpeedID)
        {
            if (hasAuto && autoSpeedID > (int)AutoPlaySpeed.Normal)
            {
                // 高速以上でSE再生不可能に
                soundManager.ChangeMuteSEPlayer(true);
                soundManager.ChangeLockSEPlayer(true);

                // オート速度が超高速ならBGMはミュート
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

        // BGMを再生
        public void PlayBonusBGM(BonusStatus status, bool hasAutoFinished)
        {
            // 前回とボーナス状態が変わっていればBGM再生(オート終了時も再生)
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

        // 指定した確率で再生音の抽選をする
        private void LotStartSound(int probability)
        {
            // 確率が0以下は通常スタート音
            if (probability <= 0)
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
            // 確率が1以上なら抽選
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

        // ファンファーレ再生
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

        // 小役ゲーム中のBGM再生
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

        // ボーナスゲーム中のBGM再生
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

        // 終了ジングル再生(BIGのみ)
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

        // コルーチン

        // 払い出し前演出処理
        private IEnumerator UpdateBeforePayoutEffect()
        {
            // 今鳴らしている効果音とフラッシュが止まるのを待つ
            while (!soundManager.GetSoundEffectStopped() || flashManager.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasBeforePayoutEffect = false;
        }

        // 払い出し後演出処理
        private IEnumerator UpdateAfterPayoutEffect()
        {
            // 今鳴らしている効果音とフラッシュが止まるのを待つ
            while (!soundManager.GetSoundEffectStopped() || flashManager.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasAfterPayoutEffect = false;
        }

        // ボーナス当選ファンファーレ再生処理
        private IEnumerator UpdateBonusFanfare()
        {
            // 今鳴らしている効果音が止まるのを待つ
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // ファンファーレを鳴らす
            PlayFanfare();
            // 今鳴らしているファンファーレが止まるのを待つ
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            HasBonusStart = false;
        }

        // ボーナス終了ファンファーレ再生処理
        private IEnumerator UpdateBonusEndFanfare()
        {
            // 今鳴らしている効果音が止まるのを待つ
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }

            // BIGの時のみファンファーレを鳴らす
            if (BigChanceColor != BigColor.None)
            {
                PlayBigEndFanfare();
                BigChanceColor = BigColor.None;
                // 今鳴らしているファンファーレが止まるのを待つ
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
