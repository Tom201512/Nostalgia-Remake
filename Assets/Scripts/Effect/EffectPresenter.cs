using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using ReelSpinGame_Payout;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;
using ReelSpinGame_Effect.Data;
using ReelSpinGame_Effect.Data.Condition;

namespace ReelSpinGame_Effect
{
    // リールフラッシュやサウンドなどの演出管理
    public class EffectPresenter : MonoBehaviour
    {
        // const
        const float ReplayWaitTime = 1.0f;        // リプレイ時に待機させる時間(秒)
        const float VFlashWaitTime = 2.0f;        // Vフラッシュ時の待機時間(秒)

        // var
        public bool HasBeforePayoutEffect { get; private set; }         // 払い出し前演出処理中か
        public bool HasPayoutEffectStart { get; private set; }          // 払い出し演出を開始したか
        public bool HasAfterPayoutEffect { get; private set; }          // 払い出し後演出が処理中か
        public bool HasBonusStart { get; private set; }        // ボーナスが開始されたか
        public bool HasBonusFinished { get; private set; }      // ボーナスが終了したか
        public BigColor BigChanceColor { get; private set; }    // ビッグチャンス時の色

        private ReelEffectManager reelEffectManager;        // リール演出マネージャー
        private FlashManager flashManager;                  // フラッシュ機能
        private SoundManager soundManager;                  // サウンド機能
        private BonusStatus lastBonusStatus;                // 直前のボーナス状態(同じBGMが再生されていないかチェック用)

        // 各種演出処理
        LeverOnEffect leverOnEffect;            // レバーオン時のエフェクト
        ReelStoppedEffect reelStoppedEffect;    // リール停止時のエフェクト

        // func 
        private void Awake()
        {
            reelEffectManager = GetComponent<ReelEffectManager>();
            flashManager = GetComponent<FlashManager>();
            soundManager = GetComponent<SoundManager>();

            leverOnEffect = GetComponent<LeverOnEffect>();
            reelStoppedEffect = GetComponent<ReelStoppedEffect>();
            HasBonusStart = false;
            HasBonusFinished = false;
            HasBeforePayoutEffect = false;
            HasAfterPayoutEffect = false;
        }

        private void Start()
        {
            flashManager.SetReelEffectManager(reelEffectManager);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // func
        // 数値を得る

        public bool GetHasFakeSpin() => reelEffectManager.HasFakeSpin;          // 疑似遊技中か
        public bool GetHasFlashWait() => flashManager.HasFlashWait;             // フラッシュの待機中か
        public void SetHasPayoutEffectStart() => HasPayoutEffectStart = true;   // 数値変更
        public void SetHasBonusStarted() => HasBonusStart = true;               // ボーナス開始されたか
        public void SetHasBonusFinished() => HasBonusFinished = true;           // ボーナスが終了したか

        // 疑似遊技関連
        // 疑似遊技を開始(試験用)
        public void StartFakeReelSpin()
        {
            reelEffectManager.StartFakeSpin();
        }

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

            // JAC中は回転中の明るさ計算を行う
            reelEffectManager.SetJacBrightnessCalculation(isJacGame);
        }

        // リールライト全消灯
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();

        // サウンド
        // ベット音再生
        public void StartBetEffect() => soundManager.PlaySE(soundManager.SoundDB.SE.Bet);
        // ウェイト音再生
        public void StartWaitEffect() => soundManager.PlaySE(soundManager.SoundDB.SE.Wait);

        // スタート時の演出
        public void StartLeverOnEffect(LeverOnEffectCondition leverOnEffectCondition) => leverOnEffect.DoEffect(leverOnEffectCondition);

        // リール停止時の演出
        public void StartReelStopEffect(ReelStoppedEffectCondition reelStoppedEffectCondition) => reelStoppedEffect.DoEffect(reelStoppedEffectCondition);

