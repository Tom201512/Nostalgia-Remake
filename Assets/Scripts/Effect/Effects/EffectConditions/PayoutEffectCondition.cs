using ReelSpinGame_Bonus;
using ReelSpinGame_Flag;
using ReelSpinGame_Payout;
using ReelSpinGame_Reels;

namespace ReelSpinGame_Effect.Data.Condition
{
    // 払い出し演出の条件
    public class PayoutEffectCondition
    {
        public FlagModel.FlagID Flag { get; set; }                            // フラグ
        public BonusModel.BonusStatus BonusStatus { get; set; }                // ボーナス中の状態
        public PayoutResultBuffer PayoutResult { get; set; }        // 払い出し結果
        public LastStoppedReelData LastStoppedReel { get; set; }    // 最後に停止させたリール

        public PayoutEffectCondition(PayoutResultBuffer payoutResult, LastStoppedReelData lastStop)
        {
            Flag = FlagModel.FlagID.FlagNone;
            BonusStatus = BonusModel.BonusStatus.BonusNone;
            PayoutResult = payoutResult;
            LastStoppedReel = lastStop;
        }
    }
}