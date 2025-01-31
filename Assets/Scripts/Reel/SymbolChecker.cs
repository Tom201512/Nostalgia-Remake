using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SymbolChecker
{
    // 図柄の判定用

    // const

    // var

    class PayoutLineData
    {
        public sbyte[] PayoutLine { get; private set; }
        public byte BetCondition { get; private set; }

        public PayoutLineData(sbyte[] buffer)
        {
            // 最後の行以外は払い出しラインのデータなので、配列にする
            PayoutLine = new sbyte[buffer.Length - 1]; 
            Array.Copy(buffer, PayoutLine, buffer.Length - 1);

            // 最後の行からデータを読み込む
            if (buffer[buffer.Length - 1] <= 0)
            {
                throw new Exception("Invalid Data at BetCondition, It must be within 1-3");
            }
            this.BetCondition = (byte)buffer[buffer.Length - 1];
        }
    }

    List<PayoutLineData> payoutLineDatas;

    public SymbolChecker(StreamReader payoutLineData)
    {
        payoutLineDatas = new List<PayoutLineData>();

        while(!payoutLineData.EndOfStream)
        {
            sbyte[] byteBuffer = Array.ConvertAll(payoutLineData.ReadLine().Split(','), sbyte.Parse);

            payoutLineDatas.Add(new PayoutLineData(byteBuffer));
        }

        // デバッグ用
        foreach (PayoutLineData data in payoutLineDatas)
        {
            string line = "";
            foreach (sbyte b in data.PayoutLine)
            {
                line += b.ToString();
            }
            Debug.Log(line + "," + data.BetCondition);
        }

        Debug.Log("PayoutLine Data loaded");
    }

    // func
}
