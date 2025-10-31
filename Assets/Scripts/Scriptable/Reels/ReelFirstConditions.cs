using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelFirstConditions : ReelBaseData
    {
        // const
        // ����~�̒�~�����ǂݍ��݈ʒu
        private const int FirstPushReadPos = ConditionMaxRead + 1;
        // ����~��TID�ǂݍ��݈ʒu
        private const int FirstPushTIDPos = FirstPushReadPos + 1;
        // ����~��CID�ǂݍ��݈ʒu
        private const int FirstPushCIDPos = FirstPushTIDPos + 1;

        // var
        // ����~�̒�~����
        [SerializeField] private int firstStopPos;

        // �R���X�g���N�^
        public ReelFirstConditions(StreamReader sReader)
        {
            string[] values = GetDataFromStream(sReader);
            int indexNum = 0;

            foreach (string value in values)
            {
                // ���C������(16�i���œǂݍ���int�^�ň��k)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // ��ꃊ�[����~�ʒu(���[�܂œǂݍ���)
                else if (indexNum < FirstPushReadPos)
                {
                    if (value != "ANY")
                    {
                        string[] stopPosData = value.Trim('"').Split(",");
                        foreach (string stop in stopPosData)
                        {
                            firstStopPos += ConvertToArrayBit(Convert.ToInt32(stop));
                        }
                    }
                    else
                    {
                        firstStopPos = 0;
                    }
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

                // �Ō�̕����͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum += 1;
            }
            //Debug.Log("MainCondition:" + MainConditions);
            //Debug.Log("FirstStopPos:" + firstStopPos);
            //Debug.Log("TID:" + TID);
            //Debug.Log("CID:" + CID);

            //Debug.Log("First Push Load Done");
        }

        // �����`�F�b�N
        public bool CheckFirstReelCondition(int flagID, int bonus, int bet, int random, int pushedPos)
        {
            // ���C�������`�F�b�N
            if(CheckMainCondition(flagID, bonus, bet, random))
            {
                // ����~�̏�������v���邩�`�F�b�N�B0��ANY
                // ����~�̐��l���r�b�g���Z�Ŕ�r�ł���悤�ɂ���
                //Debug.Log("Pushed:" + pushedPos);
                int checkValue = 1 << pushedPos + 1;
                //Debug.Log("check:" + checkValue);
                //Debug.Log("FirstPos:" + firstStopPos);

                // ��~�������m�F
                if (firstStopPos == 0 || ((checkValue & firstStopPos) != 0))
                {
                    //Debug.Log("Stop Pos has match with condition");
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
        public enum ConditionID { Flag, Bonus, Bet, Random }

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
        protected int GetConditionData(int condition, int conditionID) => ((condition >> ConditionBitOffset * conditionID) & 0xF);

        // ���C�������������Ă��邩�`�F�b�N
        protected bool CheckMainCondition(int flagID, int bonus, int bet, int random)
        {
            //Debug.Log("MainCondition:" + flagID + "," + bonus + "," + bet + "," + random);
            // �f�[�^�������ɂ���
            int[] conditions = new int[]
            {
                flagID,
                bonus,
                bet,
                random,
            };

            //Debug.Log("ConditionData:" + MainConditions);

            // ���C�������`�F�b�N
            for (int i = 0; i < ConditionMaxRead; i++)
            {
                int checkData = GetConditionData(MainConditions, i);
                //Debug.Log("checkData:" + checkData);

                // �t���OID�ȊO�̏�����0���������ꍇ�̓p�X����
                if (i != (int)ConditionID.Flag && checkData == 0)
                {
                    //Debug.Log("No condition");
                    continue;
                }
                // �{�[�i�X������3�Ȃ炢���ꂩ�̃{�[�i�X���������Ă���΃p�X
                else if (i == (int)ConditionID.Bonus && checkData == BonusAnyValueID &&
                    bonus != (int)BonusTypeID.BonusNone)
                {
                    //Debug.Log(bonus + "ANY BONUS");
                    continue;
                }
                // ����ȊO�͎󂯎�������̂Ə������������m�F����
                else if (conditions[i] != checkData)
                {
                    //Debug.Log("Condition not match");
                    return false;
                }
            }
            //Debug.Log("Condition Passed");
            return true;
        }

        // �f�[�^���r�b�g�ɂ���
        protected int ConvertToArrayBit(int data)
        {
            // 0�̎��͕ϊ����Ȃ�
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }

        // �f�[�^�ǂݍ���
        protected string[] GetDataFromStream(StreamReader sReader)
        {
            // �J���}����̃f�[�^�����邽�߁A�Ǝ��Ƀp�[�T�[���쐬
            // �ǂݍ��񂾃f�[�^�̃e�L�X�g
            string loadedText = sReader.ReadLine();
            // �f�[�^�ɓ����o�b�t�@
            string bufferText = "";
            // �_�u���N�H�[�e�[�V�����Ńp�[�X�����f�[�^
            string parseText = "";
            List<string> dataBuffer = new List<string>();

            // �_�u���N�I�[�e�[�V�����𔭌�������
            bool findDoubleQuartation = false;

            foreach(char c in loadedText)
            {
                // �󔒈ȊO�ǂݍ���
                if(c != ' ')
                {
                    // �_�u���N�H�[�e�[�V�����Ȃ�ʃe�L�X�g�Ɉڂ�(�J���}���������)
                    if(c == '"')
                    {
                        findDoubleQuartation = !findDoubleQuartation;
                        parseText += c;
                    }
                    else 
                    {
                        if(findDoubleQuartation)
                        {
                            parseText += c;
                        }
                        // �_�u���N�H�[�e�[�V������ǂݍ��񂾎��̓J���}���ǂ܂Ȃ�
                        else
                        {
                            // �J���}��ǂݍ��񂾂�o�b�t�@�e�L�X�g����荞��
                            if(c == ',')
                            {
                                // �p�[�X�������͂�����ꍇ�͂��̕��͂��o�b�t�@�Ɏ�荞��
                                if(parseText != "")
                                {
                                    dataBuffer.Add(parseText);
                                    parseText = "";
                                }
                                else
                                {
                                    dataBuffer.Add(bufferText);
                                    bufferText = "";
                                }
                            }
                            // �J���}��ǂݍ��ނ܂ł̓o�b�t�@�Ƀe�L�X�g���l�߂�
                            else
                            {
                                bufferText += c;
                            }
                        }
                    }
                }
            }

            // �Ō�ɓǂݍ��񂾃e�L�X�g��ǂݍ���
            dataBuffer.Add(bufferText);

            string finalData = String.Join(",", dataBuffer);
            return dataBuffer.ToArray();
        }
    }
}
