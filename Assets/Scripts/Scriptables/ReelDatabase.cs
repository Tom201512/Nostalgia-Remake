using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // リールデータベース
    public class ReelDatabase : ScriptableObject
    {
        // var
        // リール配列
        [SerializeField] private byte[] array;
        // リール条件
        [SerializeField] private List<ReelConditionsData> conditions;
        // ディレイテーブル
        [SerializeField] private List<ReelTableData> tables;

        public byte[] Array { get { return array; } }
        public List<ReelConditionsData> Conditions { get { return conditions; }}
        public List<ReelTableData> Tables { get { return tables; } }

        // func
        public void SetArray(byte[] array) => this.array = array;
        public void SetConditions(List<ReelConditionsData> conditions) => this.conditions = conditions;
        public void SetTables(List<ReelTableData> tables) => this.tables = tables;
    }

    // リール条件テーブル
    [Serializable]
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
        // フラグID, 第一停止, ボーナス, ベット枚数, ランダム制御の順で読み込む
        [SerializeField] private int mainConditions;
        // 第一停止したリールの位置(これをもとにテーブルの変更をする)
        [SerializeField] private int firstReelPosition;
        // 使用するテーブル番号
        [SerializeField] byte reelTableNumber;

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

        // func
        // 各条件の数値を返す
        public int GetConditionData(int conditionID) => ((MainConditions >> ConditionBitOffset * conditionID) & 0xF);

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
                Debug.Log(value);
                // リールデータを読み込む
                if (indexNum < ReelData.MaxReelArray)
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

            Debug.Log("Array:" + debugBuffer);
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
                Debug.Log(value);
                // リールデータを読み込む
                if (indexNum < ReelData.MaxReelArray)
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

            Debug.Log("Array:" + debugBuffer);
        }
    }
}
