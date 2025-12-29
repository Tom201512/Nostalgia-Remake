using ReelSpinGame_Datas.Reels;
using System;
using System.Collections.Generic;
using System.IO;
using static ReelSpinGame_Datas.ReelConditionsData;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class ReelDatabaseGen
    {
        public static byte[] MakeReelArray(StreamReader arrayFile)
        {
            // 配列読み込み
            string[] values = arrayFile.ReadLine().Split(',');
            // 配列に変換
            return Array.ConvertAll(values, byte.Parse); ;
        }

        // リール条件読み込み
        public static List<ReelConditionsData> MakeReelConditions(StreamReader conditionsFile)
        {
            List<ReelConditionsData> finalResult = new List<ReelConditionsData>();

            // 全ての行を読み込む
            while (conditionsFile.Peek() != -1)
            {
                finalResult.Add(new ReelConditionsData(conditionsFile));
            }

            foreach (ReelConditionsData condition in finalResult)
            {
                // デバッグ用
                string ConditionDebug = "";

                for (int i = 0; i < 5; i++)
                {
                    ConditionDebug += GetConditionData(condition.MainConditions, i).ToString() + ",";
                }
            }

            return finalResult;
        }

        // 第一停止テーブル作成
        public static List<ReelFirstConditions> MakeReelFirstData(StreamReader reelFirstDataFile)
        {
            List<ReelFirstConditions> finalResult = new List<ReelFirstConditions>();

            // 全ての行を読み込む
            while (reelFirstDataFile.Peek() != -1)
            {
                finalResult.Add(new ReelFirstConditions(reelFirstDataFile));
            }

            return finalResult;
        }

        // 第二停止テーブル作成
        public static List<ReelSecondConditions> MakeReelSecondData(StreamReader reelSecondFile)
        {
            List<ReelSecondConditions> finalResult = new List<ReelSecondConditions>();

            // 全ての行を読み込む
            while (reelSecondFile.Peek() != -1)
            {
                finalResult.Add(new ReelSecondConditions(reelSecondFile));
            }

            return finalResult;
        }

        // 第三停止テーブル作成
        public static List<ReelThirdConditions> MakeReelThirdData(StreamReader conditionsFile)
        {
            List<ReelThirdConditions> finalResult = new List<ReelThirdConditions>();

            // 全ての行を読み込む
            while (conditionsFile.Peek() != -1)
            {
                finalResult.Add(new ReelThirdConditions(conditionsFile));
            }

            return finalResult;
        }

        // スベリコマテーブル作成
        public static List<ReelTableData> MakeTableDatas(StreamReader tablesFile)
        {
            List<ReelTableData> finalResult = new List<ReelTableData>();

            // 全ての行を読み込む
            while (tablesFile.Peek() != -1)
            {
                finalResult.Add(new ReelTableData(tablesFile));
            }

            return finalResult;
        }
    }
#endif
}
