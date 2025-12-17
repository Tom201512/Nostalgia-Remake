using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Effect.Data.Condition
{
    // 払い出し前演出の条件
    public class BeforePayoutEffectCondition
    {
        // var
        public FlagID Flag { get; set; }                    // フラグ
        public BonusTypeID HoldingBonus { get; set; }       // ストック中のボーナス 
        public BonusStatus BonusStatus { get; set; }        // ボーナス中の状態
        public int LastLeftStoppedPos { get; set; }         // 最後に止めた左リールの位置

        public BeforePayoutEffectCondition()
        {
            Flag = FlagID.FlagNone;
            HoldingBonus = BonusTypeID.BonusNone;
            BonusStatus = BonusStatus.BonusNone;
            LastLeftStoppedPos = 0;
        }
    }
}