using System;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelSecondData : ReelBaseData
    {
        // const
        // ����~�������[��ID��ǂݍ��ވʒu
        private const int FirstPushedReelIDPos = ConditionMaxRead + 1;
        // ����~�������[����CID��ǂݍ��ވʒu
        private const int FirstPushedCIDPos = FirstPushedReelIDPos + 1;
        // ����~�̒�~�ʒu�����ǂݍ��݈ʒu
        private const int SecondPushReadPos = FirstPushedCIDPos + 1;
        // ����~��TID�ǂݍ��݈ʒu
        private const int SecondPushTIDPos = SecondPushReadPos + 1;
        // ����~��CID�ǂݍ��݈ʒu
        private const int SecondPushCIDPos = SecondPushTIDPos + 1;

        // var
        // ����~�̒�~����

        // ����~�������[����ID
        [SerializeField] private byte firstStopReelID;
        // ����~�������[����CID
        [SerializeField] private byte firstStopCID;
        // ����~�̒�~����
        [SerializeField] private int secondStopPos;

        // �R���X�g���N�^
        public ReelSecondData(StreamReader sReader)
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

                // ��ꂵ�����[����ID�ǂݍ���
                else if (indexNum < FirstPushedReelIDPos)
                {
                    firstStopReelID = Convert.ToByte(value);
                }

                // ��ꂵ�����[����TID�ǂݍ���
                else if (indexNum < FirstPushedCIDPos)
                {
                    firstStopCID = Convert.ToByte(value);
                }

                // ��񃊁[����~�ʒu(���[�܂œǂݍ���)
                else if (indexNum < SecondPushReadPos)
                {
                    if (value != "ANY")
                    {
                        string[] stopPosData = value.Trim('"').Split(",");
                        foreach (string stop in stopPosData)
                        {
                            secondStopPos += ConvertToArrayBit(Convert.ToInt32(stop));
                        }
                    }
                    else
                    {
                        secondStopPos = 0;
                    }
                    //Debug.Log("SecondStopPos:" + secondStopPos);
                }

                // TID�ǂݍ���
                else if (indexNum < SecondPushTIDPos)
                {
                    TID = Convert.ToByte(value);
                }

                // CID�ǂݍ���
                else if (indexNum < SecondPushCIDPos)
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
            //Debug.Log("FirstPushedReelID:" + firstStopReelID);
            //Debug.Log("FirstPushedCID:" + firstStopCID);
            //Debug.Log("SecondStopPos:" + secondStopPos);
            //Debug.Log("TID:" + TID);
            //Debug.Log("CID:" + CID);

            //Debug.Log("Second Push Load Done");
        }

        // �����`�F�b�N
        public bool CheckSecondReelCondition(int flagID, int bonus, int bet, int random, ReelID firstStopReelID, int firstStopCID, int pushedPos)
        {
            // ���C�������`�F�b�N
            if (CheckMainCondition(flagID, bonus, bet, random))
            {
                // ����~�̃��[��ID��CID����v���邩�`�F�b�N 

                //Debug.Log("FirstPush:" + firstStopReelID + "CID:" + firstStopCID);

                if (SecondReelCIDCheck(firstStopReelID, firstStopCID))
                {
                    //Debug.Log("Pushed:" + pushedPos);
                    // ����~�̏�������v���邩�`�F�b�N�B0��ANY
                    // ����~�̐��l���r�b�g���Z�Ŕ�r�ł���悤�ɂ���
                    int checkValue = 1 << pushedPos + 1;
                    //Debug.Log("check:" + checkValue);
                    //Debug.Log("SecondPushPos:" + secondStopPos);

                    // ��~�������m�F
                    if (secondStopPos == 0 || ((checkValue & secondStopPos) != 0))
                    {
                        //Debug.Log("Stop Pos has match with condition");
                        // ������v
                        return true;
                    }
                }
            }
            return false;
        }

        // CID�`�F�b�N(����~�p)
        private bool SecondReelCIDCheck(ReelID firstStopReelID, int firstStopCID)
        {
            //Debug.Log("FirstStopReel Condition: " + this.firstStopReelID);
            //Debug.Log("FirstStop Check: " + (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID));

            // ����~���[��ID�̏��������������`�F�b�N(0��ANY)
            if (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID)
            {
                // ����~��CID���`�F�b�N
                //Debug.Log("CID Check" + (this.firstStopCID == 0 || firstStopCID == this.firstStopCID));
                return this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }
            else
            {
                return false;
            }
        }
    }
}