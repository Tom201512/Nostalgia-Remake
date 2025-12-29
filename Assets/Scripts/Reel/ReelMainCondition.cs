using ReelSpinGame_Lots;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Reels
{
    // リールテーブルのメイン条件
    public class ReelMainCondition
    {
        public FlagID Flag { get; set; }            // フラグ
        public BonusTypeID Bonus { get; set; }      // 成立ボーナス
        public int Bet { get; set; }                // ベット枚数
        public int Random { get; set; }             // ランダム数値

        public ReelMainCondition()
        {
            Flag = FlagID.FlagNone;
            Bonus = BonusTypeID.BonusNone;
            Bet = 0;
            Random = 0;
        }
    }
}