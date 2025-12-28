using System.IO;
using UnityEditor;
using UnityEngine;
using static ReelSpinGame_Reels.ReelLogicManager;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_Datas
{
    // スクリプタブルを作成する
#if UNITY_EDITOR
    public class ScriptableGen : EditorWindow
    {
        // const
        // 読み込み時のパス
        private const string DataPath = "Assets/DataFile";

        // リールファイル先頭のファイル名
        private const string ReelFileStartPath = "Nostalgia ReelSheet - ";

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

        private void OnEnable()
        {
            reelSelection = -1;
            jacNoneProb = JacNoneDefault;
        }

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("スクリプタブルオブジェクト作成\n");
                GUILayout.Label("リールデータ作成\n");

                if (GUILayout.Button("全リールデータ作成"))
                {
                    MakeReelDataAll();
                }

                GUILayout.Label("\n各リールの作成\n");

                reelSelection = GUILayout.Toolbar(reelSelection, new[] { "左", "中", "右" });

                if (reelSelection == (int)ReelID.ReelLeft)
                {
                    reelSelection = -1;
                    MakeReelArrayData("ReelL");
                    MakeReelDelayData("ReelL");
                }

                if (reelSelection == (int)ReelID.ReelMiddle)
                {
                    reelSelection = -1;
                    MakeReelArrayData("ReelM");
                    MakeReelDelayData("ReelM");
                }

                if (reelSelection == (int)ReelID.ReelRight)
                {
                    reelSelection = -1;
                    MakeReelArrayData("ReelR");
                    MakeReelDelayData("ReelR");
                }

                GUILayout.Label("\nフラグデータの作成\n");
                jacNoneProb = EditorGUILayout.FloatField("JAC時はずれ確率(float)", jacNoneProb);

                if (GUILayout.Button("フラグデータ作成"))
                {
                    MakeFlagData();
                }

                GUILayout.Label("\n払い出しデータベース作成\n");

                if (GUILayout.Button("払い出しデータベース作成"))
                {
                    MakePayoutData();
                }
            }
        }

        private void MakeReelDataAll()
        {
            string[] pathOrder = { "ReelL", "ReelM", "ReelR" };

            // あらかじめ設定したディレクトリから全リールのデータを読み込む
            for (int i = 0; i < ReelAmount; i++)
            {
                MakeReelArrayData(pathOrder[i]);
                MakeReelDelayData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        // リール配列の作成
        private void MakeReelArrayData(string reelName)
        {
            // ディレクトリの作成
            string path = "Assets/ReelData";
            // ファイル名指定
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
            using StreamReader array = new StreamReader(Path.Combine(DataPath, reelName, ReelFileStartPath + reelName + "Array.csv"));
            reelArrayData.SetArray(ReelDatabaseGen.MakeReelArray(array));

            // 保存処理
            GenerateFile(path, fileName, reelArrayData);
        }

        // リールスベリコマデータの作成
        private void MakeReelDelayData(string reelName)
        {
            // ディレクトリの作成
            string path = "Assets/ReelData";
            // ファイル名指定
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
            using StreamReader first = new StreamReader(Path.Combine(DataPath, reelName, ReelFileStartPath + reelName + "1st.csv"));
            using StreamReader second = new StreamReader(Path.Combine(DataPath, reelName, ReelFileStartPath + reelName + "2nd.csv"));
            using StreamReader third = new StreamReader(Path.Combine(DataPath, reelName, ReelFileStartPath + reelName + "3rd.csv"));

            reelDatabase.SetReelConditions(ReelDatabaseGen.MakeReelFirstData(first), ReelDatabaseGen.MakeReelSecondData(second), ReelDatabaseGen.MakeReelThirdData(third));

            // ディレイテーブル作成
            using StreamReader table = new StreamReader(Path.Combine(DataPath, reelName, ReelFileStartPath + reelName + "Table.csv"));
            reelDatabase.SetTables(ReelDatabaseGen.MakeTableDatas(table));

            // 保存処理
            GenerateFile(path, fileName, reelDatabase);
        }

        private void MakeFlagData()
        {
            // ディレクトリの作成
            string path = "Assets/FlagData";
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
            using StreamReader normalA = new StreamReader(Path.Combine(DataPath, "LotsTable", "Nostalgia_Flag - FlagTableNormalA.csv"));
            flagDatabase.SetNormalATable(FlagDatabaseGen.MakeFlagTableSets(normalA));

            // 通常時Bフラグテーブル作成
            using StreamReader normalB = new StreamReader(Path.Combine(DataPath, "LotsTable", "Nostalgia_Flag - FlagTableNormalB.csv"));
            flagDatabase.SetNormalBTable(FlagDatabaseGen.MakeFlagTableSets(normalB));

            // 小役ゲーム中フラグテーブル作成
            using StreamReader bigTable = new StreamReader(Path.Combine(DataPath, "LotsTable", "Nostalgia_Flag - FlagTableBig.csv"));
            flagDatabase.SetBIGTable(FlagDatabaseGen.MakeFlagTableSets(bigTable));

            // JAC時ハズレ
            flagDatabase.SetJACNonePoss(jacNoneProb);

            // 保存処理
            GenerateFile(path, fileName, flagDatabase);
        }


        private void MakePayoutData()
        {
            // ディレクトリの作成
            string path = "Assets/PayoutData";
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
            using StreamReader payoutLine = new StreamReader(Path.Combine(DataPath, "PayoutTable", "Nostalgia_Payout - PayoutLineData.csv"));
            payoutDatabase.SetPayoutLines(PayoutDatabaseGen.MakePayoutLineDatas(payoutLine));

            // 払い出し組み合わせ表作成
            // 通常時
            using StreamReader normalPayout = new StreamReader(Path.Combine(DataPath, "PayoutTable", "Nostalgia_Payout - NormalPayout.csv"));
            payoutDatabase.SetNormalPayout(PayoutDatabaseGen.MakeResultDatas(normalPayout));

            // 小役ゲーム中
            using StreamReader bigPayout = new StreamReader(Path.Combine(DataPath, "PayoutTable", "Nostalgia_Payout - BigPayout.csv"));
            payoutDatabase.SetBigPayout(PayoutDatabaseGen.MakeResultDatas(bigPayout));

            // JACゲーム中
            using StreamReader jacPayout = new StreamReader(Path.Combine(DataPath, "PayoutTable", "Nostalgia_Payout - JacPayout.csv"));
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
