using ReelSpinGame_Payout;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // 払い出し後演出の条件
    public class AfterPayoutEffectCondition
    {
        public bool HasBonusStarted { get; set; }                   // ボーナスが開始したか
        public bool HasBonusFinished { get; set; }                  // ボーナスが終了したか
        public BigColor BigColor { get; set; }                      // BIG時の色
        public BonusStatus BonusStatus { get; set; }                // ボーナス中の状態

        public PayoutResultBuffer PayoutResult { get; private set; }        // 払い出し結果

        public AfterPayoutEffectCondition(PayoutResultBuffer payoutResult)
        {
            HasBonusStarted = false;
            HasBonusFinished = false;
            BigColor = BigColor.None;
            BonusStatus = BonusStatus.BonusNone;

            PayoutResult = payoutResult;
        }
    }
}