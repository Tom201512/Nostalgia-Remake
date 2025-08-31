using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelFirstData : ReelBaseData
    {
        // const
        // ����~��TID�ǂݍ��݈ʒu
        private const int FirstPushTIDPos = ConditionMaxRead + 1;
        // ����~��CID�ǂݍ��݈ʒu
        private const int FirstPushCIDPos = FirstPushTIDPos + 1;

        // var
        // ����~�̒�~����
        private int firstStopPos;

        // �R���X�g���N�^
        public ReelFirstData(StreamReader sReader)
        {
            string[] values = GetDataFromStream(sReader);

            int indexNum = 0;
            Debug.Log("Count:" + values.Length);

            foreach (string value in values)
            {
                Debug.Log(value);
                // ���C������(16�i���œǂݍ���int�^�ň��k)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // TID�ǂݍ���
                else if (indexNum < FirstPushTIDPos)
                {
                    TID = Convert.ToByte(value);
                }

                // CID�ǂݍ���
                else if (indexNum < FirstPushCIDPos)
                {
                    CID = Convert.ToByte(value);
                }

                // ��ꃊ�[����~�ʒu(���[�܂œǂݍ���)
                else if (indexNum < values.Length - 1)
                {
                    if(value != "ANY")
                    {
                        firstStopPos = ConvertToArrayBit(Convert.ToInt32(value));
                    }
                }

                // �Ō�̕����͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum += 1;
            }
            Debug.Log("Load Done");
        }

        // �����`�F�b�N
        public bool CheckFirstReelCondition(int flagID, int bet, int bonus, int random, int firstPushPos)
        {
            // ���C�������`�F�b�N
            if(CheckMainCondition(flagID, bet, bonus, random))
            {
                // ����~�̏�������v���邩�`�F�b�N�B0��ANY
                // ����~�̐��l���r�b�g���Z�Ŕ�r�ł���悤�ɂ���
                int checkValue = 1 << firstPushPos + 1;
                // �����ꂩ�̏�����
                if (firstStopPos == 0 || (checkValue & firstStopPos) != 0)
                {
                    // ������v
                    return true;
                }
            }
            return false;
        }
    }

    // ��{�f�[�^
    [Serializable]
    public class ReelBaseData
    {
        // const
        // ������ǂݍ��ރo�C�g��
        public const int ConditionMaxRead = 4;
        // ������ǂݍ��ލۂɂ��炷�r�b�g��
        public const int ConditionBitOffset = 4;

        // �����ꂩ�̃{�[�i�X�������Ă����������������
        public const int BonusAnyValueID = 3;

        // �����̃V���A���C�Y
        public enum ConditionID { Flag, Bet, Bonus, Random }

        // �t���OID, �{�[�i�X, �x�b�g����, �����_������̏��œǂݍ���
        [SerializeField] private int mainConditon;
        // �g�p����TID(�e�[�u��ID)
        [SerializeField] private byte tid;
        // �g�p����CID(�g�ݍ��킹ID)
        [SerializeField] private byte cid;

        public int MainConditions { get { return mainConditon; } protected set { mainConditon = value; } }
        public byte TID { get { return tid; } protected set { tid = value; } }
        public byte CID { get { return cid; } protected set { cid = value; } }

        // �e�����̐��l��Ԃ�
        public static int GetConditionData(int condition, int conditionID) => ((condition >> ConditionBitOffset * conditionID) & 0xF);

        // ���C�������������Ă��邩�`�F�b�N
        public bool CheckMainCondition(int flagID, int bet, int bonus, int random)
        {
            // �f�[�^�������ɂ���
            int[] conditions = new int[]
            {
                flagID,
                bet,
                bonus,
                random,
            };

            // ���C�������`�F�b�N
            for (int i = 0; i < ConditionMaxRead; i++)
            {
                //Debug.Log("Condition1:" + GetConditionData(condition, i));
                //Debug.Log("Condition2:" + GetConditionData(data.MainConditions, i));
                // �t���OID�ȊO�̏�����0���������ꍇ�̓p�X����
                if (i != (int)ConditionID.Flag && conditions[i] == 0)
                {
                    continue;
                }
                // �{�[�i�X������3�Ȃ炢���ꂩ�̃{�[�i�X���������Ă���΃p�X
                else if (i == (int)ConditionID.Bonus && conditions[i] == BonusAnyValueID &&
                    bonus != (int)BonusTypeID.BonusNone)
                {
                    //Debug.Log(bonus + "ANY BONUS");
                    continue;
                }

                // ����ȊO�͎󂯎�������̂Ə������������m�F����
                else if (conditions[i] != GetConditionData(MainConditions, i))
                {
                    return false;
                }
            }

            return true;
        }

        // �f�[�^���r�b�g�ɂ���
        public int ConvertToArrayBit(int data)
        {
            // 0�̎��͕ϊ����Ȃ�
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }

        // �f�[�^�ǂݍ���
        public string[] GetDataFromStream(StreamReader sReader)
        {
            // �J���}����̃f�[�^�����邽�߁A�Ǝ��Ƀp�[�T�[���쐬
            string dataText = sReader.ReadLine();
            Debug.Log(dataText);
            string dataBuffer = "";

            // �_�u���N�I�[�e�[�V�����𔭌�������
            bool findDoubleQuartation = false;

            int index = 0;
            foreach(char c in dataText)
            {
                // �󔒈ȊO�ǂݍ���
                if(c != ' ')
                {
                    dataBuffer += c;
                }

                index += 1;
            }

            Debug.Log("FinalData:" + dataBuffer);
            return dataBuffer.Split(",");
        }
    }
}
