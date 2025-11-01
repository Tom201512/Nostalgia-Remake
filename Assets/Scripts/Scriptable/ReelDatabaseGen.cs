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
            // �z��ǂݍ���
            string[] values = arrayFile.ReadLine().Split(',');
            // �z��ɕϊ�
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
                    ConditionDebug += GetConditionData(condition.MainConditions, i).ToString() + ",";
                }

                //Debug.Log("Condition:" + condition.MainConditions + "Details:" + ConditionDebug + "FirstReel:" + condition.FirstReelPosition + "ReelTableNum" + condition.ReelTableNumber);
            }

            return finalResult;
        }

        public static List<ReelFirstConditions> MakeReelFirstData(StreamReader reelFirstDataFile)
        {
            List<ReelFirstConditions> finalResult = new List<ReelFirstConditions>();

            // �S�Ă̍s��ǂݍ���
            while (reelFirstDataFile.Peek() != -1)
            {
                finalResult.Add(new ReelFirstConditions(reelFirstDataFile));
            }

            return finalResult;
        }

        public static List<ReelSecondConditions> MakeReelSecondData(StreamReader reelSecondFile)
        {
            List<ReelSecondConditions> finalResult = new List<ReelSecondConditions>();

            // �S�Ă̍s��ǂݍ���
            while (reelSecondFile.Peek() != -1)
            {
                finalResult.Add(new ReelSecondConditions(reelSecondFile));
            }

            return finalResult;
        }

        public static List<ReelThirdConditions> MakeReelThirdData(StreamReader conditionsFile)
        {
            List<ReelThirdConditions> finalResult = new List<ReelThirdConditions>();

            // �S�Ă̍s��ǂݍ���
            while (conditionsFile.Peek() != -1)
            {
                finalResult.Add(new ReelThirdConditions(conditionsFile));
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
