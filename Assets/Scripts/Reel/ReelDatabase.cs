using System;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // ���[���f�[�^�x�[�X�쐬
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
