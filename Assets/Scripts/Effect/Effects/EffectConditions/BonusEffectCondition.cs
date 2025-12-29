using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // ボーナス状態の演出
    public class BonusEffectCondition
    {
        public BigType BigType { get; set; }          // BIG時の種類
        public BonusStatus BonusStatus { get; set; }    // ボーナス中の状態

        public BonusEffectCondition()
        {
            BigType = BigType.None;
            BonusStatus = BonusStatus.BonusNone;
        }
    }
}