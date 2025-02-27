using ReelSpinGame_Reels;
using ReelSpinGame_Rules;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReelTableManager
{
    // リールテーブル管理用

    // const

    // リール条件データ
    public class ReelConditionsData
    {
        // const
        // 条件を読み込むバイト数
        public const int ConditionMaxRead = 5;
        // 第一停止リール停止位置を読み込むバイト数
        public const int FirstReelPosMaxRead = ConditionMaxRead + 10;
        // テーブルIDの読み込むバイト数
        public const int ReelTableIDMaxRead = FirstReelPosMaxRead + 1;
        // 条件を読み込む際にずらすビット数
        public const int ConditionBitOffset = 4;

        // enum
        // 条件のシリアライズ
        public enum Conditions { C_FLAG, C_FIRST, C_BONUS, C_BET, C_RANDOM }

        // var
        // テーブル使用条件はint型データで格納し、1バイト(8bit)ごとにデータを分ける
        // フラグID, 第一停止, ボーナス, ベット枚数, ランダム制御の順で読み込む
        public int MainConditions { get; private set; }

        // 第一停止したリールの位置(これをもとにテーブルの変更をする)
        public int FirstReelPosition { get; private set; }

        // 使用するテーブル番号
        public byte ReelTableNumber { get; private set; }

        // コンストラクタ
        public ReelConditionsData(StreamReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            Debug.Log(values.Length);

            // 読み込み開始(要素番号をもとにデータを読み込む
            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // 第一リール停止
                else if (indexNum < FirstReelPosMaxRead)
                { 
                    FirstReelPosition += OriginalMathmatics.ConvertToArrayBit(Convert.ToInt32(value));
                }

                // テーブルID読み込み
                else if (indexNum < ReelTableIDMaxRead)
                {
                    ReelTableNumber = Convert.ToByte(value); 
                }

                // 最後の一行は読まない(テーブル名)
                else
                {
                    break; 
                }
                indexNum++;
            }

            Debug.Log("Condition:" + MainConditions + "FirstReel:" + FirstReelPosition + "ReelTableNum" + ReelTableNumber);
        }
    }

    // リール制御テーブル
    public class ReelTableData
    {
        // リール制御テーブル(ディレイを格納する)
        public byte[] TableData { get; private set; } = new byte[ReelData.MaxReelArray];

        public ReelTableData(StreamReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // デバッグ用
            string debugBuffer = "";

            // 読み込み開始
            foreach (string value in values)
            {
                // リールデータを読み込む
                if (indexNum < ReelData.MaxReelArray) 
                { 
                    TableData[indexNum] = Convert.ToByte(value);
                    debugBuffer += TableData[indexNum];
                }

                // 最後の一行は読まない(テーブル名)
                else 
                { 
                    break; 
                }
                indexNum++;
            }

            Debug.Log("Array:" + debugBuffer);
        }
    }

    // var
    public List<ReelConditionsData> ReelConditionL { get; private set; }
    public List<ReelConditionsData> ReelConditionM { get; private set; }
    public List<ReelConditionsData> ReelConditionR { get; private set; }

    public List<ReelTableData> ReelTableL { get; private set; }
    public List<ReelTableData> ReelTableM { get; private set; }
    public List<ReelTableData> ReelTableR { get; private set; }

    // コンストラクタ
    public ReelTableManager(StreamReader conditionL, StreamReader conditionM, StreamReader conditionR,
        StreamReader reelTableL, StreamReader reelTableM, StreamReader reelTableR)
    {
        // リスト作成
        ReelConditionL = new List<ReelConditionsData>();
        ReelConditionM = new List<ReelConditionsData>();
        ReelConditionR = new List<ReelConditionsData>();

        ReelTableL = new List<ReelTableData>();
        ReelTableM = new List<ReelTableData>();
        ReelTableR = new List<ReelTableData>();

        // 条件読み込み
        while (!conditionL.EndOfStream)
        {
            ReelConditionL.Add(new ReelConditionsData(conditionL));
        }

        Debug.Log("ReelConditions reading done");

        // テーブル読み込み
        while (!reelTableL.EndOfStream)
        {
            ReelTableL.Add(new ReelTableData(reelTableL));
        }

        Debug.Log("ReelTable reading done");
    }
}
