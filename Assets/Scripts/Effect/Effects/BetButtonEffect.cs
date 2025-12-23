using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using UnityEngine;

using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data
{
    // ベットボタン時の演出
    public class BetButtonEffect : MonoBehaviour, IDoesEffect<BetEffectCondition>
    {
        public bool HasEffect { get; set; }  // 演出処理中か

        FlashManager flash;         // フラッシュ
        ReelEffectManager reel;     // リール演出

        void Awake()
        {
            HasEffect = false;
            flash = GetComponent<FlashManager>();
            reel = GetComponent<ReelEffectManager>();
        }

        // ベットボタン時の演出
        public void DoEffect(BetEffectCondition leverOnEffectCondition)
        {
            // フラッシュを止める
            flash.ForceStopFlash();

            // JAC中かでフラッシュの点灯方法を変える
            if(leverOnEffectCondition.BonusStatus == BonusStatus.BonusJACGames)
            {
                flash.EnableJacGameLight();
            }
            else
            {
                flash.TurnOnAllReels();
            }

            reel.SetJacBrightnessCalculation(leverOnEffectCondition.BonusStatus == BonusStatus.BonusJACGames);
        }
    }
}