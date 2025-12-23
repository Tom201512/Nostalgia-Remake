using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // ベット時演出の条件
    public class BetEffectCondition
    {
        public BonusStatus BonusStatus { get; set; }    // ボーナス中の状態 

        public BetEffectCondition()
        {
            BonusStatus = BonusStatus.BonusNone;
        }
    }
}