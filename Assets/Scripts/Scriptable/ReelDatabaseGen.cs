using ReelSpinGame_Datas.Reels;
using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using static UnityEngine.ScriptableObject;
using static ReelSpinGame_Datas.ReelConditionsData;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    // リールデータベース作成
    public class ReelDatabaseGen
    {
        private const string ReelFileStartPath = "Nostalgia ReelSheet - ";      // リールファイル先頭のファイル名

        [MenuItem("ScriptableGen/CreateReelDatabase")]
        // リールデータ作成
        public static void MakeReelDataAll()
        {
            string[] pathOrder = { "ReelL", "ReelM", "ReelR" };

            // あらかじめ設定したディレクトリから全リールのデータを読み込む
            for (int i = 0; i < ReelLogicManager.ReelAmount; i++)
            {
                MakeReelArrayData(pathOrder[i]);
                MakeReelDelayData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        // リール配列の作成
        private static void MakeReelArrayData(string reelName)
        {
            string path = "Assets/ReelData";
            string fileName = reelName + "Array";

            // ファイルが無ければ新規作成
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            ReelArrayData reelArrayData = CreateInstance<ReelArrayData>();

            // ファイル読み込み

            // 配列作成
            using StreamReader array = new StreamReader(Path.Combine(ScriptableGen.DataPath, reelName, ReelFileStartPath + reelName + "Array.csv"));
            reelArrayData.SetArray(MakeReelArray(array));

            // 保存処理
            ScriptableGen.GenerateFile(path, fileName, reelArrayData);
        }

        // リールスベリコマデータの作成
        private static void MakeReelDelayData(string reelName)
        {
            string path = "Assets/ReelData";
            string fileName = reelName + "Delay";

            // ファイルが無ければ新規作成
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            ReelDelayTableData reelDatabase = CreateInstance<ReelDelayTableData>();

            // ファイル読み込み

            // 停止順ごとの条件テーブル作成
            using StreamReader first = new StreamReader(Path.Combine(ScriptableGen.DataPath, reelName, ReelFileStartPath + reelName + "1st.csv"));
            using StreamReader second = new StreamReader(Path.Combine(ScriptableGen.DataPath, reelName, ReelFileStartPath + reelName + "2nd.csv"));
            using StreamReader third = new StreamReader(Path.Combine(ScriptableGen.DataPath, reelName, ReelFileStartPath + reelName + "3rd.csv"));

            reelDatabase.SetReelConditions(MakeReelFirstData(first), MakeReelSecondData(second), MakeReelThirdData(third));

            // ディレイテーブル作成
            using StreamReader table = new StreamReader(Path.Combine(ScriptableGen.DataPath, reelName, ReelFileStartPath + reelName + "Table.csv"));
            reelDatabase.SetTables(MakeTableDatas(table));

            // 保存処理
            ScriptableGen.GenerateFile(path, fileName, reelDatabase);
        }

        public static byte[] MakeReelArray(StreamReader arrayFile)
        {
            // 配列読み込み
            string[] values = arrayFile.ReadLine().Split(',');
            // 配列に変換
            return Array.ConvertAll(values, byte.Parse);
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