using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data
{
    // 払い出し後の演出
    public class AfterPayoutEffect : MonoBehaviour, IDoesEffect<AfterPayoutEffectCondition>
    {
        // const

        // var
        public bool HasEffect { get; set; }  // 演出処理中か
        FlashManager flash; // リールフラッシュ
        SoundManager sound; // サウンド

        BigColor currentBigColor;      // 現在のBIG図柄色

        void Awake()
        {
            currentBigColor = BigColor.None;
            HasEffect = false;
            flash = GetComponent<FlashManager>();
            sound = GetComponent<SoundManager>();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // レバーオン時のエフェクト
        public void DoEffect(AfterPayoutEffectCondition afterPayoutEffectCondition)
        {
            currentBigColor = afterPayoutEffectCondition.BigColor;

            // ボーナス開始、終了していれば演出を行う
            if (afterPayoutEffectCondition.HasBonusStarted)
            {
                StartCoroutine(nameof(UpdateBonusFanfare));
            }
            else if (afterPayoutEffectCondition.HasBonusFinished)
            {
                StartCoroutine(nameof(UpdateBonusEndFanfare));
            }
            // リプレイ発生時は待機させる
            else if (afterPayoutEffectCondition.PayoutResult.IsReplayOrJacIn &&
                afterPayoutEffectCondition.BonusStatus == BonusStatus.BonusNone)
            {
                StartCoroutine(nameof(UpdateAfterPayoutEffect));
            }
        }

        // ファンファーレ再生
        void PlayFanfare()
        {
            switch (currentBigColor)
            {
                case BigColor.Red:
                    sound.PlaySE(sound.SoundDB.SE.RedStart);
                    break;
                case BigColor.Blue:
                    sound.PlaySE(sound.SoundDB.SE.BlueStart);
                    break;
                case BigColor.Black:
                    sound.PlaySE(sound.SoundDB.SE.BlackStart);
                    break;
                default:
                    sound.PlaySE(sound.SoundDB.SE.RegStart);
                    break;
            }
        }

        // 終了ジングル再生(BIGのみ)
        void PlayBigEndFanfare()
        {
            switch (currentBigColor)
            {
                case BigColor.Red:
                    sound.PlaySE(sound.SoundDB.SE.RedEnd);
                    break;
                case BigColor.Blue:
                    sound.PlaySE(sound.SoundDB.SE.BlueEnd);
                    break;
                case BigColor.Black:
                    sound.PlaySE(sound.SoundDB.SE.BlackEnd);
                    break;
            }
        }

        // コルーチン
        // 払い出し後演出処理
        private IEnumerator UpdateAfterPayoutEffect()
        {
            HasEffect = true;

            // 今鳴らしているジングルとフラッシュが止まるのを待つ
            while (!sound.GetSoundStopped() || !sound.GetJingleStopped() || flash.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasEffect = false;
        }

        // ボーナス当選ファンファーレ再生処理
        private IEnumerator UpdateBonusFanfare()
        {
            HasEffect = true;

            // 今鳴らしている効果音が止まるのを待つ
            while (!sound.GetSoundStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // ファンファーレを鳴らす
            PlayFanfare();
            // 今鳴らしているファンファーレが止まるのを待つ
            while (!sound.GetJingleStopped())
            {
                yield return new WaitForEndOfFrame();
            }

            HasEffect = false;
        }

        // ボーナス終了ファンファーレ再生処理
        private IEnumerator UpdateBonusEndFanfare()
        {
            HasEffect = true;

            // 今鳴らしている効果音が止まるのを待つ
            while (!sound.GetSoundStopped())
            {
                yield return new WaitForEndOfFrame();
            }
            // 音楽停止
            sound.StopBGM();
            // BIGの時のみファンファーレを鳴らす
            if (currentBigColor != BigColor.None)
            {
                PlayBigEndFanfare();
                currentBigColor = BigColor.None;
                // 今鳴らしているファンファーレが止まるのを待つ
                while (!sound.GetJingleStopped())
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            HasEffect = false;
        }
    }
}