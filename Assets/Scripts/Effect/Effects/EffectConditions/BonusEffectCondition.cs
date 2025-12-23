using ReelSpinGame_Payout;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // ボーナス状態の演出
    public class BonusEffectCondition
    {
        public BigColor BigColor { get; set; }          // BIG時の色
        public BonusStatus BonusStatus { get; set; }    // ボーナス中の状態

        public BonusEffectCondition()
        {
            BigColor = BigColor.None;
            BonusStatus = BonusStatus.BonusNone;
        }
    }
}