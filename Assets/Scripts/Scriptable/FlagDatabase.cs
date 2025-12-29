using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // フラグデータベース
    public class FlagDatabase : ScriptableObject
    {
        [SerializeField] FlagDataSets normalATable;         // 通常時A(低確率)
        [SerializeField] FlagDataSets normalBTable;         // 通常時B(高確率)
        [SerializeField] FlagDataSets bigTable;             // 小役ゲーム中
        [SerializeField] float jacNonePoss;                 // JAC時のはずれ

        // プロパティ
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
            get => jacNonePoss;
            set => jacNonePoss = value;
        }
    }

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

