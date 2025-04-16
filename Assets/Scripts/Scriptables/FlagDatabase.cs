using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datass
{
    // �t���O�f�[�^�x�[�X
    public class FlagDatabase : ScriptableObject
    {
        // �ʏ펞A(��m��)
        [SerializeField] private FlagDataSets normalATable;
        // �ʏ펞B(���m��)
        [SerializeField] private FlagDataSets normalBTable;
        // �����Q�[����
        [SerializeField] private FlagDataSets bigTable;
        // JAC���̂͂���
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

    // �ݒ育�Ƃɂ܂Ƃ߂��t���O�m��
    [Serializable]
    public class FlagDataSets
    {
        [SerializeField] List<FlagDataBySetting> flagDataBySettings;
        public List<FlagDataBySetting> FlagDataBySettings { get { return flagDataBySettings; } }

        public FlagDataSets(StringReader loadedData)
        {
            flagDataBySettings = new List<FlagDataBySetting>();
            // �S�Ă̍s��ǂݍ���
            while (loadedData.Peek() != -1)
            {
                flagDataBySettings.Add(new FlagDataBySetting(loadedData));
            }
            Debug.Log(flagDataBySettings.Count);
        }
    }

    // �ݒ育�Ƃ̊m��
    [Serializable]
    public class FlagDataBySetting
    {
        // var
        // �t���O�m��
        [SerializeField] private float[] flagTable;

        public float[] FlagTable { get { return flagTable; } }

        public FlagDataBySetting(StringReader loadedData)
        {
            string[] values = loadedData.ReadLine().Split(',');
            string buffer = "";
            // �z��ɕϊ�
            flagTable = Array.ConvertAll(values, float.Parse);

            foreach(float f in flagTable)
            {
                buffer += f + ",";
            }
            Debug.Log("Flag:" + buffer);
        }
    }
}

