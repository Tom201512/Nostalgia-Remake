using ReelSpinGame_Lots;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // レバーオン時演出の条件
    public class LeverOnEffectCondition
    {
        public FlagID Flag { get; set; }                   // フラグ
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