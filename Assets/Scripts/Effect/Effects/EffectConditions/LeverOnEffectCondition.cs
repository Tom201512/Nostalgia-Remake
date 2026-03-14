using ReelSpinGame_Flag;
using ReelSpinGame_Bonus;

namespace ReelSpinGame_Effect.Data.Condition
{
    // レバーオン時演出の条件
    public class LeverOnEffectCondition
    {
        public FlagModel.FlagID Flag { get; set; }                   // フラグ
        public BonusModel.BonusTypeID HoldingBonus { get; set; }       // ストック中のボーナス 
        public BonusModel.BonusStatus BonusStatus { get; set; }        // ボーナス中の状態 

        public LeverOnEffectCondition()
        {
            Flag = FlagModel.FlagID.FlagNone;
            HoldingBonus = BonusModel.BonusTypeID.BonusNone;
            BonusStatus = BonusModel.BonusStatus.BonusNone;
        }
    }
}