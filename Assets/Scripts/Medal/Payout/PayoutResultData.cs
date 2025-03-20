namespace ReelSpinGame_Medal.Payout.Results
{
    // 払い出し結果のデータ
    public class PayoutResultData
    {
        // const
        // バッファからデータを読み込む位置
        public enum ReadPos { FlagID = 0, CombinationsStart = 1, Payout = 4, Bonus, IsReplay }
        // ANYの判定用ID
        public const int AnySymbol = 7;

        // var
        // フラグID
        public byte FlagID { get; private set; }
        // 図柄構成
        public byte[] Combinations { get; private set; }
        // 払い出し枚数
        public byte Payouts { get; private set; }
        // 当選するボーナス
        public byte BonusType { get; private set; }
        // リプレイか(またはJAC-IN)
        public bool hasReplayOrJAC { get; private set; }

        // コンストラクタ
        public PayoutResultData(byte flagID, byte[] combinations, byte payout,
            byte bonusType, bool hasReplayOrJAC)
        {
            this.FlagID = flagID;
            this.Combinations = combinations;
            this.Payouts = payout;
            this.BonusType = bonusType;
            this.hasReplayOrJAC = hasReplayOrJAC;
        }
    }
}