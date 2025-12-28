using ReelSpinGame_Datas.Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Datas
{
    // リールのスベリコマデータ
    public class ReelDelayTableData : ScriptableObject
    {
        // リール条件

        [SerializeField] private List<ReelFirstConditions> first;       // 第一停止
        [SerializeField] private List<ReelSecondConditions> second;     // 第二停止
        [SerializeField] private List<ReelThirdConditions> third;       // 第三停止
        [SerializeField] private List<ReelTableData> tables;            // スベリコマテーブル

        public List<ReelFirstConditions> FirstCondition { get { return first; } }       // 第一停止条件のリスト
        public List<ReelSecondConditions> SecondCondition { get { return second; } }    // 第二停止条件のリスト
        public List<ReelThirdConditions> ThirdCondition { get { return third; } }       // 第三停止条件のリスト
        public List<ReelTableData> Tables { get { return tables; } }                    // スベリコマテーブルのリスト

        // リール条件セット
        public void SetReelConditions(List<ReelFirstConditions> first, List<ReelSecondConditions> second, List<ReelThirdConditions> third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        // リールテーブルのセット
        public void SetTables(List<ReelTableData> tables) => this.tables = tables;
    }

    // リール条件テーブル
    [Serializable]
    public class ReelConditionsData
    {
        public const int ConditionMaxRead = 5;                              // 条件を読み込むバイト数
        public const int FirstReelPosMaxRead = ConditionMaxRead + 10;       // 第一停止リール停止位置を読み込むバイト数
        public const int ReelTableIDMaxRead = FirstReelPosMaxRead + 1;      // テーブルIDの読み込むバイト数
        public const int ConditionBitOffset = 4;                            // 条件を読み込む際にずらすビット数

        // 条件のシリアライズ
        public enum ConditionID { Flag, FirstPush, Bet, Bonus, Random }

        [SerializeField] private int mainConditions;        // メイン条件数値
        [SerializeField] private int firstReelPosition;     // 第一停止したリールの位置
        [SerializeField] byte reelTableNumber;              // 使用するテーブル番号

        public int MainConditions { get { return mainConditions; } set { mainConditions = value; } }
        public int FirstReelPosition { get { return firstReelPosition; } set { firstReelPosition = value; } }
        public byte ReelTableNumber { get { return reelTableNumber; } set { reelTableNumber = value; } }

        // コンストラクタ
        public ReelConditionsData(StringReader buffer)
        {
            string[] values = buffer.ReadLine().Split(',');

            int indexNum = 0;
            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    mainConditions += Convert.ToInt32(value) * offset;
                }

                // 第一リール停止
                else if (indexNum < FirstReelPosMaxRead)
                {
                    firstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
                }

                // テーブルID読み込み
                else if (indexNum < ReelTableIDMaxRead)
                {
                    reelTableNumber = Convert.ToByte(value);
                }

                // 最後の部分は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum += 1;
            }
        }

        public ReelConditionsData(StreamReader buffer)
        {
            string[] values = buffer.ReadLine().Split(',');

            int indexNum = 0;
            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    mainConditions += Convert.ToInt32(value) * offset;
                }

                // 第一リール停止
                else if (indexNum < FirstReelPosMaxRead)
                {
                    //Debug.Log(value);
                    firstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
                }

                // テーブルID読み込み
                else if (indexNum < ReelTableIDMaxRead)
                {
                    reelTableNumber = Convert.ToByte(value);
                }
                indexNum += 1;
            }
        }

        // 各条件の数値を返す
        public static int GetConditionData(int condition, int conditionID) => ((condition >> ConditionBitOffset * conditionID) & 0xF);

        // 各条件をintにする
        public static int ConvertConditionData(int flagID, int firstPush, int bet, int bonus, int random)
        {
            // 16進数のデータへ変更
            int conditions = 0;
            // 配列にする
            int[] conditionArray = { flagID, firstPush, bet, bonus, random };

            for (int i = 0; i < conditionArray.Length; i++)
            {
                conditions |= conditionArray[i] << ConditionBitOffset * i;
            }
            return conditions;
        }

        // データをビットにする
        public static int ConvertToArrayBit(int data)
        {
            // 0の時は変換しない
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }
    }

    // リール制御テーブル
    [Serializable]
    public class ReelTableData
    {
        // var
        // リール制御テーブル(ディレイを格納する)
        [SerializeField] private List<byte> tableData;
        public List<byte> TableData { get { return tableData; } }

        // コンストラクタ
        public ReelTableData(StringReader LoadedData)
        {
            tableData = new List<byte>();

            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;
            // デバッグ用
            string debugBuffer = "";

            // 読み込み開始
            foreach (string value in values)
            {
                //Debug.Log(value);
                // リールデータを読み込む
                if (indexNum < MaxReelArray)
                {
                    tableData.Add(Convert.ToByte(value));
                    debugBuffer += tableData[indexNum];
                }
                indexNum++;
            }

            //Debug.Log("Array:" + debugBuffer);
        }

        // コンストラクタ
        public ReelTableData(StreamReader LoadedData)
        {
            tableData = new List<byte>();

            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;
            // デバッグ用
            string debugBuffer = "";

            // 読み込み開始
            foreach (string value in values)
            {
                //Debug.Log(value);
                // リールデータを読み込む
                if (indexNum < MaxReelArray)
                {
                    tableData.Add(Convert.ToByte(value));
                    debugBuffer += tableData[indexNum];
                }

                // 最後の一行は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum++;
            }
        }
    }
}
