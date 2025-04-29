using System.Collections.Generic;
using System.IO;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class PayoutDatabaseGen
    {
        public static List<PayoutLineData> MakePayoutLineDatas(StreamReader payoutLineFile)
        {
            List<PayoutLineData> finalResult = new List<PayoutLineData>();

            // 全ての行を読み込む
            while (payoutLineFile.Peek() != -1)
            {
                finalResult.Add(new PayoutLineData(payoutLineFile));
            }
            return finalResult;
        }

        public static List<PayoutResultData> MakeResultDatas(StreamReader payoutResultFile)
        {
            List<PayoutResultData> finalResult = new List<PayoutResultData>();

            // 全ての行を読み込む
            while (payoutResultFile.Peek() != -1)
            {
                finalResult.Add(new PayoutResultData(payoutResultFile));
            }
            return finalResult;
        }
    }
#endif
}
