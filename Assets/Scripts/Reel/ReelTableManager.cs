using ReelSpinGame_Reels;
using ReelSpinGame_Rules;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static ReelManager;
using static ReelSpinGame_Lots.Flag.FlagLots;

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
        public enum ConditionID { Flag, FirstPush, Bonus, Bet, Random }

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

            // デバッグ用
            string ConditionDebug = "";

            for(int i = 0; i < 5; i++)
            {
                ConditionDebug += GetConditionData(i).ToString() + ",";
            }
            Debug.Log("Condition:" + MainConditions + "Details:" + ConditionDebug + "FirstReel:" + FirstReelPosition + "ReelTableNum" + ReelTableNumber);
        }

        // func
        // 各条件をintにする
        public static int ConvertConditionData(int flagID, int firstPush, int bonus, int bet, int random)
        {
            // 16進数のデータへ変更
            int conditions = 0;
            // 配列にする
            int[] conditionArray = { flagID, firstPush, bonus, bet, random };

            for (int i = 0; i < (int)ConditionID.Random; i++)
            {
                conditions |= conditionArray[i] << ConditionBitOffset * i;
            }
            return conditions;
        }

        // 各条件の数値を返す
        public int GetConditionData(int conditionID) => ((MainConditions >> ConditionBitOffset * conditionID) & 0xF);
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
    private List<List<ReelConditionsData>> reelConditions;
    private List<List<ReelTableData>> reelDelayTables;

    // コンストラクタ
    public ReelTableManager(List<StreamReader> conditions, List<StreamReader> tables)
    {
        // リスト作成
        reelConditions = new List<List<ReelConditionsData>>();
        reelDelayTables = new List<List<ReelTableData>>();

        // リール条件とテーブルの数が合うかチェック
        if (conditions.Count != tables.Count)
        {
            throw new Exception("Condition counts and table counts doesn't match");
        }
        
        // 条件の読み込み
        for(int i = 0; i < conditions.Count; i++)
        {
            // 条件読み込み
            reelConditions.Add(new List<ReelConditionsData>());

            while (!conditions[i].EndOfStream)
            {
                reelConditions[i].Add(new ReelConditionsData(conditions[i]));
            }

            Debug.Log("Condition:" + i + "Read done" + reelConditions[i].Count);
        }

        Debug.Log("ReelConditions reading done");

        for (int i = 0; i < tables.Count; i++)
        {
            // 条件読み込み
            reelDelayTables.Add(new List<ReelTableData>());

            while (!tables[i].EndOfStream)
            {
                reelDelayTables[i].Add(new ReelTableData(tables[i]));
            }

            Debug.Log("DelayTable:" + i + "Read done" + reelDelayTables[i].Count);
        }

        Debug.Log("ReelTable reading done");
    }

    // func

    // 条件から使用するテーブル番号を探す
    public int FindTableToUse(ReelManager.ReelID reelID, int flagID, int firstPush, int bonus, int bet, int random, int firstPushPos)
    {
        int condition = ReelConditionsData.ConvertConditionData(flagID, firstPush, bonus, bet, random);
        int[] orderToCheck = { flagID, firstPush, bonus, bet, random };

        // 使用するテーブル配列の番号(-1はエラー)
        int foundTable = -1;
        // 検索中のテーブル
        int currentIndex = 0;

        foreach (ReelConditionsData data in reelConditions[(int)reelID])
        {
            Debug.Log("Search:" + currentIndex);

            // 条件が合っているか
            bool conditionMet = true;

            for (int i = 0; i < orderToCheck.Length; i++)
            {
                // フラグID以外の条件は0ならパス
                if (i != (int)ReelConditionsData.ConditionID.Flag && data.GetConditionData(i) == 0)
                {
                    continue;
                }
                else if (orderToCheck[i] != data.GetConditionData(i))
                {
                    conditionMet = false;
                }
            }

            // 条件が合っていれば
            if(conditionMet)
            {
                Debug.Log("All conditions are met");

                // 次は第一停止のリール停止位置を見る
                // 停止位置条件が0なら無視
                if (data.FirstReelPosition != 0 || ((byte)firstPushPos & data.FirstReelPosition) == 0)
                {
                    if (data.FirstReelPosition != 0)
                    {
                        Debug.Log("No condition");
                    }
                    // ここまできたらテーブル発見。すぐに更新する
                    Debug.Log("Found:" + currentIndex);
                    foundTable = data.ReelTableNumber;
                }
            }
            currentIndex += 1;
        }
        Debug.Log("Final Found:" + foundTable);

        return foundTable;
    }


    // 指定したリールのディレイ(スベリ)を返す
    public byte GetDelayFromTable(ReelManager.ReelID reelID, int pushedPos, int tableIndex)
    {
        Debug.Log("Delay:" + reelDelayTables[(int)reelID][tableIndex].TableData[pushedPos]);
        return reelDelayTables[(int)reelID][tableIndex].TableData[pushedPos];
    }
}
