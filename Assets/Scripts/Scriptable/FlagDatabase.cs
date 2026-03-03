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
        }

        public NewFlagDatabaseSet NormalATableBet2
        {
            get => normalABet3Table;
        }

        public NewFlagDatabaseSet NormalATableBet3
        {
            get => normalABet3Table;
        }

        public NewFlagDatabaseSet NormalBTableBet1
        {
            get => normalBBet1Table;
        }

        public NewFlagDatabaseSet NormalBTableBet2
        {
            get => normalBBet2Table;
        }

        public NewFlagDatabaseSet NormalBTableBet3
        {
            get => normalBBet3Table;
        }

        public NewFlagDatabaseSet BigTableBet1
        {
            get => bigBet1Table;
        }

        public NewFlagDatabaseSet BigTableBet2
        {
            get => bigBet2Table;
        }

        public NewFlagDatabaseSet BigTableBet3
        {
            get => bigBet3Table;
        }

        public NewFlagDatabaseSet JacTable
        {
            get => jacTable;
        }

        //Deleted
        [SerializeField] FlagDataSets normalATable;         // 通常時A(低確率)
        [SerializeField] FlagDataSets normalBTable;         // 通常時B(高確率)
        [SerializeField] FlagDataSets bigTable;             // 小役ゲーム中
        [SerializeField] float jacNoneProbability;          // JAC時のはずれ

        public FlagDataSets NormalATable
        {
            get => normalATable;
            set => normalATable = value;
        }

        public FlagDataSets NormalBTable
        {
            get => normalBTable;
            set => normalBTable = value;
        }

        public FlagDataSets BigTable
        {
            get => bigTable;
            set => bigTable = value;
        }

        public float JacNonePoss
        {
            get => jacNoneProbability;
            set => jacNoneProbability = value;
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
                if(index == 0)
                {
                    Debug.Log("FlagID:" + s);
                    flagID = (FlagID)Enum.Parse(typeof(FlagID), s);
                }
                else
                {
                    Debug.Log(Convert.ToInt32(s));
                    flagCountBySetting.Add(Convert.ToInt32(s));
                }

                index += 1;
            }
        }
    }

    //Deleted
    // 設定ごとにまとめたフラグ確率
    [Serializable]
    public class FlagDataSets
    {
        [SerializeField] List<FlagDataBySetting> flagDataBySettings;
        public List<FlagDataBySetting> FlagDataBySettings { get { return flagDataBySettings; } }

        public FlagDataSets(StreamReader loadedData)
        {
            flagDataBySettings = new List<FlagDataBySetting>();
            // 全ての行を読み込む
            while (loadedData.Peek() != -1)
            {
                flagDataBySettings.Add(new FlagDataBySetting(loadedData));
            }
        }
    }

    // 設定ごとの確率
    [Serializable]
    public class FlagDataBySetting
    {
        // フラグ確率
        [SerializeField] private float[] flagTable;
        public float[] FlagTable { get { return flagTable; } }

        public FlagDataBySetting(StreamReader loadedData)
        {
            string[] values = loadedData.ReadLine().Split(',');
            string buffer = "";
            // 配列に変換
            flagTable = Array.ConvertAll(values, float.Parse);

            foreach (float f in flagTable)
            {
                buffer += f + ",";
            }
        }
    }
}

