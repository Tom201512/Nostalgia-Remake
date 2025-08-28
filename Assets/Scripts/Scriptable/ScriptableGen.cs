using ReelSpinGame_Sound.SE;
using ReelSpinGame_Sound.BGM;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static ReelSpinGame_Datas.ScriptableGen;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Datas
{
    // スクリプタブルを作成する
#if UNITY_EDITOR
    public class ScriptableGen : EditorWindow
    {
        // const
        // 読み込み時のパス
        private const string DataPath = "Assets/DataFile";

        // JACはずれデフォルト値
        private const float JacNoneDefault = 256f;

        // var
        // 各リールを作る際に選んだボタン番号
        private int reelSelection;

        // JACゲーム時のはずれ確率
        private float jacNoneProb;

        // 効果音にするAudioClipの配列
        [Serializable]
        public class AudioClipList : ScriptableObject
        {
            [SerializeField] private List<AudioClip> seAudioClipList;
            [SerializeField] private List<AudioClip> bgmAudioClipList;

            public List<AudioClip> SeAudioClipList { get { return seAudioClipList; } }
            public List<AudioClip> BgmAudioClipList { get { return bgmAudioClipList; } }
        }

        private AudioClipList audioClipList;

        // スクロール値
        private Vector2 scrollPos;

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
            audioClipList = CreateInstance<AudioClipList>();
            scrollPos = new Vector2(0, 0);
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
                    MakeReelData("ReelL");
                }

                if (reelSelection == (int)ReelID.ReelMiddle)
                {
                    reelSelection = -1;
                    MakeReelData("ReelM");
                }

                if (reelSelection == (int)ReelID.ReelRight)
                {
                    reelSelection = -1;
                    MakeReelData("ReelR");
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
                GUILayout.Label("\nSE、BGMファイル変換\n");

                using (EditorGUILayout.ScrollViewScope scroll = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Height(200)))
                {
                    scrollPos = scroll.scrollPosition;
                    Editor.CreateEditor(audioClipList).OnInspectorGUI();
                }

                if (GUILayout.Button("SEファイル変換"))
                {
                    MakeSEFile();
                }

                if (GUILayout.Button("BGMファイル変換"))
                {
                    MakeBGMFile();
                }
            }
        }

        private void MakeReelDataAll()
        {
            string[] pathOrder = { "ReelL", "ReelM", "ReelR" };

            // あらかじめ設定したディレクトリから全リールのデータを読み込む
            for (int i = 0; i < ReelAmount; i++)
            {
                MakeReelData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        private void MakeReelData(string reelName)
        {
            // ディレクトリの作成
            string path = "Assets/ReelData";
            // ファイル名指定
            string fileName = reelName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();

            // ファイル読み込み

            // 配列作成
            using StreamReader array = new StreamReader(Path.Combine(DataPath, reelName, "Nostalgia_Reel - " + reelName + "Array.csv"));
            reelDatabase.SetArray(ReelDatabaseGen.MakeReelArray(array));

            // 条件テーブル作成
            using StreamReader condition = new StreamReader(Path.Combine(DataPath, reelName, "Nostalgia_Reel - " + reelName + "Condition.csv"));
            reelDatabase.SetConditions(ReelDatabaseGen.MakeReelConditions(condition));

            // ディレイテーブル作成
            using StreamReader table = new StreamReader(Path.Combine(DataPath, reelName, "Nostalgia_Reel - " + reelName + "Table.csv"));
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
            using StreamReader normalA = new StreamReader(Path.Combine(DataPath, "FlagTable", "Nostalgia_Flag - FlagTableNormalA.csv"));
            flagDatabase.SetNormalATable(FlagDatabaseGen.MakeFlagTableSets(normalA));

            // 通常時Bフラグテーブル作成
            using StreamReader normalB = new StreamReader(Path.Combine(DataPath, "FlagTable", "Nostalgia_Flag - FlagTableNormalB.csv"));
            flagDatabase.SetNormalBTable(FlagDatabaseGen.MakeFlagTableSets(normalB));

            // 小役ゲーム中フラグテーブル作成
            using StreamReader bigTable = new StreamReader(Path.Combine(DataPath, "FlagTable", "Nostalgia_Flag - FlagTableBig.csv"));
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

        // AudioClipを独自の効果音ファイルに変換
        private void MakeSEFile()
        {
            if (audioClipList.SeAudioClipList.Count > 0)
            {
                // ディレクトリの作成
                string path = Path.Combine("Assets", "Sound", "SE");
                // ファイル名
                string fileName = "";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Debug.Log("Directory is created");
                }

                // 登録したファイルから作成
                foreach (AudioClip clip in audioClipList.SeAudioClipList)
                {
                    SeFile seFile = CreateInstance<SeFile>();
                    seFile.SourceFile = clip;
                    fileName = clip.name;
                    GenerateFile(path, fileName, seFile);
                }

                Debug.Log("Finished generate sound file");
            }
            else
            {
                Debug.LogError("No SE audioclip add");
            }
        }

        // AudioClipを独自の音楽ファイルに変換
        private void MakeBGMFile()
        {
            if (audioClipList.BgmAudioClipList.Count > 0)
            {
                // ディレクトリの作成
                string path = "Assets/Sound/BGM";
                // ファイル名
                string fileName = "";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Debug.Log("Directory is created");
                }

                // 登録したファイルから作成
                foreach (AudioClip clip in audioClipList.BgmAudioClipList)
                {
                    BgmFile bgmFile = CreateInstance<BgmFile>();
                    bgmFile.SourceFile = clip;
                    fileName = clip.name;
                    GenerateFile(path, fileName, bgmFile);
                }

                Debug.Log("Finished generate BGM file");
            }
            else
            {
                Debug.LogError("No BGM audioclip add");
            }
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

    // オーディオクリップのファイル編集
    [CustomEditor(typeof(AudioClipList))]
    public class AudioClipListEditor : Editor
    {
        private SerializedProperty seList;
        private SerializedProperty bgmList;

        public void Awake()
        {
            seList = serializedObject.FindProperty("seAudioClipList");
            bgmList = serializedObject.FindProperty("bgmAudioClipList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("SE");
            EditorGUILayout.PropertyField(seList);
            EditorGUILayout.LabelField("BGM");
            EditorGUILayout.PropertyField(bgmList);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
