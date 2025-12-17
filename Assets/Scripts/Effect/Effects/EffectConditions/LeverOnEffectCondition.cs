using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Effect.Data.Condition
{
    // レバーオン時の条件
    public class LeverOnEffectCondition
    {
        // var
        public FlagID Flag {  get; set; }                   // フラグ
        public BonusTypeID HoldingBonus { get; set; }       // ストック中のボーナス 
        public BonusStatus BonusStatus { get; set; }        // ボーナス中の状態 

        public LeverOnEffectCondition()
        {
            Flag = FlagID.FlagNone;
            HoldingBonus = BonusTypeID.BonusNone;
            BonusStatus = BonusStatus.BonusNone;
        }
    }
}