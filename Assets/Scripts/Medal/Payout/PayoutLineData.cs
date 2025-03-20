namespace ReelSpinGame_Medal.Payout.Lines
{
    // 払い出しラインのデータ
    public class PayoutLineData
	{
        // const
        // バッファからデータを読み込む位置
        public enum ReadPos { BetCondition = 0, PayoutLineStart }

        // 有効に必要なベット枚数
        public byte BetCondition { get; private set; }

        // 払い出しライン(符号付きbyte)
        public sbyte[] PayoutLine { get; private set; }

        //コンストラクタ
        public PayoutLineData(byte betCondition, sbyte[] lines)
        {
            this.BetCondition = betCondition;
            this.PayoutLine = lines;
        }
    }
}