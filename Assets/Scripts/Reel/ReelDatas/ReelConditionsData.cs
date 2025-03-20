using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Reels.Conditions
{
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
                    FirstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
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

            for (int i = 0; i < 5; i++)
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

        // データをビットにする
        private int ConvertToArrayBit(int data)
        {
            //Do not convert if data is 0
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }
    }
}