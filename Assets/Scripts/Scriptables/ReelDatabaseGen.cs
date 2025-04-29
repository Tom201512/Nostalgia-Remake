using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ReelSpinGame_Reels;

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
                Debug.Log(value + "Symbol:" + ReelData.ReturnSymbol(value));
            }

            for (int i = 0; i < ReelData.MaxReelArray; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReelData.ReturnSymbol(result[i]));
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
                    ConditionDebug += condition.GetConditionData(i).ToString() + ",";
                }

                Debug.Log("Condition:" + condition.MainConditions + "Details:" + ConditionDebug + "FirstReel:" + condition.FirstReelPosition + "ReelTableNum" + condition.ReelTableNumber);
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