        // 払い出し前演出開始
        public void StartBeforePayoutEffect(FlagID flagID, BonusTypeID holdingBonusID, BonusStatus bonusStatus, bool hasBita)
        {
            // 全ての演出をリセット
            HasBeforePayoutEffect = false;
            HasPayoutEffectStart = false;
            HasAfterPayoutEffect = false;

            // 役ごとのフラッシュを発生
            switch (flagID)
            {
                // BIG時、REG時, またはボーナス当選後のはずれのとき1/6でVフラッシュ発生
                case FlagID.FlagBig:
                case FlagID.FlagReg:
                case FlagID.FlagNone:
                    if (holdingBonusID != BonusTypeID.BonusNone && OriginalRandomLot.LotRandomByNum(6))
                    {
                        flashManager.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
                        HasBeforePayoutEffect = true;
                    }
                    break;


                // チェリー2枚の場合
                case FlagID.FlagCherry2:
                    break;

                // チェリー4枚の場合
                case FlagID.FlagCherry4:
                    break;

                // ベルの場合:
                case FlagID.FlagBell:
                    break;

                // スイカの場合
                case FlagID.FlagMelon:
                    break;

                // リプレイの場合
                case FlagID.FlagReplayJacIn:
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
        public void StartPayoutEffect(FlagID flagID, BonusStatus bonusStatus, PayoutResultBuffer payoutResultData, LastStoppedReelData lastStoppedReelData)
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
                    if(flagID == FlagID.FlagJac)
                    {
                        TurnOnAllReels(true);
                        soundManager.PlaySE(soundManager.SoundDB.SE.JacPayout);
                    }
                    else
                    {
                        TurnOnAllReels(false);
                        soundManager.PlaySE(soundManager.SoundDB.SE.MaxPayout);
                    }

                }
                //　それ以外は通常の払い出し
                else
                {
                    TurnOnAllReels(false);
                    soundManager.PlaySE(soundManager.SoundDB.SE.NormalPayout);
                }

                flashManager.StartPayoutFlash(0f, payoutResultData, lastStoppedReelData);
            }

            // 通常時のリプレイならフラッシュ再生
            else if (payoutResultData.IsReplayOrJacIn && bonusStatus == BonusStatus.BonusNone)
            {
                flashManager.StartPayoutFlash(ReplayWaitTime, payoutResultData, lastStoppedReelData);
                soundManager.PlaySE(soundManager.SoundDB.SE.Replay);
            }

            HasPayoutEffectStart = true;
        }

        // 払い出し後演出開始
        public void StartAfterPayoutEffect(PayoutResultBuffer payoutResultData, BonusStatus bonusStatus)
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
            else if(payoutResultData.IsReplayOrJacIn && bonusStatus == BonusStatus.BonusNone)
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

        // ファンファーレ再生
        private void PlayFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlaySE(soundManager.SoundDB.SE.RedStart);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySE(soundManager.SoundDB.SE.BlueStart);
                    break;
                case BigColor.Black:
                    soundManager.PlaySE(soundManager.SoundDB.SE.BlackStart);
                    break;
                default:
                    soundManager.PlaySE(soundManager.SoundDB.SE.RegStart);
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
                    soundManager.PlaySE(soundManager.SoundDB.SE.RedEnd);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySE(soundManager.SoundDB.SE.BlueEnd);
                    break;
                case BigColor.Black:
                    soundManager.PlaySE(soundManager.SoundDB.SE.BlackEnd);
                    break;
            }
        }

        // コルーチン

        // 払い出し前演出処理
        private IEnumerator UpdateBeforePayoutEffect()
        {
            // 今鳴らしているジングルとフラッシュが止まるのを待つ
            while (!soundManager.GetSoundStopped() || !soundManager.GetJingleStopped() || flashManager.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasBeforePayoutEffect = false;
        }

        // 払い出し後演出処理
        private IEnumerator UpdateAfterPayoutEffect()
        {
            // 今鳴らしているジングルとフラッシュが止まるのを待つ
            while (!soundManager.GetSoundStopped() || !soundManager.GetJingleStopped() || flashManager.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasAfterPayoutEffect = false;
        }

        // ボーナス当選ファンファーレ再生処理
        private IEnumerator UpdateBonusFanfare()
        {
            // 今鳴らしている効果音が止まるのを待つ
            while (!soundManager.GetSoundStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // ファンファーレを鳴らす
            PlayFanfare();
            // 今鳴らしているファンファーレが止まるのを待つ
            while (!soundManager.GetJingleStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            HasBonusStart = false;
        }

        // ボーナス終了ファンファーレ再生処理
        private IEnumerator UpdateBonusEndFanfare()
        {
            // 今鳴らしている効果音が止まるのを待つ
            while (!soundManager.GetSoundStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // 音楽停止
            soundManager.StopBGM();
            // BIGの時のみファンファーレを鳴らす
            if (BigChanceColor != BigColor.None)
            {
                PlayBigEndFanfare();
                BigChanceColor = BigColor.None;
                // 今鳴らしているファンファーレが止まるのを待つ
                while (!soundManager.GetJingleStopped())
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            HasBonusFinished = false;
        }
    }
}
