using ReelSpinGame_Reel;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reel.ReelManager;

namespace ReelSpinGame_Scriptable
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

        [SerializeField] private int betCondition;                  // 有効に必要なベット枚数
        [SerializeField] private List<ReelPosID> payoutLines;       // 払い出しライン

        public int BetCondition { get => betCondition; }
        public List<ReelPosID> PayoutLines { get => payoutLines; }

        public PayoutLineData(StreamReader loadedData)
        {
            // ストリームからデータを得る
            int[] dataBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), int.Parse);
            payoutLines = new List<ReelPosID>();

            betCondition = dataBuffer[(int)ReadPos.BetCondition];
            // 読み込み
            for (int i = 0; i < ReelAmount; i++)
            {
                ReelPosID posID = (ReelPosID)Enum.ToObject(typeof(ReelPosID), dataBuffer[i + (int)ReadPos.PayoutLineStart]);
                payoutLines.Add(posID);
            }
        }
    }

    // 払い出し結果のデータ
    [Serializable]
    public class PayoutResultData
    {
        // バッファからデータを読み込む位置
        public enum ReadPos 
        { 
            CombinationStart = 0, 
            Payout = 3, 
            Bonus, 
            IsReplay, 
        }

        public const int AnySymbol = 10;     // ANYの判定用ID

        [SerializeField] int flagID;               // フラグID
        [SerializeField] List<int> combination;    // 組み合わせ
        [SerializeField] int payout;               // 払い出し枚数
        [SerializeField] int bonusType;            // 当選するボーナス
        [SerializeField] bool hasReplayOrJac;       // リプレイか(またはJAC-IN)

        // プロパティ
        public List<int> Combination { get => combination; }
        public int Payout { get => payout; }
        public int BonusType { get => bonusType; }
        public bool HasReplayOrJac { get => hasReplayOrJac; }

        public PayoutResultData(StreamReader loadedData)
        {
            // ストリームからデータを得る
            int[] dataBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), int.Parse);
            // 図柄組み合わせのデータ読み込み(Payoutの位置まで読み込む)
            combination = new List<int>();

            // データ作成
            for (int i = 0; i < ReelAmount; i++)
            {
                combination.Add(dataBuffer[i + (int)ReadPos.CombinationStart]);
            }

            payout = dataBuffer[(int)ReadPos.Payout];
            bonusType = dataBuffer[(int)ReadPos.Bonus];
            hasReplayOrJac = dataBuffer[(int)ReadPos.IsReplay] == 1;
        }
    }
}
