using ReelSpinGame_Reels;
using ReelSpinGame_Rules;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Lots.Flag.FlagLots;

public class ReelTableManager
{
    // ���[���e�[�u���Ǘ��p

    // const

    // ���[�������f�[�^
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
        // �e�[�u���g�p������int�^�f�[�^�Ŋi�[���A1�o�C�g(8bit)���ƂɃf�[�^�𕪂���
        // �t���OID, ����~, �{�[�i�X, �x�b�g����, �����_������̏��œǂݍ���
        public int MainConditions { get; private set; }

        // ����~�������[���̈ʒu(��������ƂɃe�[�u���̕ύX������)
        public int FirstReelPosition { get; private set; }

        // �g�p����e�[�u���ԍ�
        public byte ReelTableNumber { get; private set; }

        // �R���X�g���N�^
        public ReelConditionsData(StreamReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // �ǂݍ��݊J�n(�v�f�ԍ������ƂɃf�[�^��ǂݍ���
            foreach (string value in values)
            {
                // ���C������(16�i���œǂݍ���int�^�ň��k)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // ��ꃊ�[����~
                else if (indexNum < FirstReelPosMaxRead)
                {
                    FirstReelPosition += OriginalMathmatics.ConvertToArrayBit(Convert.ToInt32(value));
                }

                // �e�[�u��ID�ǂݍ���
                else if (indexNum < ReelTableIDMaxRead)
                {
                    ReelTableNumber = Convert.ToByte(value);
                }

                // �Ō�̈�s�͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum++;
            }

            // �f�o�b�O�p
            string ConditionDebug = "";

            for(int i = 0; i < 5; i++)
            {
                ConditionDebug += GetConditionData(i).ToString() + ",";
            }
            Debug.Log("Condition:" + MainConditions + "Details:" + ConditionDebug + "FirstReel:" + FirstReelPosition + "ReelTableNum" + ReelTableNumber);
        }

        // func
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

        // �e�����̐��l��Ԃ�
        public int GetConditionData(int conditionID) => ((MainConditions >> ConditionBitOffset * conditionID) & 0xF);
    }

    // ���[������e�[�u��
    public class ReelTableData
    {
        // ���[������e�[�u��(�f�B���C���i�[����)
        public byte[] TableData { get; private set; } = new byte[ReelData.MaxReelArray];

        public ReelTableData(StreamReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // �f�o�b�O�p
            string debugBuffer = "";

            // �ǂݍ��݊J�n
            foreach (string value in values)
            {
                // ���[���f�[�^��ǂݍ���
                if (indexNum < ReelData.MaxReelArray)
                {
                    TableData[indexNum] = Convert.ToByte(value);
                    debugBuffer += TableData[indexNum];
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

    // var
    private List<List<ReelConditionsData>> reelConditions;
    private List<List<ReelTableData>> reelDelayTables;

    // �R���X�g���N�^
    public ReelTableManager(List<StreamReader> conditions, List<StreamReader> tables)
    {
        // ���X�g�쐬
        reelConditions = new List<List<ReelConditionsData>>();
        reelDelayTables = new List<List<ReelTableData>>();

        // ���[�������ƃe�[�u���̐����������`�F�b�N
        if (conditions.Count != tables.Count)
        {
            throw new Exception("Condition counts and table counts doesn't match");
        }
        
        // �����̓ǂݍ���
        for(int i = 0; i < conditions.Count; i++)
        {
            // �����ǂݍ���
            reelConditions.Add(new List<ReelConditionsData>());

            while (!conditions[i].EndOfStream)
            {
                reelConditions[i].Add(new ReelConditionsData(conditions[i]));
            }

            Debug.Log("Condition:" + i + "Read done");
        }

        Debug.Log("ReelConditions reading done");

        for (int i = 0; i < tables.Count; i++)
        {
            // �����ǂݍ���
            reelDelayTables.Add(new List<ReelTableData>());

            while (!tables[i].EndOfStream)
            {
                reelDelayTables[i].Add(new ReelTableData(tables[i]));
            }

            Debug.Log("Condition:" + i + "Read done");
        }

        Debug.Log("ReelTable reading done");
    }

    // func
    // �w�肵�����[���̃f�B���C(�X�x��)��Ԃ�
    public byte GetDelayFromTable(ReelManager.ReelID reelID, int flagID, int firstPush,
        int bonus, int bet, int random, int firstPushPos)
    {
        int condition = ReelConditionsData.ConvertConditionData(flagID, firstPush, bonus, bet, random);
        int[] orderToCheck = {flagID, firstPush, bonus, bet, random };
        // ���C�������������Ă��邩����

        foreach (ReelConditionsData data in reelConditions[(int)reelID])
        {
            for (int i = 0; i < orderToCheck.Length; i++)
            {
                // �t���OID�ȊO�̏�����0�Ȃ�p�X
                if (i == (int)ReelConditionsData.ConditionID.Flag && data.GetConditionData(i) == 0)
                {
                    continue;
                }
                else if (orderToCheck[i] != data.GetConditionData(i))
                {
                    break;
                }
            }

            Debug.Log("All conditions are met");
            // ���͑���~�̃��[����~�ʒu������
        }
        return 0;
    }
}
