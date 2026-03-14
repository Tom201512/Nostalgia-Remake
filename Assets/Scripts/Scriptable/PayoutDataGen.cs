using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ScriptableObject;

namespace ReelSpinGame_Scriptable
{
#if UNITY_EDITOR
    public class PayoutDatabaseGen
    {
        private const string PayoutDatabasePath = "Payout";

        [MenuItem("ScriptableGen/CreatePayoutDatabase")]
        public static void MakePayoutData()
        {
            // ディレクトリの作成
            string path = "Assets/Payout";
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
            using StreamReader payoutLine = new StreamReader(Path.Combine(ScriptableGen.DataPath, PayoutDatabasePath, "Nostalgia_Payout - PayoutLineData.csv"));
            payoutDatabase.PayoutLines = MakePayoutLineDatas(payoutLine);

            // 払い出し組み合わせ表作成
            // 通常時
            using StreamReader normalPayout = new StreamReader(Path.Combine(ScriptableGen.DataPath, PayoutDatabasePath, "Nostalgia_Payout - NormalPayout.csv"));
            payoutDatabase.NormalPayoutDatas = MakeResultDatas(normalPayout);

            // 小役ゲーム中
            using StreamReader bigPayout = new StreamReader(Path.Combine(ScriptableGen.DataPath, PayoutDatabasePath, "Nostalgia_Payout - BigPayout.csv"));
            payoutDatabase.BigPayoutDatas = MakeResultDatas(bigPayout);

            // JACゲーム中
            using StreamReader jacPayout = new StreamReader(Path.Combine(ScriptableGen.DataPath, PayoutDatabasePath, "Nostalgia_Payout - JacPayout.csv"));
            payoutDatabase.JacPayoutDatas = MakeResultDatas(jacPayout);

            // 保存処理
            ScriptableGen.GenerateFile(path, fileName, payoutDatabase);
        }

        // 払い出しラインデータ作成
        private static List<PayoutLineData> MakePayoutLineDatas(StreamReader payoutLineFile)
        {
            List<PayoutLineData> finalResult = new List<PayoutLineData>();

            // 全ての行を読み込む
            while (payoutLineFile.Peek() != -1)
            {
                finalResult.Add(new PayoutLineData(payoutLineFile));
            }
            return finalResult;
        }

        // 払い出し結果データ作成
        public static List<PayoutResultData> MakeResultDatas(StreamReader payoutResultFile)
        {
            List<PayoutResultData> finalResult = new List<PayoutResultData>();

            // 全ての行を読み込む
            while (payoutResultFile.Peek() != -1)
            {
                finalResult.Add(new PayoutResultData(payoutResultFile));
            }
            return finalResult;
        }
    }
#endif
}
