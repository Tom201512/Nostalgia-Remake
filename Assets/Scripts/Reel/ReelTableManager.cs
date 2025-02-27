using ReelSpinGame_Reels;
using ReelSpinGame_Rules;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        public enum Conditions { C_FLAG, C_FIRST, C_BONUS, C_BET, C_RANDOM }

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

            Debug.Log(values.Length);

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

            Debug.Log("Condition:" + MainConditions + "FirstReel:" + FirstReelPosition + "ReelTableNum" + ReelTableNumber);
        }
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
    public List<ReelConditionsData> ReelConditionL { get; private set; }
    public List<ReelConditionsData> ReelConditionM { get; private set; }
    public List<ReelConditionsData> ReelConditionR { get; private set; }

    public List<ReelTableData> ReelTableL { get; private set; }
    public List<ReelTableData> ReelTableM { get; private set; }
    public List<ReelTableData> ReelTableR { get; private set; }

    // �R���X�g���N�^
    public ReelTableManager(StreamReader conditionL, StreamReader conditionM, StreamReader conditionR,
        StreamReader reelTableL, StreamReader reelTableM, StreamReader reelTableR)
    {
        // ���X�g�쐬
        ReelConditionL = new List<ReelConditionsData>();
        ReelConditionM = new List<ReelConditionsData>();
        ReelConditionR = new List<ReelConditionsData>();

        ReelTableL = new List<ReelTableData>();
        ReelTableM = new List<ReelTableData>();
        ReelTableR = new List<ReelTableData>();

        // �����ǂݍ���
        while (!conditionL.EndOfStream)
        {
            ReelConditionL.Add(new ReelConditionsData(conditionL));
        }

        Debug.Log("ReelConditions reading done");

        // �e�[�u���ǂݍ���
        while (!reelTableL.EndOfStream)
        {
            ReelTableL.Add(new ReelTableData(reelTableL));
        }

        Debug.Log("ReelTable reading done");
    }
}
