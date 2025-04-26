using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // ���[���f�[�^�x�[�X
    public class ReelDatabase : ScriptableObject
    {
        // var
        // ���[���z��
        [SerializeField] private byte[] array;
        // ���[������
        [SerializeField] private List<ReelConditionsData> conditions;
        // �f�B���C�e�[�u��
        [SerializeField] private List<ReelTableData> tables;

        public byte[] Array { get { return array; } }
        public List<ReelConditionsData> Conditions { get { return conditions; }}
        public List<ReelTableData> Tables { get { return tables; } }

        // func
        public void SetArray(byte[] array) => this.array = array;
        public void SetConditions(List<ReelConditionsData> conditions) => this.conditions = conditions;
        public void SetTables(List<ReelTableData> tables) => this.tables = tables;
    }

    // ���[�������e�[�u��
    [Serializable]
    public class ReelConditionsData
    {
        // const
        // ������ǂݍ��ރo�C�g��
        public const int ConditionMaxRead = 5;
        // ����~���[����~�ʒu��ǂݍ��ރo�C�g��
        public const int FirstReelPosMaxRead = ConditionMaxRead + 10;
        // �e�[�u��ID�̓ǂݍ��ރo�C�g��
        public const int ReelTableIDMaxRead = FirstReelPosMaxRead + 1;
        // ������ǂݍ��ލۂɂ��炷�r�b�g��
        public const int ConditionBitOffset = 4;

        // enum
        // �����̃V���A���C�Y
        public enum ConditionID { Flag, FirstPush, Bonus, Bet, Random }

        // var
        // �t���OID, ����~, �{�[�i�X, �x�b�g����, �����_������̏��œǂݍ���
        [SerializeField] private int mainConditions;
        // ����~�������[���̈ʒu(��������ƂɃe�[�u���̕ύX������)
        [SerializeField] private int firstReelPosition;
        // �g�p����e�[�u���ԍ�
        [SerializeField] byte reelTableNumber;

        public int MainConditions { get { return mainConditions; } set { mainConditions = value; } }
        public int FirstReelPosition { get { return firstReelPosition; } set { firstReelPosition = value; } }
        public byte ReelTableNumber { get { return reelTableNumber; } set { reelTableNumber = value; } }

        // �R���X�g���N�^
        public ReelConditionsData(StringReader buffer)
        {
            string[] values = buffer.ReadLine().Split(',');

            int indexNum = 0;
            foreach (string value in values)
            {
                // ���C������(16�i���œǂݍ���int�^�ň��k)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    mainConditions += Convert.ToInt32(value) * offset;
                }

                // ��ꃊ�[����~
                else if (indexNum < FirstReelPosMaxRead)
                {
                    firstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
                }

                // �e�[�u��ID�ǂݍ���
                else if (indexNum < ReelTableIDMaxRead)
                {
                    reelTableNumber = Convert.ToByte(value);
                }

                // �Ō�̕����͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum += 1;
            }
        }

        public ReelConditionsData(StreamReader buffer)
        {
            string[] values = buffer.ReadLine().Split(',');

            int indexNum = 0;
            foreach (string value in values)
            {
                // ���C������(16�i���œǂݍ���int�^�ň��k)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    mainConditions += Convert.ToInt32(value) * offset;
                }

                // ��ꃊ�[����~
                else if (indexNum < FirstReelPosMaxRead)
                {
                    firstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
                }

                // �e�[�u��ID�ǂݍ���
                else if (indexNum < ReelTableIDMaxRead)
                {
                    reelTableNumber = Convert.ToByte(value);
                }

                // �Ō�̕����͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum += 1;
            }
        }

        // func
        // �e�����̐��l��Ԃ�
        public int GetConditionData(int conditionID) => ((MainConditions >> ConditionBitOffset * conditionID) & 0xF);

        // �e������int�ɂ���
        public static int ConvertConditionData(int flagID, int firstPush, int bonus, int bet, int random)
        {
            // 16�i���̃f�[�^�֕ύX
            int conditions = 0;
            // �z��ɂ���
            int[] conditionArray = { flagID, firstPush, bonus, bet, random };

            for (int i = 0; i < (int)ConditionID.Random; i++)
            {
                conditions |= conditionArray[i] << ConditionBitOffset * i;
            }
            return conditions;
        }

        // �f�[�^���r�b�g�ɂ���
        public static int ConvertToArrayBit(int data)
        {
            // 0�̎��͕ϊ����Ȃ�
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }
    }

    // ���[������e�[�u��
    [Serializable]
    public class ReelTableData
    {
        // var
        // ���[������e�[�u��(�f�B���C���i�[����)
        [SerializeField] private List<byte> tableData;
        public List<byte> TableData { get { return tableData; } }

        // �R���X�g���N�^
        public ReelTableData(StringReader LoadedData)
        {
            tableData = new List<byte>();

            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;
            // �f�o�b�O�p
            string debugBuffer = "";

            // �ǂݍ��݊J�n
            foreach (string value in values)
            {
                Debug.Log(value);
                // ���[���f�[�^��ǂݍ���
                if (indexNum < ReelData.MaxReelArray)
                {
                    tableData.Add(Convert.ToByte(value));
                    debugBuffer += tableData[indexNum];
                }

                // �Ō�̈�s�͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum++;
            }

            Debug.Log("Array:" + debugBuffer);
        }

        // �R���X�g���N�^
        public ReelTableData(StreamReader LoadedData)
        {
            tableData = new List<byte>();

            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;
            // �f�o�b�O�p
            string debugBuffer = "";

            // �ǂݍ��݊J�n
            foreach (string value in values)
            {
                Debug.Log(value);
                // ���[���f�[�^��ǂݍ���
                if (indexNum < ReelData.MaxReelArray)
                {
                    tableData.Add(Convert.ToByte(value));
                    debugBuffer += tableData[indexNum];
                }

                // �Ō�̈�s�͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum++;
            }

            Debug.Log("Array:" + debugBuffer);
        }
    }
}
