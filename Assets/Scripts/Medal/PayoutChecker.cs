using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class PayoutChecker
{
    // 図柄の判定用

    // const
    public enum PayoutCheckMode { PayoutNormal, PayoutBIG, PayoutJAC};

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
        public PayoutLineData(byte betCondition, sbyte[] lines)
        {
            this.BetCondition = betCondition;
            this.PayoutLine = lines;
        }
    }

    // 払い出し結果のデータ
    class PayoutResultData
    {
        // const

        // バッファからデータを読み込む位置
        public enum ReadPos { FlagID = 0, CombinationsStart = 1, Payout = 4, Bonus, IsReplay}

        // ANYの判定用ID
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
        public bool hasReplayOrJAC { get; private set; }


        // コンストラクタ
        public PayoutResultData(byte flagID, byte[] combinations, byte payout,
            byte bonusType, bool hasReplayOrJAC)
        {
            this.FlagID = flagID;
            this.Combinations = combinations;   
            this.Payouts = payout;
            this.BonusType = bonusType;
            this.hasReplayOrJAC = hasReplayOrJAC;
        }
    }

    // 払い出し結果
    public class PayoutResultBuffer
    {
        public int Payouts { get; private set; }
        public int BonusID { get; private set; }
        public bool IsReplayOrJAC { get; private set; }

        public PayoutResultBuffer(int payouts, int bonusID, bool isReplayOrJac)
        {
            Payouts = payouts;
            BonusID = bonusID;
            IsReplayOrJAC = isReplayOrJac;
        }

        public void SetPayout(int payouts) => Payouts = payouts;
        public void SetBonusID(int bonusID) => BonusID = bonusID;
        public void SetReplayStatus(bool isReplayOrJac) => IsReplayOrJAC = isReplayOrJac;
    }

    // 最後に当たった結果
    public PayoutResultBuffer LastPayoutResult { get; private set; }

    // 各払い出しラインのデータ
    private List<PayoutLineData> payoutLineDatas;

    // 各種払い出し構成のテーブル
    // 通常時
    private List<PayoutResultData> normalPayoutDatas;
    // 小役ゲーム中
    private List<PayoutResultData> bigPayoutDatas;
    // JACゲーム中
    private List<PayoutResultData> jacPayoutDatas;
    // 選択中のテーブル
    public PayoutCheckMode CheckMode { get; private set; }

    // コンストラクタ
    public PayoutChecker(string normalPayoutPath, string bigPayoutPath, string jacPayoutPath,
        string payoutLineDataPath, PayoutCheckMode payoutMode)
    {
        StreamReader normalPayout = new StreamReader(normalPayoutPath);
        StreamReader bigPayout = new StreamReader(bigPayoutPath);
        StreamReader jacPayout = new StreamReader(jacPayoutPath);
        StreamReader payoutLine = new StreamReader(payoutLineDataPath);

        // 判定モード読み込み
        CheckMode = payoutMode;

        // 払い出し構成作成
        normalPayoutDatas = new List<PayoutResultData>();
        bigPayoutDatas = new List<PayoutResultData>();
        jacPayoutDatas = new List<PayoutResultData>();

        // 最後に判定した時の結果
        LastPayoutResult = new PayoutResultBuffer(0, 0, false);

        // データ読み込み
        // 通常時
        while (!normalPayout.EndOfStream)
        {
            normalPayoutDatas.Add(LoadPayoutResult(normalPayout));
        }

        // BIG小役ゲーム中
        while (!bigPayout.EndOfStream)
        {
            bigPayoutDatas.Add(LoadPayoutResult(bigPayout));
        }

        // JACゲーム中
        while (!jacPayout.EndOfStream)
        {
            jacPayoutDatas.Add(LoadPayoutResult(jacPayout));
        }

        Debug.Log("JacPayoutData loaded");

        // 払い出しラインの読み込み
        payoutLineDatas = new List<PayoutLineData>();

        // データ読み込み
        while(!payoutLine.EndOfStream)
        {
            payoutLineDatas.Add(LoadPayoutLines(payoutLine));
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

    //判定モード変更
    public void ChangePayoutCheckMode(PayoutCheckMode checkMode) => this.CheckMode = checkMode;

    // ライン判定
    public void CheckPayoutLines(int betAmount, List<List<ReelData.ReelSymbols>> lastSymbols)
    {
        // 最終的な払い出し結果
        int finalPayouts = 0;
        int bonusID = 0;
        bool replayStatus = false;

        // すること
        // 1:指定された払い出しラインごとにデータを格納する(ベット条件を満たしている必要がある)
        // 2:配当表の図柄が揃っているかを確認する
        // 3:揃っていれば払い出し、ボーナス、またはリプレイの状態を変化させる

        // 指定したラインごとにデータを得る
        foreach (PayoutLineData lineData in payoutLineDatas)
        {
            // 指定ラインの結果を保管
            List<ReelSymbols> lineResult = new List<ReelSymbols>();

            // ベット枚数の条件を満たしているかチェック
            if (betAmount >= lineData.BetCondition)
            {
                // 各リールから指定ラインの位置を得る(枠下2段目を基準に)
                int reelIndex = 0;
                foreach (List<ReelSymbols> reelResult in lastSymbols)
                {
                    // マイナス数値を配列番号に変換
                    int lineIndex = lineData.PayoutLine[reelIndex] + (int)ReelPosID.Lower3rd * -1;

                    Debug.Log("Symbol:" + reelResult[lineIndex]);
                   lineResult.Add(reelResult[lineIndex]);
                    reelIndex += 1;
                }

                // 図柄構成リストと見比べて該当するものがあれば当選。払い出し、ボーナス、リプレイ処理もする。
                // ボーナスは非当選でもストックされる
                int foundIndex = CheckPayoutLines(lineResult, GetPayoutResultData(CheckMode));

                // データを追加(払い出しだけ当たった分追加する)
                // 当たったデータがあれば記録(-1以外)
                if (foundIndex != -1)
                {
                    // 払い出しは常にカウント(15枚を超えても切り捨てられる)
                    finalPayouts += GetPayoutResultData(CheckMode)[foundIndex].Payouts;

                    // ボーナス未成立なら当たった時に変更
                    if (bonusID == 0)
                    {
                        bonusID = GetPayoutResultData(CheckMode)[foundIndex].BonusType;
                    }
                    // リプレイでなければ当たった時に変更
                    if (replayStatus == false)
                    {
                        replayStatus = GetPayoutResultData(CheckMode)[foundIndex].hasReplayOrJAC;
                    }
                }
            }
            
            // 条件を満たさない場合は終了
            else
            {
                break;
            }
        }
        // 最終的な払い出し枚数をイベントに送る

        Debug.Log("payout:" + finalPayouts);
        Debug.Log("Bonus:" + bonusID);
        Debug.Log("IsReplay:" + replayStatus);

        LastPayoutResult.SetPayout(finalPayouts);
        LastPayoutResult.SetBonusID(bonusID);
        LastPayoutResult.SetReplayStatus(replayStatus);
    }

    // 払い出しラインのデータ読み込み
    private PayoutLineData LoadPayoutLines(StreamReader streamLines)
    {
        // ストリームからデータを得る
        sbyte[] byteBuffer = Array.ConvertAll(streamLines.ReadLine().Split(','), sbyte.Parse);
        // 払い出しラインのデータ
        sbyte[] lineData = new sbyte[ReelManager.ReelAmounts];
        // デバッグ用
        string combinationBuffer = "";

        // 読み込み
        for (int i = 0; i < ReelManager.ReelAmounts; i++)
        {
            lineData[i] = byteBuffer[i + (int)PayoutLineData.ReadPos.PayoutLineStart];
            combinationBuffer += lineData[i];
        }

        PayoutLineData finalResult = new PayoutLineData((byte)byteBuffer[(int)PayoutLineData.ReadPos.BetCondition], lineData);

        //デバッグ用
        Debug.Log("Condition:" + finalResult.BetCondition + "Lines" + combinationBuffer);

        return finalResult;
    }

    // 払い出し結果のデータ読み込み
    private PayoutResultData LoadPayoutResult(StreamReader streamPayout)
    {
        // ストリームからデータを得る
        byte[] byteBuffer = Array.ConvertAll(streamPayout.ReadLine().Split(','), byte.Parse);
        // 図柄組み合わせのデータ読み込み(Payoutの位置まで読み込む)
        byte[] combinations = new byte[ReelManager.ReelAmounts];
        // デバッグ用
        string combinationBuffer = "";

        // 読み込み
        for (int i = 0; i < ReelManager.ReelAmounts; i++)
        {
            combinations[i] = byteBuffer[i + (int)PayoutResultData.ReadPos.CombinationsStart];
            combinationBuffer += combinations[i];
        }

        // データ作成
        PayoutResultData finalResult =  new PayoutResultData(byteBuffer[(int)PayoutResultData.ReadPos.FlagID], combinations,
            byteBuffer[(int)PayoutResultData.ReadPos.Payout], byteBuffer[(int)PayoutResultData.ReadPos.Bonus],
            byteBuffer[(int)PayoutResultData.ReadPos.IsReplay] == 1);


        //デバッグ用
        Debug.Log("Combination:" + combinationBuffer + "Payouts:" + finalResult.Payouts +
            "Bonus:" + finalResult.BonusType + "HasReplay:" + finalResult.hasReplayOrJAC);

        return finalResult;
    }

    // 図柄の判定(配列を返す)
    private int CheckPayoutLines(List<ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
    {
        // 全て同じ図柄が揃っていたらHITを返す
        // ANY(10番)は無視

        // 見つかったデータの位置
        int indexNum = 0;
        
        // 払い出し結果の判定
        foreach (PayoutResultData data in payoutResult)
        {
            // 同じ図柄をカウントする
            int sameSymbolCount = 0;

            // 図柄のチェック
            for (int i = 0; i < data.Combinations.Length; i++)
            {
                // 図柄が合っているかチェック(ANYなら次の図柄へ)
                if (data.Combinations[i] == PayoutResultData.AnySymbol ||
                    (byte)lineResult[i] == data.Combinations[i])
                {
                    sameSymbolCount += 1;
                }
            }

            Debug.Log(sameSymbolCount);

            // 同じ図柄(ANY含め)がリールの数と合えば当選とみなす
            if(sameSymbolCount == ReelManager.ReelAmounts)
            {
                Debug.Log("HIT!:" + payoutResult[indexNum].Payouts + "Bonus:"
                 + payoutResult[indexNum].BonusType + "Replay:" + payoutResult[indexNum].hasReplayOrJAC);

                // 配列番号を送る
                return indexNum;
            }
            // なかった場合は次の番号へ
            indexNum += 1;
        }
        // 見つからない場合は-1を返す(はずれとなる)
        return -1;
    }

    private List<PayoutResultData> GetPayoutResultData(PayoutCheckMode payoutCheckMode)
    {
        Debug.Log(payoutCheckMode.ToString());
        switch(payoutCheckMode)
        {
            case PayoutCheckMode.PayoutNormal:
                return normalPayoutDatas;

            case PayoutCheckMode.PayoutBIG:
                return bigPayoutDatas;

            case PayoutCheckMode.PayoutJAC:
                return jacPayoutDatas;
        }
        return null;
    }
}
