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

            // �S�Ă̍s��ǂݍ���
            while (payoutLineFile.Peek() != -1)
            {
                finalResult.Add(new PayoutLineData(payoutLineFile));
            }
            return finalResult;
        }

        public static List<PayoutResultData> MakeResultDatas(StreamReader payoutResultFile)
        {
            List<PayoutResultData> finalResult = new List<PayoutResultData>();

            // �S�Ă̍s��ǂݍ���
            while (payoutResultFile.Peek() != -1)
            {
                finalResult.Add(new PayoutResultData(payoutResultFile));
            }
            return finalResult;
        }
    }
#endif
}
