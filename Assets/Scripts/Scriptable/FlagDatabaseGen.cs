using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ScriptableObject;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class FlagDatabaseGen
    {
        private const string LotsTablePath = "LotsTable";

        [MenuItem("ScriptableGen/CreateFlagDatabase")]
        private static void MakeFlagData()
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

            using StreamReader normalABet1 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableNormalABet1.csv"));
            flagDatabase.NormalATableBet1 = MakeFlagDataSets(normalABet1);

            using StreamReader normalABet2 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableNormalABet1.csv"));
            flagDatabase.NormalATableBet2 = MakeFlagDataSets(normalABet2);

            using StreamReader normalABet3 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableNormalABet1.csv"));
            flagDatabase.NormalATableBet3 = MakeFlagDataSets(normalABet3);


            using StreamReader normalBBet1 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableNormalBBet1.csv"));
            flagDatabase.NormalBTableBet1 = MakeFlagDataSets(normalBBet1);

            using StreamReader normalBBet2 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableNormalBBet2.csv"));
            flagDatabase.NormalBTableBet2 = MakeFlagDataSets(normalBBet2);

            using StreamReader normalBBet3 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableNormalBBet3.csv"));
            flagDatabase.NormalBTableBet3 = MakeFlagDataSets(normalBBet3);


            using StreamReader bigBet1 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableBIGBet1.csv"));
            flagDatabase.BigTableBet1 = MakeFlagDataSets(bigBet1);

            using StreamReader bigBet2 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableBIGBet1.csv"));
            flagDatabase.BigTableBet2 = MakeFlagDataSets(bigBet2);

            using StreamReader bigBet3 = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableBIGBet3.csv"));
            flagDatabase.BigTableBet3 = MakeFlagDataSets(bigBet3);

            using StreamReader jacTable = new StreamReader(Path.Combine(ScriptableGen.DataPath, LotsTablePath, "Nostalgia_Flag - FlagTableJAC.csv"));
            flagDatabase.JacTable = MakeFlagDataSets(jacTable);

            // 保存処理
            ScriptableGen.GenerateFile(path, fileName, flagDatabase);
        }
        static FlagDatabaseSet MakeFlagDataSets(StreamReader flagTableFile)
        {
            return new FlagDatabaseSet(flagTableFile);
        }
    }
#endif
}

