using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using ReelSpinGame_Lots;

namespace ReelSpinGame_Effect.Data
{
    // 払い出し中の演出
    public class PayoutEffect : MonoBehaviour, IDoesEffect<PayoutEffectCondition>
    {
        const float ReplayWaitTime = 1.0f;          // リプレイ時に待機させる時間(秒)
        const float MedalUpdateTime = 0.12f;        // メダル更新の間隔(ミリ秒)

        public bool HasEffect { get; set; }  // 演出処理中か

        int remainingPayout;        // 残り払い出し枚数
        FlashManager flash;         // リールフラッシュ
        SoundManager sound;         // サウンド

        void Awake()
        {
            HasEffect = false;
            remainingPayout = 0;
            flash = GetComponent<FlashManager>();
            sound = GetComponent<SoundManager>();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // レバーオン時のエフェクト
        public void DoEffect(PayoutEffectCondition payoutEffectCondition)
        {
            // 払い出しがあれば再生
            if (payoutEffectCondition.PayoutResult.Payout > 0)
            {
                // フラッシュ停止
                flash.ForceStopFlash();
                // 払い出し枚数分SEを再生する
                remainingPayout = payoutEffectCondition.PayoutResult.Payout;

                // JAC役ならJAC時の払い出しを再生
                if (payoutEffectCondition.Flag == FlagID.FlagJac)
                {
                    flash.EnableJacGameLight();
                    sound.PlaySE(sound.SoundDB.SE.JacPayout);
                }
                else
                {
                    // 払い出しが15枚以上なら音を変える
                    flash.TurnOnAllReels();
                    if (payoutEffectCondition.PayoutResult.Payout >= 15)
                    {
                        sound.PlaySE(sound.SoundDB.SE.MaxPayout);
                    }
                    else
                    {
                        sound.PlaySE(sound.SoundDB.SE.NormalPayout);
                    }
                }

                flash.StartPayoutFlash(0f, payoutEffectCondition.PayoutResult, payoutEffectCondition.LastStoppedReel);
            }

            // 通常時のリプレイならフラッシュとSEを再生
            else if (payoutEffectCondition.PayoutResult.IsReplayOrJacIn && 
                payoutEffectCondition.BonusStatus == BonusStatus.BonusNone)
            {
                flash.StartPayoutFlash(ReplayWaitTime, 
                    payoutEffectCondition.PayoutResult, payoutEffectCondition.LastStoppedReel);
                sound.PlaySE(sound.SoundDB.SE.Replay);
            }

            StartCoroutine(nameof(UpdatePayoutEffect));
        }

        // 払い出し演出時の処理
        IEnumerator UpdatePayoutEffect()
        {
            HasEffect = true;

            // 残り枚数が0になるまで待機し、全て払い出したらSEを止める
            while (remainingPayout > 0)
            {
                remainingPayout -= 1;
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            sound.StopLoopSE();
            HasEffect = false;
        }
    }
}