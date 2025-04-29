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

        // フラグのファイル
        private const string FlagPath = "LotsTable";

        // var
        // 各リールを作る際に選んだボタン番号
        private int reelSelection;

        // JACゲーム時のはずれ確率
        private float jacNoneProb;

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

            GUILayout.Label("\nフラグデータの作成\n");
            jacNoneProb = EditorGUILayout.FloatField("JAC時はずれ確率(float)", jacNoneProb);

            if (GUILayout.Button("フラグデータ作成"))
            {
                Debug.Log("Pressed");
                MakeFlagData();
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
            reelDatabase.SetArray(ReelDatabaseGen.MakeReelArray
                (new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Array.csv"))));
            // 条件テーブル作成
            reelDatabase.SetConditions(ReelDatabaseGen.MakeReelConditions
                (new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Condition.csv"))));
            // ディレイテーブル作成
            reelDatabase.SetTables(ReelDatabaseGen.MakeTableDatas
                (new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Table.csv"))));

            var fileName = filePath + ".asset";
            // 保存処理
            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }


        private void MakeFlagData()
        {
            // ディレクトリの作成
            string path = "Assets/FlagDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            FlagDatabase flagDatabase = CreateInstance<FlagDatabase>();

            // フラグテーブル作成
            // 通常時Aフラグテーブル作成
            flagDatabase.SetNormalATable(FlagDatabaseGen.MakeFlagTableSets(
                new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalA.csv"))));

            // 通常時Bフラグテーブル作成
            flagDatabase.SetNormalBTable(FlagDatabaseGen.MakeFlagTableSets(
                new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalB.csv"))));

            // 小役ゲーム中フラグテーブル作成
            flagDatabase.SetBIGTable(FlagDatabaseGen.MakeFlagTableSets(
                new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableBig.csv"))));
            // JAC時ハズレ
            flagDatabase.SetJACNonePoss(jacNoneProb);

            // 保存処理
            AssetDatabase.CreateAsset(flagDatabase, Path.Combine(path, "FlagDatabase.asset"));
            Debug.Log("Flag Database is generated");
        }
    }
#endif
}
