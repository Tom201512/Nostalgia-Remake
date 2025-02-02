using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChecker
{
    // �}���̔���p

    // �����o�����C���̃f�[�^
    class PayoutLineData
    {
        // const
        // �o�b�t�@����f�[�^��ǂݍ��ވʒu
        public enum ReadPos { BetCondition = 0, PayoutLineStart}

        // var

        // �L���ɕK�v�ȃx�b�g����
        public byte BetCondition { get; private set; }

        // �����o�����C��(�����t��byte)
        public sbyte[] PayoutLine { get; private set; }

        //�R���X�g���N�^
        public PayoutLineData(sbyte[] buffer)
        {
            // �x�b�g�����̓ǂݍ���
            this.BetCondition = (byte)buffer[(int)ReadPos.BetCondition];

            // �����o�����C���̓ǂݍ���
            // �}���g�ݍ��킹�̃f�[�^�ǂݍ���(Payout�̈ʒu�܂œǂݍ���)
            PayoutLine = new sbyte[ReelManager.ReelAmounts];

            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                PayoutLine[i] = buffer[i + (int)ReadPos.PayoutLineStart];
            }
        }
    }

    // �����o�����ʂ̃f�[�^
    class PayoutResultData
    {
        // const

        // �o�b�t�@����f�[�^��ǂݍ��ވʒu
        public enum ReadPos { FlagID = 0, CombinationsStart = 1, Payout = 4, Bonus, IsReplay}

        public const int AnySymbol = 7;
        // var

        // �t���OID
        public byte FlagID { get; private set; }
        // �}���\��
        public byte[] Combinations{get; private set; }

        // �����o������
        public byte Payouts {get; private set; }

        // ���I����{�[�i�X
        public byte BonusType { get; private set; }

        // ���v���C��(�܂���JAC-IN)
        public byte IsReplay { get; private set; }

        public PayoutResultData(byte[] buffer)
        {
            // �t���OID
            FlagID = buffer[(int)ReadPos.FlagID];

            // �}���g�ݍ��킹�̃f�[�^�ǂݍ���(Payout�̈ʒu�܂œǂݍ���)
            Combinations = new byte[ReelManager.ReelAmounts];

            Debug.Log(Combinations.Length);
            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                Combinations[i] = buffer[i + (int)ReadPos.CombinationsStart];
                Debug.Log(Combinations[i]);
            }

            // �����o������
            Payouts = buffer[(int)ReadPos.Payout];

            // ���I����{�[�i�X
            BonusType = buffer[(int)ReadPos.Bonus];

            // ���v���C��
            IsReplay = buffer[(int)ReadPos.IsReplay];
        }
        
        // func
        public void ShowData()
        {
            string debugData = "";

            string symbols = "";
            foreach (byte b in Combinations)
            {
                symbols += b.ToString();
            }

            debugData += FlagID + ",";
            debugData += symbols + ",";
            debugData += Payouts + ",";
            debugData += BonusType + ",";
            debugData += IsReplay + ",";

            Debug.Log(debugData);
        }
    }

    // �e�����o�����C���̃f�[�^
    private List<PayoutLineData> payoutLineDatas;

    // �e�핥���o���\���̃e�[�u��

    // �ʏ펞
    private List<PayoutResultData> normalPayoutDatas;

    // �����Q�[����

    // JAC�Q�[����


    // �R���X�g���N�^
    public SymbolChecker(StreamReader normalPayoutData, StreamReader payoutLineData)
    {
        // �����o���\���̓ǂݍ���

        normalPayoutDatas = new List<PayoutResultData>();

        // �f�[�^�ǂݍ���
        while (!normalPayoutData.EndOfStream)
        {
            byte[] byteBuffer = Array.ConvertAll(normalPayoutData.ReadLine().Split(','), byte.Parse);
            normalPayoutDatas.Add(new PayoutResultData(byteBuffer));
        }

        // �f�o�b�O�p
        foreach (PayoutResultData data in normalPayoutDatas)
        {
            data.ShowData();
        }
        Debug.Log("NormalPayoutData loaded");

        // �����o�����C���̓ǂݍ���
        payoutLineDatas = new List<PayoutLineData>();

        // �f�[�^�ǂݍ���
        while(!payoutLineData.EndOfStream)
        {
            sbyte[] byteBuffer = Array.ConvertAll(payoutLineData.ReadLine().Split(','), sbyte.Parse);
            payoutLineDatas.Add(new PayoutLineData(byteBuffer));
        }

        // �f�o�b�O�p
        foreach (PayoutLineData data in payoutLineDatas)
        {
            string line = "";
            foreach (sbyte b in data.PayoutLine)
            {
                line += b.ToString();
            }
            Debug.Log(line + "," + data.BetCondition);
        }
        Debug.Log("PayoutLine Data loaded");
    }

    // func

    // ���C������
    public int CheckPayout(ReelObject[] reelObjects, int betAmount)
    {
        int finalPayoutResult = 0;
        // �e���C�����略���o���̃`�F�b�N������
        foreach (PayoutLineData lineData in payoutLineDatas)
        {
            // �x�b�g�����̏����𖞂����Ă��邩�`�F�b�N
            if (betAmount >= lineData.BetCondition)
            {
                // ���ʂ����X�g�ɂ܂Ƃ߂�
                List<ReelSymbols> lineResult = new List<ReelData.ReelSymbols>();

                // �e���[���̕����o�����`�F�b�N
                for(int i = 0; i < reelObjects.Length; i++)
                {
                    lineResult.Add(reelObjects[i].ReelData.GetReelSymbol(lineData.PayoutLine[i]));
                }

                // �f�o�b�O�p
                string lineBuffer = "";
                foreach(byte b in lineData.PayoutLine)
                {
                    lineBuffer += b.ToString();
                }

                string resultBuffer = "";
                foreach (ReelSymbols symbol in lineResult)
                {
                    resultBuffer += symbol.ToString();
                }
                Debug.Log(lineBuffer + "," + resultBuffer);

                // �}���\�����X�g�ƌ���ׂĊY��������̂�����Γ��I�B�����o���A�{�[�i�X�A���v���C����������B
                // �{�[�i�X�͔񓖑I�ł��X�g�b�N�����

                // �f�o�b�O�p
                finalPayoutResult += CheckPayoutLines(lineResult);
            }
        }
        // �ŏI�I�ȕ����o��������Ԃ�

        Debug.Log("payout:" + finalPayoutResult);
        return finalPayoutResult;
    }

    // �}���̔���
    private int CheckPayoutLines(List<ReelSymbols> lineResult)
    {
        // �����o������(�ő�15���܂�)
        int payoutAmounts = 0;

        // �S�ē����}���������Ă�����HIT��Ԃ�
        // ANY(10��)�͖���

        foreach (PayoutResultData resultData in normalPayoutDatas)
        {
            int sameSymbolCount = 0;
            for (int i = 0; i < resultData.Combinations.Length; i++)
            {
                // �}���������Ă��邩�`�F�b�N(ANY�Ȃ玟�̐}����)
                if (resultData.Combinations[i] == PayoutResultData.AnySymbol ||
                    (byte)lineResult[i] == resultData.Combinations[i])
                {
                    sameSymbolCount += 1;
                }
            }

            Debug.Log(sameSymbolCount);

            if(sameSymbolCount == ReelManager.ReelAmounts)
            {
                Debug.Log("HIT!:" + resultData.Payouts + "Bonus:"
                 + resultData.BonusType + "Replay:" + resultData.IsReplay);

                //�����ɓ��I�����{�[�i�X�A���v���C�𔽉f����

                return resultData.Payouts;
            }
        }

        return 0;
    }
}
