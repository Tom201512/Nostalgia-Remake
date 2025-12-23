using ReelSpinGame_AutoPlay;
using ReelSpinGame_Effect.Data;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using UnityEngine;

namespace ReelSpinGame_Effect
{
    // リールフラッシュやサウンドなどの演出管理
    public class EffectPresenter : MonoBehaviour
    {
        private ReelEffectManager reelEffectManager;        // リール演出マネージャー
        private FlashManager flashManager;                  // フラッシュ機能
        private SoundManager soundManager;                  // サウンド機能

        // 各種演出処理
        private BetButtonEffect betButtonEffect;            // ベットボタン
        private LeverOnEffect leverOnEffect;                // レバーオン時
        private ReelStoppedEffect reelStoppedEffect;        // リール停止時
        private BeforePayoutEffect beforePayoutEffect;      // 払い出し前
        private PayoutEffect payoutEffect;                  // 払い出し中
        private AfterPayoutEffect afterPayoutEffect;        // 払い出し後
        private BonusEffect bonusEffect;                    // ボーナス中

        private void Awake()
        {
            reelEffectManager = GetComponent<ReelEffectManager>();
            flashManager = GetComponent<FlashManager>();
            soundManager = GetComponent<SoundManager>();

            betButtonEffect = GetComponent<BetButtonEffect>();
            leverOnEffect = GetComponent<LeverOnEffect>();
            reelStoppedEffect = GetComponent<ReelStoppedEffect>();
            beforePayoutEffect = GetComponent<BeforePayoutEffect>();
            payoutEffect = GetComponent<PayoutEffect>();
            afterPayoutEffect = GetComponent<AfterPayoutEffect>();
            bonusEffect = GetComponent<BonusEffect>();
        }

        void Start()
        {
            flashManager.SetReelEffectManager(reelEffectManager);
        }

        public bool GetHasFakeSpin() => reelEffectManager.HasFakeSpin;                  // 疑似遊技中か
        public bool GetHasFlashWait() => flashManager.HasFlashWait;                     // フラッシュの待機中か
        public bool GetHasBeforeEffectActivating() => beforePayoutEffect.HasEffect;     // 払い出し前の演出が実行中か
        public bool GetPayoutEffectActivating() => payoutEffect.HasEffect;              // 払い出し中演出が実行中か
        public bool GetAfterPayoutEffectActivating() => afterPayoutEffect.HasEffect;    // 払い出し後演出が実行中か

        // 疑似遊技を開始(試験用)
        public void StartFakeReelSpin()
        {
            reelEffectManager.StartFakeSpin();
        }

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
        // フラッシュ停止
        public void StopReelFlash() => flashManager.ForceStopFlash();

        // ループしている音を止める
        public void StopLoopSound() => soundManager.StopLoopSE();
        // SEボリューム変更 (0.0 ~ 1.0)
        public void ChangeSEVolume(float volume) => soundManager.ChangeSEVolume(volume);
        // BGMボリューム変更(0.0 ~ 1.0)
        public void ChangeBGMVolume(float volume) => soundManager.ChangeBGMVolume(volume);

        // オート機能時の効果音、音楽解除
        public void ChangeSoundSettingByAuto(bool hasAuto, AutoSpeedName autoSpeedID)
        {
            if (hasAuto && autoSpeedID > AutoSpeedName.Normal)
            {
                // 高速以上でSE再生不可能に
                soundManager.ChangeMuteSEPlayer(true);
                soundManager.ChangeLockSEPlayer(true);

                // オート速度が超高速ならBGMはミュート
                if (autoSpeedID == AutoSpeedName.Quick)
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

        // 演出開始
        // ベット時の演出
        public void StartBetEffect(BetEffectCondition condition) => betButtonEffect.DoEffect(condition);
        // ベット音再生
        public void StartPlayBetSound() => soundManager.PlaySE(soundManager.SoundDB.SE.Bet);
        // ウェイト音再生
        public void StartWaitEffect() => soundManager.PlaySE(soundManager.SoundDB.SE.Wait);
        // スタート時の演出
        public void StartLeverOnEffect(LeverOnEffectCondition condition) => leverOnEffect.DoEffect(condition);
        // リール停止時の演出
        public void StartReelStopEffect(ReelStoppedEffectCondition condition) => reelStoppedEffect.DoEffect(condition);
        // 払い出し前演出開始
        public void StartBeforePayoutEffect(BeforePayoutEffectCondition condition) => beforePayoutEffect.DoEffect(condition);
        // 払い出し演出開始
        public void StartPayoutEffect(PayoutEffectCondition condition) => payoutEffect.DoEffect(condition);
        // 払い出し後演出開始
        public void StartAfterPayoutEffect(AfterPayoutEffectCondition condition) => afterPayoutEffect.DoEffect(condition);
        // ボーナス中演出開始
        public void StartBonusEffect(BonusEffectCondition condition) => bonusEffect.DoEffect(condition);
    }
}
