using ReelSpinGame_Lots;

using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay.AI
{
    // オートAI条件のパラメータークラス
    public class AutoAIConditionClass
    {
        public FlagID Flag { get; set; }                // フラグ
        public ReelID FirstPush { get; set; }           // 第一停止
        public BonusStatus BonusStatus { get; set; }    // ボーナス状態
        public BonusTypeID HoldingBonus { get; set; }   // ストック中のボーナス
        public int BigChanceGames { get; set; }         // 残りビッグチャンスゲーム数
        public int RemainingJacIn {  get; set; }        // 残りJAC-IN
        public int BetAmount {  get; set; }             // ベット枚数

        public AutoAIConditionClass()
        {
            Flag = FlagID.FlagNone;
            FirstPush = ReelID.ReelLeft;
            BonusStatus = BonusStatus.BonusNone;
            HoldingBonus = BonusTypeID.BonusNone;
            BigChanceGames = 0;
            RemainingJacIn = 0;
            BetAmount = 0;
        }
    }
}