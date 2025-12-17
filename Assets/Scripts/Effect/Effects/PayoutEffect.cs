using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Effect.Data
{
    // 払い出し中の演出
    public class PayoutEffect : MonoBehaviour, IDoesEffect<PayoutEffectCondition>
    {
        // const
        const float ReplayWaitTime = 1.0f;                  // リプレイ時に待機させる時間(秒)
        public const float MedalUpdateTime = 0.12f;         // メダル更新の間隔(ミリ秒)

        // var
        public bool HasEffect { get; set; }  // 演出処理中か

        int remainingPayout;        // 残り払い出し枚数
        FlashManager flash; // リールフラッシュ
        SoundManager sound; // サウンド

        void Awake()
        {
            HasEffect = false;
            remainingPayout = 0;
            flash = GetComponent<FlashManager>();
            sound = GetComponent<SoundManager>();
        }

        // レバーオン時のエフェクト
        public void DoEffect(PayoutEffectCondition payoutEffectCondition)
        {
            // 払い出しがあれば再生
            if (payoutEffectCondition.PayoutResult.Payout > 0)
            {
                remainingPayout = payoutEffectCondition.PayoutResult.Payout;
                flash.ForceStopFlash();         // フラッシュ停止

                // JAC役ならJAC時の払い出しを再生
                if (payoutEffectCondition.Flag == FlagID.FlagJac)
                {
                    flash.EnableJacGameLight();
                    sound.PlaySE(sound.SoundDB.SE.JacPayout);
                }
                else
                {
                    // 15枚払い出しかで音を変える
                    flash.TurnOnAllReels();
                    if (payoutEffectCondition.PayoutResult.Payout >= 15)
                    {
                        sound.PlaySE(sound.SoundDB.SE.MaxPayout);
                    }
                    else
                    {
                        Debug.Log("Playing sound");
                        sound.PlaySE(sound.SoundDB.SE.NormalPayout);
                    }
                }

                flash.StartPayoutFlash(0f, payoutEffectCondition.PayoutResult, payoutEffectCondition.LastStoppedReel);
            }

            // 通常時のリプレイならフラッシュ再生
            else if (payoutEffectCondition.PayoutResult.IsReplayOrJacIn && 
                payoutEffectCondition.BonusStatus == BonusStatus.BonusNone)
            {
                flash.StartPayoutFlash(ReplayWaitTime, 
                    payoutEffectCondition.PayoutResult, payoutEffectCondition.LastStoppedReel);
                sound.PlaySE(sound.SoundDB.SE.Replay);
            }

            StartCoroutine(nameof(UpdatePayoutEffect));
        }

        // コルーチン処理
        IEnumerator UpdatePayoutEffect()
        {
            HasEffect = true;

            // 払い出し処理
            while (remainingPayout > 0)
            {
                Debug.Log("Remaining:" + remainingPayout);
                remainingPayout -= 1;
                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            sound.StopLoopSE();
            HasEffect = false;
        }
    }
}