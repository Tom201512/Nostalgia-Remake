using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class PayoutDatabaseGen : EditorWindow
    {
        // const

        // var
        // ファイル名
        private string fileName;

        // ファイル指定
        private TextAsset payoutLineFile;
        private TextAsset normalPayoutFile;
        private TextAsset bigPayoutFile;
        private TextAsset jacPayoutFile;

        // func
        // リール配列の作成
        [MenuItem("ScriptableGen/PayoutData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open PayoutData Generator");
            PayoutDatabaseGen window = GetWindow<PayoutDatabaseGen>();
            window.titleContent = new GUIContent("PayoutData Generator");
        }

        private void Awake()
        {
            fileName = "PayoutDatabase.asset";
            payoutLineFile = null;
            normalPayoutFile = null;
            bigPayoutFile = null;
            jacPayoutFile = null;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("払い出しデータベース作成");
            payoutLineFile = (TextAsset)EditorGUILayout.ObjectField("PayoutLines:", payoutLineFile, typeof(TextAsset), true);
            normalPayoutFile = (TextAsset)EditorGUILayout.ObjectField("NormalPayouts:", normalPayoutFile, typeof(TextAsset), true);
            bigPayoutFile = (TextAsset)EditorGUILayout.ObjectField("BigPayouts:", bigPayoutFile, typeof(TextAsset), true);
            jacPayoutFile = (TextAsset)EditorGUILayout.ObjectField("JacPayouts:", jacPayoutFile, typeof(TextAsset), true);

            if (GUILayout.Button("払い出しデータベース作成"))
            {
                Debug.Log("Pressed");
                MakePayoutData(payoutLineFile, normalPayoutFile, bigPayoutFile, jacPayoutFile);
            }
        }

        private void MakePayoutData(TextAsset payoutLineFile, TextAsset normalPayoutFile, TextAsset bigPayoutFile, TextAsset jacPayoutFile)
        {
            // ディレクトリの作成
            string path = "Assets/PayoutDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            PayoutDatabase payoutDatabase = CreateInstance<PayoutDatabase>();

            // 払い出しライン作成
            payoutDatabase.SetPayoutLines(MakePayoutLineDatas(payoutLineFile));
            // 払い出し組み合わせ表作成
            // 通常時
            payoutDatabase.SetNormalPayout(MakeResultDatas(normalPayoutFile));
            // 小役ゲーム中
            payoutDatabase.SetBigPayout(MakeResultDatas(bigPayoutFile));
            // JACゲーム中
            payoutDatabase.SetJacPayout(MakeResultDatas(jacPayoutFile));

            // 保存処理
            AssetDatabase.CreateAsset(payoutDatabase, Path.Combine(path, fileName));
            Debug.Log("Payout Database is generated");
        }

        private List<PayoutLineData> MakePayoutLineDatas(TextAsset payoutLineFile)
        {
            List<PayoutLineData> finalResult = new List<PayoutLineData>();
            StringReader buffer = new StringReader(payoutLineFile.text);

            Debug.Log(buffer.ToString());

            // 全ての行を読み込む
            while (buffer.Peek() != -1)
            {
                finalResult.Add(new PayoutLineData(buffer));
            }
            return finalResult;
        }

        private List<PayoutResultData> MakeResultDatas(TextAsset payoutResultFile)
        {
            List<PayoutResultData> finalResult = new List<PayoutResultData>();
            StringReader buffer = new StringReader(payoutResultFile.text);

            // 全ての行を読み込む
            while (buffer.Peek() != -1)
            {
                finalResult.Add(new PayoutResultData(buffer));
            }
            return finalResult;
        }
    }
#endif
}
