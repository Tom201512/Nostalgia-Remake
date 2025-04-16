using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datass
{
    // フラグデータベース
    public class FlagDatabase : ScriptableObject
    {
        // 通常時A(低確率)
        [SerializeField] private FlagDataSets normalATable;
        // 通常時B(高確率)
        [SerializeField] private FlagDataSets normalBTable;
        // 小役ゲーム中
        [SerializeField] private FlagDataSets bigTable;
        // JAC時のはずれ
        [SerializeField] private float jacNonePoss;


        public FlagDataSets NormalATable { get { return normalATable; } }
        public FlagDataSets NormalBTable { get { return normalBTable; } }
        public FlagDataSets BigTable { get { return bigTable; } }
        public float JacNonePoss { get { return jacNonePoss; } }

        public void SetNormalATable(FlagDataSets normalATable) => this.normalATable = normalATable;
        public void SetNormalBTable(FlagDataSets normalBTable) => this.normalBTable = normalBTable;
        public void SetBIGTable(FlagDataSets bigTable) => this.bigTable = bigTable;
        public void SetJACNonePoss(float jacNonePoss) => this.jacNonePoss = jacNonePoss;
    }

    // 設定ごとにまとめたフラグ確率
    [Serializable]
    public class FlagDataSets
    {
        [SerializeField] List<FlagDataBySetting> flagDataBySettings;
        public List<FlagDataBySetting> FlagDataBySettings { get { return flagDataBySettings; } }

        public FlagDataSets(StringReader loadedData)
        {
            flagDataBySettings = new List<FlagDataBySetting>();
            // 全ての行を読み込む
            while (loadedData.Peek() != -1)
            {
                flagDataBySettings.Add(new FlagDataBySetting(loadedData));
            }
            Debug.Log(flagDataBySettings.Count);
        }
    }

    // 設定ごとの確率
    [Serializable]
    public class FlagDataBySetting
    {
        // var
        // フラグ確率
        [SerializeField] private float[] flagTable;

        public float[] FlagTable { get { return flagTable; } }

        public FlagDataBySetting(StringReader loadedData)
        {
            string[] values = loadedData.ReadLine().Split(',');
            string buffer = "";
            // 配列に変換
            flagTable = Array.ConvertAll(values, float.Parse);

            foreach(float f in flagTable)
            {
                buffer += f + ",";
            }
            Debug.Log("Flag:" + buffer);
        }
    }
}

