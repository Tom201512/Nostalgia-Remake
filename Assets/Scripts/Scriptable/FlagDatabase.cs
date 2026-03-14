using ReelSpinGame_Flag;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // フラグデータベース
    public class FlagDatabase : ScriptableObject
    {
        [SerializeField] private FlagDatabaseSet normalABet1Table;    // 通常時A 1枚掛け時(低確率)
        [SerializeField] private FlagDatabaseSet normalABet2Table;    // 通常時A 2枚掛け時(低確率)
        [SerializeField] private FlagDatabaseSet normalABet3Table;    // 通常時A 3枚掛け時(低確率)

        [SerializeField] private FlagDatabaseSet normalBBet1Table;    // 通常時B 1枚掛け時(高確率)
        [SerializeField] private FlagDatabaseSet normalBBet2Table;    // 通常時B 2枚掛け時(高確率)
        [SerializeField] private FlagDatabaseSet normalBBet3Table;    // 通常時B 3枚掛け時(高確率)

        [SerializeField] private FlagDatabaseSet bigBet1Table;        // ビッグチャンス 1枚掛け時
        [SerializeField] private FlagDatabaseSet bigBet2Table;        // ビッグチャンス 2枚掛け時
        [SerializeField] private FlagDatabaseSet bigBet3Table;        // ビッグチャンス 3枚掛け時

        [SerializeField] private FlagDatabaseSet jacTable;            // JAC時

        public FlagDatabaseSet NormalATableBet1
        {
            get => normalABet1Table;
            set => normalABet1Table = value;
        }

        public FlagDatabaseSet NormalATableBet2
        {
            get => normalABet2Table;
            set => normalABet2Table = value;
        }

        public FlagDatabaseSet NormalATableBet3
        {
            get => normalABet3Table;
            set => normalABet3Table = value;
        }

        public FlagDatabaseSet NormalBTableBet1
        {
            get => normalBBet1Table;
            set => normalBBet1Table = value;
        }

        public FlagDatabaseSet NormalBTableBet2
        {
            get => normalBBet2Table;
            set => normalBBet2Table = value;
        }

        public FlagDatabaseSet NormalBTableBet3
        {
            get => normalBBet3Table;
            set => normalBBet3Table = value;
        }

        public FlagDatabaseSet BigTableBet1
        {
            get => bigBet1Table;
            set => bigBet1Table = value;
        }

        public FlagDatabaseSet BigTableBet2
        {
            get => bigBet2Table;
            set => bigBet2Table = value;
        }

        public FlagDatabaseSet BigTableBet3
        {
            get => bigBet3Table;
            set => bigBet3Table = value;
        }

        public FlagDatabaseSet JacTable
        {
            get => jacTable;
            set => jacTable = value;
        }
    }

    // 各テーブル、ベット枚数ごとのフラグデータ
    [Serializable]
    public class FlagDatabaseSet
    {
        [SerializeField] List<FlagData> flagDataList;

        public List<FlagData> FlagDataList { get => flagDataList;}

        public FlagDatabaseSet(StreamReader loadedData)
        {
            flagDataList = new List<FlagData>();
            // 全ての行を読み込む
            while (loadedData.Peek() != -1)
            {
                flagDataList.Add(new FlagData(loadedData));
            }
        }
    }

    // フラグデータ
    [Serializable]
    public class FlagData
    {
        // 配列要素の識別用
        public enum FlagIndexSerialize
        {
            FlagID = 0,
            FlagProbabilityStart = 1 << 0,
        }

        [SerializeField] private FlagModel.FlagID flagID;       //フラグID
        [SerializeField] private List<int> flagCountBySetting;  // 設定ごとのフラグ数

        public FlagModel.FlagID FlagID {  get { return flagID; } }
        public List<int> FlagCountBySetting {  get { return flagCountBySetting; } }

        public FlagData(StreamReader loadedData)
        {
            flagCountBySetting = new List<int>();
            string[] values = loadedData.ReadLine().Split(',');
            int index = 0;

            foreach (string s in values)
            {
                // 最初の数値はフラグIDを登録する
                if(index == (int)FlagIndexSerialize.FlagID)
                {
                    flagID = (FlagModel.FlagID)Enum.Parse(typeof(FlagModel.FlagID), s);
                }
                else
                {
                    flagCountBySetting.Add(Convert.ToInt32(s));
                }
                index += 1;
            }
        }
    }
}

