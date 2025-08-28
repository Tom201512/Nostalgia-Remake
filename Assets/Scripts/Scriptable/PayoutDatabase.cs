using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Datas
{
    // �����o���f�[�^�x�[�X
    public class PayoutDatabase : ScriptableObject
    {
        // var
        // �e�����o�����C���̃f�[�^
        [SerializeField]private List<PayoutLineData> payoutLineDatas;

        // �e�핥���o���\���̃e�[�u��
        // �ʏ펞
        [SerializeField] private List<PayoutResultData> normalPayoutDatas;
        // �����Q�[����
        [SerializeField] private List<PayoutResultData> bigPayoutDatas;
        // JAC�Q�[����
        [SerializeField] private List<PayoutResultData> jacPayoutDatas;

        public List<PayoutLineData> PayoutLines { get { return payoutLineDatas; }}
        public List<PayoutResultData> NormalPayoutDatas { get { return normalPayoutDatas; } }
        public List<PayoutResultData> BigPayoutDatas { get { return bigPayoutDatas; }  }
        public List<PayoutResultData> JacPayoutDatas { get { return jacPayoutDatas; } }

        // func
        public void SetPayoutLines(List<PayoutLineData> payoutLines) => payoutLineDatas = payoutLines;
        public void SetNormalPayout(List<PayoutResultData> normalPayoutDatas) => this.normalPayoutDatas = normalPayoutDatas;
        public void SetBigPayout(List<PayoutResultData> bigPayoutDatas) => this.bigPayoutDatas = bigPayoutDatas;
        public void SetJacPayout(List<PayoutResultData> jacPayoutDatas) => this.jacPayoutDatas = jacPayoutDatas;
    }

    // �����o�����C���̃f�[�^
    [Serializable]
    public class PayoutLineData
    {
        // const
        // �o�b�t�@����f�[�^��ǂݍ��ވʒu
        public enum ReadPos { BetCondition = 0, PayoutLineStart }

        // �L���ɕK�v�ȃx�b�g����
        [SerializeField] private byte betCondition;
        // �����o�����C��(�����t��byte)
        [SerializeField] private List<sbyte> payoutLines;

        public byte BetCondition { get { return betCondition; } }
        public List<sbyte> PayoutLines { get { return payoutLines; }}

        public PayoutLineData(StreamReader loadedData)
        {
            // �X�g���[������f�[�^�𓾂�
            sbyte[] byteBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), sbyte.Parse);
            // �����o�����C���̃f�[�^
            payoutLines = new List<sbyte>();
            // �f�o�b�O�p
            string combinationBuffer = "";
            //Debug.Log(byteBuffer[0]);

            betCondition = (byte)byteBuffer[(int)ReadPos.BetCondition];
            // �ǂݍ���
            for (int i = 0; i < ReelAmount; i++)
            {
                payoutLines.Add(byteBuffer[i + (int)ReadPos.PayoutLineStart]);
                combinationBuffer += payoutLines[i];
            }

            //�f�o�b�O�p
            //Debug.Log("Condition:" + byteBuffer[(int)ReadPos.BetCondition] + "Lines" + combinationBuffer);
        }
    }

    // �����o�����ʂ̃f�[�^
    [Serializable]
    public class PayoutResultData
    {
        // const
        // �o�b�t�@����f�[�^��ǂݍ��ވʒu
        public enum ReadPos { FlagID = 0, CombinationStart = 1, Payout = 4, Bonus, IsReplay }
        // ANY�̔���pID
        public const int AnySymbol = 7;

        // var
        // �t���OID
        [SerializeField] private byte flagID;
        // �}���\��
        [SerializeField] private List<byte> combination;
        // �����o������
        [SerializeField] private byte payout;
        // ���I����{�[�i�X
        [SerializeField] private byte bonusType;
        // ���v���C��(�܂���JAC-IN)
        [SerializeField] private bool hasReplayOrJac;

        public byte FlagID { get { return flagID; }}
        public List<byte> Combination { get { return combination; } }
        public byte Payout { get { return payout; } }
        public byte BonusType { get { return bonusType; } }
        public bool HasReplayOrJac { get { return hasReplayOrJac; } }

        public PayoutResultData(StreamReader loadedData)
        {
            // �X�g���[������f�[�^�𓾂�
            byte[] byteBuffer = Array.ConvertAll(loadedData.ReadLine().Split(','), byte.Parse);
            // �}���g�ݍ��킹�̃f�[�^�ǂݍ���(Payout�̈ʒu�܂œǂݍ���)
            combination = new List<byte>();
            // �f�o�b�O�p
            string combinationBuffer = "";

            // �f�[�^�쐬
            // �t���OID
            flagID = byteBuffer[(int)ReadPos.FlagID];

            // �g�ݍ��킹
            for (int i = 0; i < ReelAmount; i++)
            {
                combination.Add(byteBuffer[i + (int)ReadPos.CombinationStart]);
                combinationBuffer += combination[i];
            }
            // �����o��
            payout = byteBuffer[(int)ReadPos.Payout];
            // �{�[�i�X
            bonusType = byteBuffer[(int)ReadPos.Bonus];
            // ���v���C�̗L��
            hasReplayOrJac = byteBuffer[(int)ReadPos.IsReplay] == 1;

            //�f�o�b�O�p
            //Debug.Log("Flag:" + flagID + "Combination:" + combinationBuffer + "Payout:" + payout +
                //"Bonus:" + bonusType + "HasReplay:" + hasReplayOrJac);
        }
    }
}
