using ReelSpinGame_Datass;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class FlagDatabaseGen : EditorWindow
    {
        // const

        // var
        // ファイル名
        private string fileName;

        // ファイル指定
        private TextAsset normalTableAfile;
        private TextAsset normalTableBfile;
        private TextAsset bigTablefile;
        private float jacNonePossibility;

        // func
        // リール配列の作成
        [MenuItem("ScriptableGen/FlagData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open FlagData Generator");
            FlagDatabaseGen window = GetWindow<FlagDatabaseGen>();
            window.titleContent = new GUIContent("FlagData Generator");
        }

        private void Awake()
        {
            fileName = "FlagDatabase.asset";
            normalTableAfile = null;
            normalTableBfile = null;
            bigTablefile = null;
            jacNonePossibility = 1.0f;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("フラグデータベース作成");
            normalTableAfile = (TextAsset)EditorGUILayout.ObjectField("NormalTableA:", normalTableAfile, typeof(TextAsset), true);
            normalTableBfile = (TextAsset)EditorGUILayout.ObjectField("NormalTableB:", normalTableBfile, typeof(TextAsset), true);
            bigTablefile = (TextAsset)EditorGUILayout.ObjectField("BigTable:", bigTablefile, typeof(TextAsset), true);
            jacNonePossibility = EditorGUILayout.FloatField("Jac None Possibility(n > 0)", jacNonePossibility);

            if (GUILayout.Button("フラグデータベース作成"))
            {
                Debug.Log("Pressed");

                if(jacNonePossibility > 0)
                {
                    MakeFlagData(normalTableAfile, normalTableBfile, bigTablefile, jacNonePossibility);
                }
                else
                {
                    Debug.LogError("JAC none possibility is must be higher that 0");
                }
            }
        }

        private void MakeFlagData(TextAsset normalTableAfile, TextAsset normalTableBfile, TextAsset bigTablefile, float jacNonePossibility)
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
            flagDatabase.SetNormalATable(MakeFlagTableSets(normalTableAfile));
            // 通常時Bフラグテーブル作成
            flagDatabase.SetNormalBTable(MakeFlagTableSets(normalTableBfile));
            // 小役ゲーム中フラグテーブル作成
            flagDatabase.SetBIGTable(MakeFlagTableSets(bigTablefile));

            // JAC時ハズレ
            flagDatabase.SetJACNonePoss(jacNonePossibility);

            // 保存処理
            AssetDatabase.CreateAsset(flagDatabase, Path.Combine(path, fileName));
            Debug.Log("Flag Database is generated");
        }

        private FlagDataSets MakeFlagTableSets(TextAsset flagTableFile)
        {
            StringReader buffer = new StringReader(flagTableFile.text);

            return new FlagDataSets(buffer);
        }
    }
#endif
}

