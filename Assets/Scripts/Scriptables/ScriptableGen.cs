using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // スクリプタブルを作成する
#if UNITY_EDITOR
    public class ScriptableGen : EditorWindow
    {
        // const
        // デフォルトで使うファイルパス
        private const string DataPath = "Assets/DataFile";

        // リールのファイル
        private const string LeftPath = "ReelL";
        private const string MiddlePath = "ReelM";
        private const string RightPath = "ReelR";

        // var
        // 各リールを作る際に選んだボタン番号
        private int reelSelection;

        // func
        // リール配列の作成
        [MenuItem("ScriptableGen/Scriptable Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open ScriptableGen Generator");
            ScriptableGen window = GetWindow<ScriptableGen>();
            window.titleContent = new GUIContent("Scriptable Generator");
        }

        private void Awake()
        {
            reelSelection = -1;
        }

        private void OnGUI()
        {
            GUILayout.Label("スクリプタブルオブジェクト作成\n");
            // fileName = EditorGUILayout.TextField("File Name", fileName);
            //arrayFile = (TextAsset)EditorGUILayout.ObjectField("Array:", arrayFile, typeof(TextAsset), true);
            //conditionsFile = (TextAsset)EditorGUILayout.ObjectField("Conditions:", conditionsFile, typeof(TextAsset), true);
            //delaysData = (TextAsset)EditorGUILayout.ObjectField("Tables:", delaysData, typeof(TextAsset), true);
            GUILayout.Label("リールデータ作成\n");

            if (GUILayout.Button("全リールデータ作成"))
            {
                Debug.Log("Pressed");
                //MakeReelData(fileName, arrayFile, conditionsFile, delaysData);
                MakeReelDataAll();
            }

            GUILayout.Label("\n各リールの作成\n");

            reelSelection = GUILayout.Toolbar(reelSelection, new[] { "左", "中", "右" });

            if (reelSelection == (int)ReelManager.ReelID.ReelLeft)
            {
                reelSelection = -1;
                MakeReelData(LeftPath);
            }

            if (reelSelection == (int)ReelManager.ReelID.ReelMiddle)
            {
                reelSelection = -1;
                MakeReelData(MiddlePath);
            }

            if (reelSelection == (int)ReelManager.ReelID.ReelRight)
            {
                reelSelection = -1;
                MakeReelData(RightPath);
            }
        }

        private void MakeReelDataAll()
        {
            string[] pathOrder = { LeftPath, MiddlePath, RightPath };

            // あらかじめ設定したディレクトリから全リールのデータを読み込む
            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                MakeReelData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        private void MakeReelData(string filePath)
        {
            // ディレクトリの作成
            string path = "Assets/ReelDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }
            // スクリプタブルオブジェクト作成
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();
            // 配列作成
            reelDatabase.SetArray(MakeReelArray(new StreamReader(Path.Combine(DataPath, filePath, filePath + "Array.csv"))));
            // 条件テーブル作成
            reelDatabase.SetConditions(MakeReelConditions(new StreamReader(Path.Combine(DataPath, filePath, filePath + "Condition.csv"))));
            // ディレイテーブル作成
            reelDatabase.SetTables(MakeTableDatas(new StreamReader(Path.Combine(DataPath, filePath, filePath + "Table.csv"))));

            var fileName = filePath + ".asset";
            // 保存処理
            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }

        private byte[] MakeReelArray(StreamReader arrayFile)
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

        private List<ReelConditionsData> MakeReelConditions(StreamReader conditionsFile)
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

        private List<ReelTableData> MakeTableDatas(StreamReader tablesFile)
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
