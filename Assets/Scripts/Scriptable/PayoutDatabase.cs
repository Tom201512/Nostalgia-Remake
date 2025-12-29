using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelLogicManager;

namespace ReelSpinGame_Datas
{
    // 払い出しデータベース
    public class PayoutDatabase : ScriptableObject
    {
        [SerializeField] List<PayoutLineData> payoutLineDatas;        // 各払い出しラインのデータ

        // 各種払い出し構成のテーブル

        [SerializeField] List<PayoutResultData> normalPayoutDatas;      // 通常時
        [SerializeField] List<PayoutResultData> bigPayoutDatas;         // 小役ゲーム中
        [SerializeField] List<PayoutResultData> jacPayoutDatas;         // JACゲーム中

        // プロパティ
        public List<PayoutLineData> PayoutLines
        {
            get => payoutLineDatas;
            set => payoutLineDatas = value;
        }
        public List<PayoutResultData> NormalPayoutDatas
        {
            get => normalPayoutDatas;
            set => normalPayoutDatas = value;
        }
        public List<PayoutResultData> BigPayoutDatas
        {
            get => bigPayoutDatas;
            set => bigPayoutDatas = value;
        }
        public List<PayoutResultData> JacPayoutDatas
        {
            get => jacPayoutDatas;
            set => jacPayoutDatas = value;
        }
    }

    // 払い出しラインのデータ
    [Serializable]
    public class PayoutLineData
    {
        // バッファからデータを読み込む位置
        public enum ReadPos { BetCondition = 0, PayoutLineStart }

        [SerializeField] private byte betCondition;             // 有効に必要なベット枚数
        [SerializeField] private List<sbyte> payoutLines;       // 払い出しライン(符号付きbyte)

        public byte BetCondition { get => betCondition; }
        public List<sbyte> PayoutLines { get => payoutLines; }

        public PayoutLineData(StreamReader loadedData)
        {
            // ストリームからデータを得る
            sbyte[] byteBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), sbyte.Parse);
            payoutLines = new List<sbyte>();

            betCondition = (byte)byteBuffer[(int)ReadPos.BetCondition];
            // 読み込み
            for (int i = 0; i < ReelAmount; i++)
            {
                payoutLines.Add(byteBuffer[i + (int)ReadPos.PayoutLineStart]);
            }
        }
    }

    // 払い出し結果のデータ
    [Serializable]
    public class PayoutResultData
    {
        // バッファからデータを読み込む位置
        public enum ReadPos { FlagID = 0, CombinationStart = 1, Payout = 4, Bonus, IsReplay }

        public const int AnySymbol = 7;     // ANYの判定用ID

        [SerializeField] byte flagID;               // フラグID
        [SerializeField] List<byte> combination;    // 組み合わせ
        [SerializeField] byte payout;               // 払い出し枚数
        [SerializeField] byte bonusType;            // 当選するボーナス
        [SerializeField] bool hasReplayOrJac;       // リプレイか(またはJAC-IN)

        // プロパティ
        public byte FlagID { get => flagID; }
        public List<byte> Combination { get => combination; }
        public byte Payout { get => payout; }
        public byte BonusType { get => bonusType; }
        public bool HasReplayOrJac { get => hasReplayOrJac; }

        public PayoutResultData(StreamReader loadedData)
        {
            // ストリームからデータを得る
            byte[] byteBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), byte.Parse);
            // 図柄組み合わせのデータ読み込み(Payoutの位置まで読み込む)
            combination = new List<byte>();

            // データ作成
            flagID = byteBuffer[(int)ReadPos.FlagID];

            for (int i = 0; i < ReelAmount; i++)
            {
                combination.Add(byteBuffer[i + (int)ReadPos.CombinationStart]);
            }

            payout = byteBuffer[(int)ReadPos.Payout];
            bonusType = byteBuffer[(int)ReadPos.Bonus];
            hasReplayOrJac = byteBuffer[(int)ReadPos.IsReplay] == 1;
        }
    }
}
