using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SymbolChecker
{
    // ê}ïøÇÃîªíËóp

    // const

    // var

    class PayoutLineData
    {
        const int LengthOfLinePattern = 3;
        const int MaxReadBetCondition = LengthOfLinePattern + 1;

        public sbyte[] PayoutLine { get; private set; }
        private byte betCondition;

        public PayoutLineData(sbyte[] buffer)
        {
            PayoutLine = new sbyte[LengthOfLinePattern];

            Array.Copy(buffer, PayoutLine, LengthOfLinePattern);
        }
    }

    List<PayoutLineData> payoutLineDatas;

    public SymbolChecker(StreamReader payoutLineData)
    {
        payoutLineDatas = new List<PayoutLineData>();

        while(!payoutLineData.EndOfStream)
        {
            string[] buffer = payoutLineData.ReadLine().Split(',');
            sbyte[] byteBuffer = Array.ConvertAll(buffer, sbyte.Parse);

            payoutLineDatas.Add(new PayoutLineData(byteBuffer));
        }

        foreach (PayoutLineData data in payoutLineDatas)
        {
            foreach (sbyte b in data.PayoutLine)
            {
                Debug.Log(b);
            }
        }
    }

    // func
}
