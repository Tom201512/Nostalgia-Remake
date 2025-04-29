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
            // �z��ǂݍ���
            string[] values = arrayFile.ReadLine().Split(',');
            // �z��ɕϊ�
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

            // �S�Ă̍s��ǂݍ���
            while (conditionsFile.Peek() != -1)
            {
                finalResult.Add(new ReelConditionsData(conditionsFile));
            }

            foreach (ReelConditionsData condition in finalResult)
            {
                // �f�o�b�O�p
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

            // �S�Ă̍s��ǂݍ���
            while (tablesFile.Peek() != -1)
            {
                finalResult.Add(new ReelTableData(tablesFile));
            }

            return finalResult;
        }
    }
#endif
}
