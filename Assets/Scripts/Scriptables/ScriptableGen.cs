using System.IO;
using UnityEditor;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

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

        // 払い出し関連のファイル
        private const string PayoutPath = "Payouts";

        // JACはずれデフォルト値
        private const float JacNoneDefault = 256f;

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
            jacNoneProb = JacNoneDefault;
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

            if (reelSelection == (int)ReelID.ReelLeft)
            {
                reelSelection = -1;
                MakeReelData(LeftPath);
            }

            if (reelSelection == (int)ReelID.ReelMiddle)
            {
                reelSelection = -1;
                MakeReelData(MiddlePath);
            }

            if (reelSelection == (int)ReelID.ReelRight)
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

            GUILayout.Label("\n払い出しデータベース作成\n");

            if (GUILayout.Button("払い出しデータベース作成"))
            {
                Debug.Log("Pressed");
                MakePayoutData();
            }
        }

        private void MakeReelDataAll()
        {
            string[] pathOrder = { LeftPath, MiddlePath, RightPath };

            // あらかじめ設定したディレクトリから全リールのデータを読み込む
            for (int i = 0; i < ReelAmounts; i++)
            {
                MakeReelData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        private void MakeReelData(string filePath)
        {
            // ディレクトリの作成
            string path = "Assets/ReelDatas";
            // ファイル名指定
            string fileName = filePath;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();

            // ファイル読み込み

            // 配列作成
            using StreamReader array = new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Array.csv"));
            reelDatabase.SetArray(ReelDatabaseGen.MakeReelArray(array));

            // 条件テーブル作成
            using StreamReader condition = new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Condition.csv"));
            reelDatabase.SetConditions(ReelDatabaseGen.MakeReelConditions(condition));

            // ディレイテーブル作成
            using StreamReader table = new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Table.csv"));
            reelDatabase.SetTables(ReelDatabaseGen.MakeTableDatas(table));

            // 保存処理
            GenerateFile(path, fileName, reelDatabase);
        }


        private void MakeFlagData()
        {
            // ディレクトリの作成
            string path = "Assets/FlagDatas";
            // ファイル名指定
            string fileName = "FlagDatabase";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            FlagDatabase flagDatabase = CreateInstance<FlagDatabase>();

            // フラグテーブル作成
            // 通常時Aフラグテーブル作成
            using StreamReader normalA = new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalA.csv"));
            flagDatabase.SetNormalATable(FlagDatabaseGen.MakeFlagTableSets(normalA));

            // 通常時Bフラグテーブル作成
            using StreamReader normalB = new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalB.csv"));
            flagDatabase.SetNormalBTable(FlagDatabaseGen.MakeFlagTableSets(normalB));

            // 小役ゲーム中フラグテーブル作成
            using StreamReader bigTable = new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableBig.csv"));
            flagDatabase.SetBIGTable(FlagDatabaseGen.MakeFlagTableSets(bigTable));

            // JAC時ハズレ
            flagDatabase.SetJACNonePoss(jacNoneProb);

            // 保存処理
            GenerateFile(path, fileName, flagDatabase);
        }


        private void MakePayoutData()
        {
            // ディレクトリの作成
            string path = "Assets/PayoutDatas";
            // ファイル名指定
            string fileName = "PayoutDatabase";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            PayoutDatabase payoutDatabase = CreateInstance<PayoutDatabase>();

            // 払い出しライン作成
            using StreamReader payoutLine = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - PayoutLineData.csv"));
            payoutDatabase.SetPayoutLines(PayoutDatabaseGen.MakePayoutLineDatas(payoutLine));

            // 払い出し組み合わせ表作成
            // 通常時
            using StreamReader normalPayout = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - NormalPayout.csv"));
            payoutDatabase.SetNormalPayout(PayoutDatabaseGen.MakeResultDatas(normalPayout));

            // 小役ゲーム中
            using StreamReader bigPayout = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - BigPayout.csv"));
            payoutDatabase.SetBigPayout(PayoutDatabaseGen.MakeResultDatas(bigPayout));

            // JACゲーム中
            using StreamReader jacPayout = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - JacPayout.csv"));
            payoutDatabase.SetJacPayout(PayoutDatabaseGen.MakeResultDatas(jacPayout));

            // 保存処理
            GenerateFile(path, fileName, payoutDatabase);
        }

        private void GenerateFile(string path, string fileName, ScriptableObject scriptableObject)
        {
            // 保存処理
            // ファイルがある場合
            if (File.Exists(Path.Combine(path, fileName) + ".asset"))
            {
                // ディレクトリの作成
                string temporaryPath = "Assets/Temp";

                Directory.CreateDirectory(temporaryPath);
                Debug.Log("Temporary Directory is created");

                // 書き換え用の仮ファイルを作成
                AssetDatabase.CreateAsset(scriptableObject, Path.Combine(temporaryPath, fileName) + ".asset");
                // 元あったファイルに置き換える
                FileUtil.ReplaceFile(Path.Combine(temporaryPath, fileName) + ".asset", Path.Combine(path, fileName) + ".asset");

                // 仮ファイルを削除
                AssetDatabase.DeleteAsset(temporaryPath);
                Debug.Log(fileName + " is replaced");
            }
            else
            {
                AssetDatabase.CreateAsset(scriptableObject, Path.Combine(path, fileName) + ".asset");
                Debug.Log(fileName + " is generated");
            }

            AssetDatabase.Refresh();
        }
    }
#endif
}
