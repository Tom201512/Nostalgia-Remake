using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    //リールデータ作成用
#if UNITY_EDITOR
    public class ReelDatabaseGen : EditorWindow
    {
        // const

        // var
        // ファイル名
        private string fileName;

        // ファイル指定
        private TextAsset arrayFile;
        private TextAsset conditionsFile;
        private TextAsset delaysData;

        // func
        // リール配列の作成
        [MenuItem("ScriptableGen/ReelData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open ReelData Generator");
            ReelDatabaseGen window = GetWindow<ReelDatabaseGen>();
            window.titleContent = new GUIContent("ReelData Generator");
        }

        private void Awake()
        {
            fileName = "";
            arrayFile = null;
            conditionsFile = null;
            delaysData = null;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("リールデータベース作成");
            fileName = EditorGUILayout.TextField("File Name", fileName);
            arrayFile = (TextAsset)EditorGUILayout.ObjectField("Array:", arrayFile, typeof(TextAsset), true);
            conditionsFile = (TextAsset)EditorGUILayout.ObjectField("Conditions:", conditionsFile, typeof(TextAsset), true);
            delaysData = (TextAsset)EditorGUILayout.ObjectField("Tables:", delaysData, typeof(TextAsset), true);

            if (GUILayout.Button("リールデータ作成"))
            {
                Debug.Log("Pressed");
                MakeReelData(fileName, arrayFile, conditionsFile, delaysData);
            }
        }

        private void MakeReelData(string _fileName, TextAsset arrayFile, TextAsset conditionsFile, TextAsset tablesFile)
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
            reelDatabase.SetArray(MakeReelArray(arrayFile));
            // 条件テーブル作成
            reelDatabase.SetConditions(MakeReelConditions(conditionsFile));
            // ディレイテーブル作成
            reelDatabase.SetTables(MakeTableDatas(tablesFile));

            var fileName = _fileName + ".asset";
            // 保存処理
            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }

        private byte[] MakeReelArray(TextAsset arrayFile)
        {
            StringReader buffer = new StringReader(arrayFile.text);
            // 配列読み込み
            string[] values = buffer.ReadLine().Split(',');
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

        private List<ReelConditionsData> MakeReelConditions(TextAsset conditionsFile)
        {
            List<ReelConditionsData> finalResult = new List<ReelConditionsData>();
            StringReader buffer = new StringReader(conditionsFile.text);

            // 全ての行を読み込む
            while(buffer.Peek() != -1)
            {
                finalResult.Add(new ReelConditionsData(buffer));
            }

            foreach(ReelConditionsData condition in finalResult)
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

        private List<ReelTableData> MakeTableDatas(TextAsset tablesFile)
        {
            List<ReelTableData> finalResult = new List<ReelTableData>();
            StringReader buffer = new StringReader(tablesFile.text);

            // 全ての行を読み込む
            while (buffer.Peek() != -1)
            {
                finalResult.Add(new ReelTableData(buffer));
            }

            foreach (ReelTableData table in finalResult)
            {
                Debug.Log("Array:" + table.TableData);
            }

            return finalResult;
        }
    }
#endif
}
