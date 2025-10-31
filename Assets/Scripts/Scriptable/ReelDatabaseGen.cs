using ReelSpinGame_Datas.Reels;
using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Datas.ReelConditionsData;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

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
            byte[] result = Array.ConvertAll(values, byte.Parse);

            foreach (byte value in result)
            {
                //Debug.Log(value + "Symbol:" + ReelData.ReturnSymbol(value));
            }

            for (int i = 0; i < MaxReelArray; i++)
            {
                //Debug.Log("No." + i + " Symbol:" + ReelData.ReturnSymbol(result[i]));
            }

            return result;
        }

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

                //Debug.Log("Condition:" + condition.MainConditions + "Details:" + ConditionDebug + "FirstReel:" + condition.FirstReelPosition + "ReelTableNum" + condition.ReelTableNumber);
            }

            return finalResult;
        }

        public static List<ReelFirstData> MakeReelFirstData(StreamReader reelFirstDataFile)
        {
            List<ReelFirstData> finalResult = new List<ReelFirstData>();

            // 全ての行を読み込む
            while (reelFirstDataFile.Peek() != -1)
            {
                finalResult.Add(new ReelFirstData(reelFirstDataFile));
            }

            return finalResult;
        }

        public static List<ReelSecondData> MakeReelSecondData(StreamReader reelSecondFile)
        {
            List<ReelSecondData> finalResult = new List<ReelSecondData>();

            // 全ての行を読み込む
            while (reelSecondFile.Peek() != -1)
            {
                finalResult.Add(new ReelSecondData(reelSecondFile));
            }

            return finalResult;
        }

        public static List<ReelThirdData> MakeReelThirdData(StreamReader conditionsFile)
        {
            List<ReelThirdData> finalResult = new List<ReelThirdData>();

            // 全ての行を読み込む
            while (conditionsFile.Peek() != -1)
            {
                finalResult.Add(new ReelThirdData(conditionsFile));
            }

            return finalResult;
        }

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
