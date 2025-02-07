using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChecker
{
    // 図柄の判定用

    // var

    // 払い出しラインのデータ
    class PayoutLineData
    {
        // const
        // バッファからデータを読み込む位置
        public enum ReadPos { BetCondition = 0, PayoutLineStart}

        // 有効に必要なベット枚数
        public byte BetCondition { get; private set; }

        // 払い出しライン(符号付きbyte)
        public sbyte[] PayoutLine { get; private set; }

        //コンストラクタ
        public PayoutLineData(sbyte[] buffer)
        {
            // ベット条件の読み込み
            this.BetCondition = (byte)buffer[(int)ReadPos.BetCondition];

            // 払い出しラインの読み込み
            // 図柄組み合わせのデータ読み込み(Payoutの位置まで読み込む)
            PayoutLine = new sbyte[ReelManager.ReelAmounts];

            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                PayoutLine[i] = buffer[i + (int)ReadPos.PayoutLineStart];
            }
        }
    }

    // 払い出し結果のデータ
    class PayoutResultData
    {
        // const

        // バッファからデータを読み込む位置
        public enum ReadPos { FlagID = 0, CombinationsStart = 1, Payout = 4, Bonus, IsReplay}

        public const int AnySymbol = 7;
        // var

        // フラグID
        public byte FlagID { get; private set; }
        // 図柄構成
        public byte[] Combinations{get; private set; }

        // 払い出し枚数
        public byte Payouts {get; private set; }

        // 当選するボーナス
        public byte BonusType { get; private set; }

        // リプレイか(またはJAC-IN)
        public byte IsReplayAndJac { get; private set; }

        // コンストラクタ

        public PayoutResultData(byte[] buffer)
        {
            // フラグID
            FlagID = buffer[(int)ReadPos.FlagID];

            // 図柄組み合わせのデータ読み込み(Payoutの位置まで読み込む)
            Combinations = new byte[ReelManager.ReelAmounts];

            Debug.Log(Combinations.Length);
            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                Combinations[i] = buffer[i + (int)ReadPos.CombinationsStart];
                Debug.Log(Combinations[i]);
            }

            // 払い出し枚数
            Payouts = buffer[(int)ReadPos.Payout];

            // 当選するボーナス
            BonusType = buffer[(int)ReadPos.Bonus];

            // リプレイか
            IsReplayAndJac = buffer[(int)ReadPos.IsReplay];
        }
        
        // func
        public void ShowData()
        {
            string debugData = "";

            string symbols = "";
            foreach (byte b in Combinations)
            {
                symbols += b.ToString();
            }

            debugData += FlagID + ",";
            debugData += symbols + ",";
            debugData += Payouts + ",";
            debugData += BonusType + ",";
            debugData += IsReplayAndJac + ",";

            Debug.Log(debugData);
        }
    }

    // 各払い出しラインのデータ
    private List<PayoutLineData> payoutLineDatas;

    // 各種払い出し構成のテーブル

    // 通常時
    private List<PayoutResultData> normalPayoutDatas;

    // 小役ゲーム中
    private List<PayoutResultData> bigPayoutDatas;

    // JACゲーム中
    private List<PayoutResultData> jacPayoutDatas;


    // コンストラクタ
    public SymbolChecker(StreamReader normalPayoutData, StreamReader payoutLineData)
    {
        // 払い出し構成の読み込み

        normalPayoutDatas = new List<PayoutResultData>();
        bigPayoutDatas = new List<PayoutResultData>();
        jacPayoutDatas = new List<PayoutResultData>();

        // データ読み込み
        while (!normalPayoutData.EndOfStream)
        {
            byte[] byteBuffer = Array.ConvertAll(normalPayoutData.ReadLine().Split(','), byte.Parse);
            normalPayoutDatas.Add(new PayoutResultData(byteBuffer));
        }

        // デバッグ用
        foreach (PayoutResultData data in normalPayoutDatas)
        {
            data.ShowData();
        }
        Debug.Log("NormalPayoutData loaded");

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

        // イベント受け取り用読み込み
    }

    // func

    // ライン判定
    public ReelManager.PayoutResultBuffer CheckPayoutLines(ReelObject[] reelObjects, int betAmount)
    {
        byte finalPayouts = 0;
        byte bonusID = 0;
        byte replayStatus = 0;

        // 各ラインから払い出しのチェックをする
        foreach (PayoutLineData lineData in payoutLineDatas)
        {
            // ベット枚数の条件を満たしているかチェック
            if (betAmount >= lineData.BetCondition)
            {
                // 結果をリストにまとめる
                List<ReelSymbols> lineResult = new List<ReelData.ReelSymbols>();

                // 各リールの払い出しをチェック
                for(int i = 0; i < reelObjects.Length; i++)
                {
                    lineResult.Add(reelObjects[i].ReelData.GetReelSymbol(lineData.PayoutLine[i]));
                }

                // デバッグ用
                string lineBuffer = "";
                foreach(byte b in lineData.PayoutLine)
                {
                    lineBuffer += b.ToString();
                }

                string resultBuffer = "";
                foreach (ReelSymbols symbol in lineResult)
                {
                    resultBuffer += symbol.ToString();
                }
                Debug.Log(lineBuffer + "," + resultBuffer);

                // 図柄構成リストと見比べて該当するものがあれば当選。払い出し、ボーナス、リプレイ処理もする。
                // ボーナスは非当選でもストックされる

                // デバッグ用

                int foundIndex = CheckPayoutLines(lineResult, normalPayoutDatas);

                // データを追加(払い出しだけ当たった分追加する)
                // 当たったデータがあれば記録(-1以外)
                if(foundIndex != -1)
                {
                    finalPayouts += normalPayoutDatas[foundIndex].Payouts;
                    bonusID = normalPayoutDatas[foundIndex].BonusType;
                    replayStatus = normalPayoutDatas[foundIndex].IsReplayAndJac;
                }
            }
        }
        // 最終的な払い出し枚数をイベントに送る

        Debug.Log("payout:" + finalPayouts);
        Debug.Log("Bonus:" + bonusID);
        Debug.Log("IsReplay:" + replayStatus);

        return new ReelManager.PayoutResultBuffer(finalPayouts);
    }

    // 図柄の判定(配列を返す)
    private int CheckPayoutLines(List<ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
    {
        // 全て同じ図柄が揃っていたらHITを返す
        // ANY(10番)は無視

        for (int i = 0; i < payoutResult.Count; i++)
        {
            int sameSymbolCount = 0;
            for (int j = 0; j < payoutResult[i].Combinations.Length; j++)
            {
                // 図柄が合っているかチェック(ANYなら次の図柄へ)
                if (payoutResult[i].Combinations[j] == PayoutResultData.AnySymbol ||
                    (byte)lineResult[j] == payoutResult[i].Combinations[j])
                {
                    sameSymbolCount += 1;
                }
            }

            Debug.Log(sameSymbolCount);

            if(sameSymbolCount == ReelManager.ReelAmounts)
            {
                Debug.Log("HIT!:" + normalPayoutDatas[i].Payouts + "Bonus:"
                 + normalPayoutDatas[i].BonusType + "Replay:" + normalPayoutDatas[i].IsReplayAndJac);

                // 配列番号を送る

                return i;
            }
        }
        return -1;
    }
}
