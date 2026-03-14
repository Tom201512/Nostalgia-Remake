using ReelSpinGame_Flag;
using ReelSpinGame_Bonus;

namespace ReelSpinGame_Effect.Data.Condition
{
    // 払い出し前演出の条件
    public class BeforePayoutEffectCondition
    {
        public FlagModel.FlagID Flag { get; set; }                    // フラグ
        public BonusModel.BonusTypeID HoldingBonus { get; set; }       // ストック中のボーナス 
        public BonusModel.BonusStatus BonusStatus { get; set; }        // ボーナス中の状態
        public int LastLeftStoppedPos { get; set; }         // 最後に止めた左リールの位置

        public BeforePayoutEffectCondition()
        {
            Flag = FlagModel.FlagID.FlagNone;
            HoldingBonus = BonusModel.BonusTypeID.BonusNone;
            BonusStatus = BonusModel.BonusStatus.BonusNone;
            LastLeftStoppedPos = 0;
        }
    }
}