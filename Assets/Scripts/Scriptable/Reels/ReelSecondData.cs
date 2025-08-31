using System;
using System.IO;
using UnityEngine;

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
        // ����~��TID�ǂݍ��݈ʒu
        private const int SecondPushTIDPos = FirstPushedCIDPos + 1;
        // ����~��CID�ǂݍ��݈ʒu
        private const int SecondPushCIDPos = SecondPushTIDPos + 1;

        // var
        // ����~�̒�~����

        // ����~�������[����ID
        [SerializeField] private byte firstStopReelID;
        // ����~�������[����CID
        [SerializeField] private byte firstStopCID;
        // ����~�������[����CID
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
                    secondStopPos = Convert.ToByte(value);
                }

                // ��ꂵ�����[����TID�ǂݍ���
                else if (indexNum < FirstPushedCIDPos)
                {
                    firstStopCID = Convert.ToByte(value);
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

                // ��񃊁[����~�ʒu(���[�܂œǂݍ���)
                else if (indexNum < values.Length - 1)
                {
                    if (value != "ANY")
                    {
                        secondStopPos += ConvertToArrayBit(Convert.ToInt32(value));
                    }
                }

                // �Ō�̕����͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum += 1;
            }
        }

        // �����`�F�b�N
        public bool CheckSecondReelCondition(int flagID, int bet, int bonus, int random, int firstStopReelID, int firstStopCID)
        {
            // ���C�������`�F�b�N
            if (CheckMainCondition(flagID, bet, bonus, random))
            {
                // ����~�̃��[��ID��CID����v���邩�`�F�b�N 
                
                if (SecondReelCIDCheck(firstStopReelID, firstStopCID))
                {
                    // ����~�̏�������v���邩�`�F�b�N�B0��ANY
                    // ����~�̐��l���r�b�g���Z�Ŕ�r�ł���悤�ɂ���
                    int checkValue = 1 << secondStopPos + 1;
                    return secondStopPos == 0 || (checkValue & secondStopPos) != 0;
                }
            }
            return false;
        }

        // CID�`�F�b�N(����~�p)
        private bool SecondReelCIDCheck(int firstStopReelID, int firstStopCID)
        {
            // ����~���[��ID�̎w�肪0(ANY)�Ȃ疳������
            if (this.firstStopReelID == 0)
            {
                return true;
            }
            else if (firstStopReelID == this.firstStopReelID)
            {
                // ����~��CID����v�A�܂���0�Ȃ�true��Ԃ�
                return this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }

            return false;
        }
    }
}