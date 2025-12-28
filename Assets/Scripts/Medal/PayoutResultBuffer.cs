using ReelSpinGame_Datas;
using System.Collections.Generic;

namespace ReelSpinGame_Payout
{
    // 払い出し結果
    public class PayoutResultBuffer
    {
        public int Payout { get; set; }             // 払い出し枚数
        public int BonusID { get; set; }            // 当選ボーナスID
        public bool IsReplayOrJacIn { get; set; }   // リプレイか

        public List<PayoutLineData> PayoutLines { get; set; }   // 払い出しのあったライン

        public PayoutResultBuffer(int payout, int bonusID, bool isReplayOrJac)
        {
            Payout = payout;
            BonusID = bonusID;
            IsReplayOrJacIn = isReplayOrJac;
            PayoutLines = new List<PayoutLineData>();
        }
    }
}