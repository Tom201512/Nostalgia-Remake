using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelThirdData : ReelBaseData
    {
        // const
        // ����~�������[��ID��ǂݍ��ވʒu
        private const int FirstPushedReelIDPos = ConditionMaxRead + 1;
        // ����~�������[����CID��ǂݍ��ވʒu
        private const int FirstPushedCIDPos = FirstPushedReelIDPos + 1;
        // ����~�������[��ID��ǂݍ��ވʒu
        private const int SecondPushedReedPos = FirstPushedCIDPos + 1;
        // ����~�������[����CID��ǂݍ��ވʒu
        private const int SecondPushedTIDPos = SecondPushedReedPos + 1;
        // ��O��~��TID�ǂݍ��݈ʒu
        private const int ThirdPushTIDPos = SecondPushedTIDPos + 1;
        // ��O��~��CID�ǂݍ��݈ʒu
        private const int ThirdPushCIDPos = ThirdPushTIDPos + 1;

        // var
        // ����~�̒�~����

        // ����~�������[����ID
        [SerializeField] private byte firstStopReelID;
        // ����~�������[����CID
        [SerializeField] private byte firstStopCID;
        // ����~�������[����ID
        [SerializeField] private byte secondStopReelID;
        // ����~�������[����CID
        [SerializeField] private byte secondStopCID;

        // �R���X�g���N�^
        public ReelThirdData(StreamReader sReader)
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

                // ��񂵂����[����ID�ǂݍ���
                else if (indexNum < SecondPushedReedPos)
                {
                    secondStopReelID = Convert.ToByte(value);
                }

                // ��񂵂����[����TID�ǂݍ���
                else if (indexNum < SecondPushedTIDPos)
                {
                    secondStopCID = Convert.ToByte(value);
                }

                // ��O��~��TID
                else if (indexNum < ThirdPushTIDPos)
                {
                    TID = Convert.ToByte(value);
                }

                // ��O��~��CID
                else if (indexNum < ThirdPushCIDPos)
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

            Debug.Log("MainCondition:" + MainConditions);
            Debug.Log("FirstPushedReelID:" + firstStopReelID);
            Debug.Log("FirstPushedCID:" + firstStopCID);
            Debug.Log("SecondPushedReelID:" + secondStopReelID);
            Debug.Log("SecondPushedCID:" + secondStopCID);
            Debug.Log("TID:" + TID);
            Debug.Log("CID:" + CID);

            Debug.Log("Third Push Load Done");
        }

        // �����`�F�b�N
        public bool CheckThirdReelCondition(int flagID, int bet, int bonus, int random, 
            int firstStopReelID, int firstStopCID, int secondStopReelID, int secondStopReelCID)
        {
            // ���C�������`�F�b�N
            if (CheckMainCondition(flagID, bet, bonus, random))
            {
                // ���Ƒ���~�̃��[��ID, CID����v���邩�`�F�b�N
                return ThirdReelCIDCheck(firstStopReelID, firstStopCID, secondStopReelID, secondStopReelCID);
            }
            return false;
        }

        // CID�`�F�b�N(��O��~�p)
        private bool ThirdReelCIDCheck(int firstStopReelID, int firstStopCID, int secondStopReelID, int secondStopCID)
        {
            bool first = false;
            bool second = false;

            // ����~
            // ����~���[��ID�̎w�肪0�ȊO�Ȃ���������������`�F�b�N
            if (this.firstStopReelID != 0 && firstStopReelID + 1 == this.firstStopReelID)
            {
                return false;
            }
            // ����~���[��ID�̏��������������CID���`�F�b�N
            else
            {
                // ����~��CID����v�A�܂���0�Ȃ�true��Ԃ�
                Debug.Log("CID Check" + (this.firstStopCID == 0 || firstStopCID == this.firstStopCID));
                first = this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }

            // ����~
            // ����~���[��ID�̎w�肪0�ȊO�Ȃ���������������`�F�b�N
            if (this.secondStopReelID != 0 && secondStopReelID + 1 == this.firstStopReelID)
            {
                return false;
            }
            // ����~���[��ID�̏��������������CID���`�F�b�N
            else
            {
                // ����~��CID����v�A�܂���0�Ȃ�true��Ԃ�
                Debug.Log("CID Check" + (this.secondStopCID == 0 || firstStopCID == this.secondStopCID));
                second = this.secondStopCID == 0 || secondStopCID == this.secondStopCID;
            }

            // �o���̌��ʂ�true�Ȃ������v
            return first && second;
        }
    }
}
