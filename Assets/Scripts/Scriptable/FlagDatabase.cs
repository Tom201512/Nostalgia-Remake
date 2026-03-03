using ReelSpinGame_Lots;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // フラグデータベース
    public class FlagDatabase : ScriptableObject
    {
        [SerializeField] private NewFlagDatabaseSet normalABet1Table;    // 通常時A 1枚掛け時(低確率)
        [SerializeField] private NewFlagDatabaseSet normalABet2Table;    // 通常時A 2枚掛け時(低確率)
        [SerializeField] private NewFlagDatabaseSet normalABet3Table;    // 通常時A 3枚掛け時(低確率)

        [SerializeField] private NewFlagDatabaseSet normalBBet1Table;    // 通常時B 1枚掛け時(高確率)
        [SerializeField] private NewFlagDatabaseSet normalBBet2Table;    // 通常時B 2枚掛け時(高確率)
        [SerializeField] private NewFlagDatabaseSet normalBBet3Table;    // 通常時B 3枚掛け時(高確率)

        [SerializeField] private NewFlagDatabaseSet bigBet1Table;        // ビッグチャンス 1枚掛け時
        [SerializeField] private NewFlagDatabaseSet bigBet2Table;        // ビッグチャンス 2枚掛け時
        [SerializeField] private NewFlagDatabaseSet bigBet3Table;        // ビッグチャンス 3枚掛け時

        [SerializeField] private NewFlagDatabaseSet jacTable;            // JAC時

        public NewFlagDatabaseSet NormalATableBet1
        {
            get => normalABet1Table;
            set => normalABet1Table = value;
        }

        public NewFlagDatabaseSet NormalATableBet2
        {
            get => normalABet2Table;
            set => normalABet2Table = value;
        }

        public NewFlagDatabaseSet NormalATableBet3
        {
            get => normalABet3Table;
            set => normalABet3Table = value;
        }

        public NewFlagDatabaseSet NormalBTableBet1
        {
            get => normalBBet1Table;
            set => normalBBet1Table = value;
        }

        public NewFlagDatabaseSet NormalBTableBet2
        {
            get => normalBBet2Table;
            set => normalBBet2Table = value;
        }

        public NewFlagDatabaseSet NormalBTableBet3
        {
            get => normalBBet3Table;
            set => normalBBet3Table = value;
        }

        public NewFlagDatabaseSet BigTableBet1
        {
            get => bigBet1Table;
            set => bigBet1Table = value;
        }

        public NewFlagDatabaseSet BigTableBet2
        {
            get => bigBet2Table;
            set => bigBet2Table = value;
        }

        public NewFlagDatabaseSet BigTableBet3
        {
            get => bigBet3Table;
            set => bigBet3Table = value;
        }

        public NewFlagDatabaseSet JacTable
        {
            get => jacTable;
            set => jacTable = value;
        }
    }

    // 各テーブル、ベット枚数ごとのフラグデータ
    [Serializable]
    public class NewFlagDatabaseSet
    {
        [SerializeField] List<NewFlagData> flagDataList;

        public List<NewFlagData> FlagDataList { get => flagDataList;}

        public NewFlagDatabaseSet(StreamReader loadedData)
        {
            flagDataList = new List<NewFlagData>();
            // 全ての行を読み込む
            while (loadedData.Peek() != -1)
            {
                flagDataList.Add(new NewFlagData(loadedData));
            }
        }
    }

    // フラグデータ
    [Serializable]
    public class NewFlagData
    {
        // 配列要素の識別用
        public enum FlagIndexSerialize
        {
            FlagID = 0,
            FlagProbabilityStart = 1 << 0,
        }

        [SerializeField] private FlagID flagID;       //フラグID
        [SerializeField] private List<int> flagCountBySetting;  // 設定ごとのフラグ数

        public FlagID FlagID {  get { return flagID; } }
        public List<int> FlagCountBySetting {  get { return flagCountBySetting; } }

        public NewFlagData(StreamReader loadedData)
        {
            flagCountBySetting = new List<int>();
            string[] values = loadedData.ReadLine().Split(',');
            int index = 0;

            foreach (string s in values)
            {
                // 最初の数値はフラグIDを登録する
                if(index == (int)FlagIndexSerialize.FlagID)
                {
                    flagID = (FlagID)Enum.Parse(typeof(FlagID), s);
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

