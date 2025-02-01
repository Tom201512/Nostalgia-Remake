using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChecker
{
    // 図柄の判定用

    // const

    // var

    // 払い出しラインのデータ
    class PayoutLineData
    {
        // var

        // 払い出しライン(符号付きbyte)
        public sbyte[] PayoutLine { get; private set; }

        // 有効に必要なベット枚数
        public byte BetCondition { get; private set; }


        //コンストラクタ
        public PayoutLineData(sbyte[] buffer)
        {
            // 最後の行以外は払い出しラインのデータなので配列にする
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

    // 各払い出しラインのデータ
    private List<PayoutLineData> payoutLineDatas;


    // コンストラクタ
    public SymbolChecker(StreamReader payoutLineData)
    {
        // 払い出しラインの読み込み
        payoutLineDatas = new List<PayoutLineData>();

        // データ読み込み
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

    // ライン判定
    public int CheckPayout(ReelObject[] reelObjects, int betAmount)
    {
        // 払い出し枚数(最大15枚まで)
        int payoutAmounts = 0;

        // 各ラインから払い出しのチェックをする
        foreach (PayoutLineData lineData in payoutLineDatas)
        {
            // ベット枚数の条件を満たしているかチェック
            if (betAmount >= lineData.BetCondition)
            {
                // 結果をリストにまとめる
                List<ReelSymbols> result = new List<ReelData.ReelSymbols>();

                // 各リールの払い出しをチェック
                for(int i = 0; i < reelObjects.Length; i++)
                {
                    result.Add(reelObjects[i].ReelData.GetReelSymbol(lineData.PayoutLine[i]));
                }

                // デバッグ用
                string lineBuffer = "";
                foreach(byte b in lineData.PayoutLine)
                {
                    lineBuffer += b.ToString();
                }

                string resultBuffer = "";
                foreach (ReelSymbols symbol in result)
                {
                    resultBuffer += symbol.ToString();
                }
                Debug.Log(lineBuffer + "," + resultBuffer);

                // 図柄構成リストと見比べて該当するものがあれば当選。払い出し、ボーナス、リプレイ処理もする。
                // ボーナスは非当選でもストックされる

                // デバッグ用
                // 全て同じ図柄が揃っていたらHITを返す
                if (result[0] == result[1] && result[0] == result[2])
                {
                    Debug.Log("HIT!");

                    payoutAmounts = 1;
                }
            }
        }

        // 最終的な払い出し枚数を返す
        return payoutAmounts;
    }
}
