using ReelSpinGame_Flag;
using ReelSpinGame_Bonus;

namespace ReelSpinGame_Reels
{
    // リールテーブルのメイン条件
    public class ReelMainCondition
    {
        public FlagModel.FlagID Flag { get; set; }            // フラグ
        public BonusModel.BonusTypeID Bonus { get; set; }      // 成立ボーナス
        public int Bet { get; set; }                // ベット枚数
        public int Random { get; set; }             // ランダム数値

        public ReelMainCondition()
        {
            Flag = FlagModel.FlagID.FlagNone;
            Bonus = BonusModel.BonusTypeID.BonusNone;
            Bet = 0;
            Random = 0;
        }
    }
}