using ReelSpinGame_Payout;
using ReelSpinGame_Reels;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Effect.Data.Condition
{
    // 払い出し演出の条件
    public class PayoutEffectCondition
    {
        // var
        public FlagID Flag { get; set; }                            // フラグ
        public BonusStatus BonusStatus { get; set; }                // ボーナス中の状態
        public PayoutResultBuffer PayoutResult { get; set; }        // 払い出し結果
        public LastStoppedReelData LastStoppedReel { get; set; }    // 最後に停止させたリール

        public PayoutEffectCondition(PayoutResultBuffer payoutResult, LastStoppedReelData lastStop)
        {
            Flag = FlagID.FlagNone;
            BonusStatus = BonusStatus.BonusNone;
            PayoutResult = payoutResult;
            LastStoppedReel = lastStop;
        }
    }
}