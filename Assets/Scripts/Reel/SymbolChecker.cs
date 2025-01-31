using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SymbolChecker
{
    // �}���̔���p

    // const

    // var

    class PayoutLineData
    {
        public sbyte[] PayoutLine { get; private set; }
        public byte BetCondition { get; private set; }

        public PayoutLineData(sbyte[] buffer)
        {
            // �Ō�̍s�ȊO�͕����o�����C���̃f�[�^�Ȃ̂ŁA�z��ɂ���
            PayoutLine = new sbyte[buffer.Length - 1]; 
            Array.Copy(buffer, PayoutLine, buffer.Length - 1);

            // �Ō�̍s����f�[�^��ǂݍ���
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

        // �f�o�b�O�p
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
