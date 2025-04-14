using System;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // リールデータベース作成
    public class ReelDatabase : ScriptableObject
    {
        [SerializeField] private byte[] array;
        //SerializeField] private ReelConditionsData[] conditions;
        //[SerializeField] private ReelTableData[] tables;

        public byte[] Array { get { return array; } set { array = value;} }
        //public ReelConditionsData[] Conditions { get; }
        //public ReelTableData[] Tables { get; }
    }
}
