using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // 払い出しデータベース
    public class PayoutDatabase : ScriptableObject
    {
        // var
        // 各払い出しラインのデータ
        [SerializeField]private List<PayoutLineData> payoutLineDatas;

        // 各種払い出し構成のテーブル
        // 通常時
        [SerializeField] private List<PayoutResultData> normalPayoutDatas;
        // 小役ゲーム中
        [SerializeField] private List<PayoutResultData> bigPayoutDatas;
        // JACゲーム中
        [SerializeField] private List<PayoutResultData> jacPayoutDatas;

        public List<PayoutLineData> PayoutLines { get { return payoutLineDatas; }}
        public List<PayoutResultData> NormalPayoutDatas { get { return normalPayoutDatas; } }
        public List<PayoutResultData> BigPayoutDatas { get { return bigPayoutDatas; }  }
        public List<PayoutResultData> JacPayoutDatas { get { return jacPayoutDatas; } }

        // func
        public void SetPayoutLines(List<PayoutLineData> payoutLines) => payoutLineDatas = payoutLines;
        public void SetNormalPayout(List<PayoutResultData> normalPayoutDatas) => this.normalPayoutDatas = normalPayoutDatas;
        public void SetBigPayout(List<PayoutResultData> bigPayoutDatas) => this.bigPayoutDatas = bigPayoutDatas;
        public void SetJacPayout(List<PayoutResultData> jacPayoutDatas) => this.jacPayoutDatas = jacPayoutDatas;
    }

    // 払い出しラインのデータ
    [Serializable]
    public class PayoutLineData
    {
        // const
        // バッファからデータを読み込む位置
        public enum ReadPos { BetCondition = 0, PayoutLineStart }

        // 有効に必要なベット枚数
        [SerializeField] private byte betCondition;
        // 払い出しライン(符号付きbyte)
        [SerializeField] private List<sbyte> payoutLines;

        public byte BetCondition { get { return betCondition; } }
        public List<sbyte> PayoutLines { get { return payoutLines; }}

        public PayoutLineData(StringReader loadedData)
        {
            // ストリームからデータを得る
            sbyte[] byteBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), sbyte.Parse);
            // 払い出しラインのデータ
            payoutLines = new List<sbyte>();
            // デバッグ用
            string combinationBuffer = "";
            Debug.Log(byteBuffer[0]);

            betCondition = (byte)byteBuffer[(int)ReadPos.BetCondition];
            // 読み込み
            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                payoutLines.Add(byteBuffer[i + (int)ReadPos.PayoutLineStart]);
                combinationBuffer += payoutLines[i];
            }

            //デバッグ用
            Debug.Log("Condition:" + byteBuffer[(int)ReadPos.BetCondition] + "Lines" + combinationBuffer);
        }
    }

    // 払い出し結果のデータ
    [Serializable]
    public class PayoutResultData
    {
        // const
        // バッファからデータを読み込む位置
        public enum ReadPos { FlagID = 0, CombinationsStart = 1, Payout = 4, Bonus, IsReplay }
        // ANYの判定用ID
        public const int AnySymbol = 7;

        // var
        // フラグID
        [SerializeField] private byte flagID;
        // 図柄構成
        [SerializeField] private List<byte> combinations;
        // 払い出し枚数
        [SerializeField] private byte payouts;
        // 当選するボーナス
        [SerializeField] private byte bonusType;
        // リプレイか(またはJAC-IN)
        [SerializeField] private bool hasReplayOrJac;

        public byte FlagID { get { return flagID; }}
        public List<byte> Combinations { get { return combinations; } }
        public byte Payouts { get { return payouts; } }
        public byte BonusType { get { return bonusType; } }
        public bool HasReplayOrJac { get { return hasReplayOrJac; } }

        public PayoutResultData(StringReader loadedData)
        {
            // ストリームからデータを得る
            byte[] byteBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), byte.Parse);
            // 図柄組み合わせのデータ読み込み(Payoutの位置まで読み込む)
            byte[] combinations = new byte[ReelManager.ReelAmounts];
            // デバッグ用
            string combinationBuffer = "";

            // データ作成
            // フラグID
            flagID = byteBuffer[(int)PayoutResultData.ReadPos.FlagID];

            // 組み合わせ
            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                combinations[i] = byteBuffer[i + (int)ReadPos.CombinationsStart];
                combinationBuffer += combinations[i];
            }
            // 払い出し
            payouts = byteBuffer[(int)ReadPos.Payout];
            // ボーナス
            bonusType = byteBuffer[(int)ReadPos.Bonus];
            // リプレイの有無
            hasReplayOrJac = byteBuffer[(int)PayoutResultData.ReadPos.IsReplay] == 1;

            //デバッグ用
            Debug.Log("Flag:" + flagID + "Combination:" + combinationBuffer + "Payouts:" + payouts +
                "Bonus:" + bonusType + "HasReplay:" + hasReplayOrJac);
        }
    }
}
