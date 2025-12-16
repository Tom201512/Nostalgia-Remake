using ReelSpinGame_Datas;
using System.Collections.Generic;

namespace ReelSpinGame_Payout
{
    // ï•Ç¢èoÇµåãâ 
    public class PayoutResultBuffer
    {
        public int Payout { get; set; }
        public int BonusID { get; set; }
        public bool IsReplayOrJacIn { get; set; }

        // ï•Ç¢èoÇµÇÃÇ†Ç¡ÇΩÉâÉCÉì
        public List<PayoutLineData> PayoutLines { get; set; }

        public PayoutResultBuffer(int payout, int bonusID, bool isReplayOrJac)
        {
            Payout = payout;
            BonusID = bonusID;
            IsReplayOrJacIn = isReplayOrJac;
            PayoutLines = new List<PayoutLineData>();
        }
    }
}