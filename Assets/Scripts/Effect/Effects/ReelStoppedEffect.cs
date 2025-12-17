using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Sound;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data
{
    // リール停止時のエフェクト
    public class ReelStoppedEffect : MonoBehaviour, IDoesEffect<ReelStoppedEffectCondition>
    {
        // var
        public bool HasEffect { get; set; }  // 演出処理中か
        SoundManager sound; // サウンド

        void Awake()
        {
            sound = GetComponent<SoundManager>();
        }

        // レバーオン時のエフェクト
        public void DoEffect(ReelStoppedEffectCondition reelStoppedEffectCondition)
        {
            sound.PlaySE(sound.SoundDB.SE.Stop);

            // 通常時でBIG図柄がテンパイしていたらリーチ音を再生
            if(reelStoppedEffectCondition.StoppedReelCount == 2 &&
                reelStoppedEffectCondition.BonusStatus == BonusStatus.BonusNone)
            {
                switch (reelStoppedEffectCondition.RiichiBigColor)
                {
                    case BigColor.Red:
                        sound.PlaySE(sound.SoundDB.SE.RedRiichiSound);
                        break;
                    case BigColor.Blue:
                        sound.PlaySE(sound.SoundDB.SE.BlueRiichiSound);
                        break;
                    case BigColor.Black:
                        sound.PlaySE(sound.SoundDB.SE.BB7RiichiSound);
                        break;
                }
            }
        }
    }
}