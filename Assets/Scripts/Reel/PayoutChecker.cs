using ReelSpinGame_Reels;
using static ReelSpinGame_Reels.ReelData;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using static ReelSpinGame_Bonus.BonusManager;
using static ReelSpinGame_Lots.Flag.FlagLots;

public class PayoutChecker
{
    // �}���̔���p

    // const
    public enum PayoutCheckMode { PayoutNormal, PayoutBIG, PayoutJAC};

    // var

    // �����o�����C���̃f�[�^
    class PayoutLineData
    {
        // const
        // �o�b�t�@����f�[�^��ǂݍ��ވʒu
        public enum ReadPos { BetCondition = 0, PayoutLineStart}

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

        // ANY�̔���pID
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
        public bool hasReplayOrJAC { get; private set; }


        // �R���X�g���N�^
        public PayoutResultData(byte flagID, byte[] combinations, byte payout,
            byte bonusType, bool hasReplayOrJAC)
        {
            this.FlagID = flagID;
            this.Combinations = combinations;   
            this.Payouts = payout;
            this.BonusType = bonusType;
            this.hasReplayOrJAC = hasReplayOrJAC;
        }
        
        // func
        public void ShowDebugData()
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
            debugData += hasReplayOrJAC + ",";

            Debug.Log(debugData);
        }
    }

    // �e�����o�����C���̃f�[�^
    private List<PayoutLineData> payoutLineDatas;


    // �e�핥���o���\���̃e�[�u��

    // �ʏ펞
    private List<PayoutResultData> normalPayoutDatas;

    // �����Q�[����
    private List<PayoutResultData> bigPayoutDatas;

    // JAC�Q�[����
    private List<PayoutResultData> jacPayoutDatas;

    // �I�𒆂̃e�[�u��
    public PayoutCheckMode CheckMode { get; private set; }

    // �R���X�g���N�^
    public PayoutChecker(StreamReader normalPayout, StreamReader bigPayout, StreamReader jacPayout, 
        StreamReader payoutLineData, PayoutCheckMode payoutMode)
    {
        // ���胂�[�h�ǂݍ���
        CheckMode = payoutMode;

        // �����o���\���쐬
        normalPayoutDatas = new List<PayoutResultData>();
        bigPayoutDatas = new List<PayoutResultData>();
        jacPayoutDatas = new List<PayoutResultData>();

        // �f�[�^�ǂݍ���
        // �ʏ펞
        while (!normalPayout.EndOfStream)
        {
            normalPayoutDatas.Add(LoadPayoutResult(normalPayout));
        }

        // BIG�����Q�[����
        while (!bigPayout.EndOfStream)
        {
            bigPayoutDatas.Add(LoadPayoutResult(bigPayout));
        }

        // JAC�Q�[����
        while (!jacPayout.EndOfStream)
        {
            jacPayoutDatas.Add(LoadPayoutResult(jacPayout));
        }


        // �f�o�b�O�p
        foreach (PayoutResultData data in normalPayoutDatas)
        {
            data.ShowDebugData();
        }
        Debug.Log("NormalPayoutData loaded");

        foreach (PayoutResultData data in bigPayoutDatas)
        {
            data.ShowDebugData();
        }
        Debug.Log("BigPayoutData loaded");

        foreach (PayoutResultData data in jacPayoutDatas)
        {
            data.ShowDebugData();
        }
        Debug.Log("JacPayoutData loaded");


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

        // �C�x���g�󂯎��p�ǂݍ���
    }

    // func

    //���胂�[�h�ύX

    public void ChangePayoutMode(PayoutCheckMode checkMode) => this.CheckMode = checkMode;

    // ���C������
    public ReelManager.PayoutResultBuffer CheckPayoutLines(ReelObject[] reelObjects, int betAmount)
    {
        byte finalPayouts = 0;
        byte bonusID = 0;
        bool replayStatus = false;

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

                int foundIndex = CheckPayoutLines(lineResult, GetPayoutResultData(CheckMode));

                // �f�[�^��ǉ�(�����o�����������������ǉ�����)
                // ���������f�[�^������΋L�^(-1�ȊO)
                if(foundIndex != -1)
                {
                    finalPayouts += GetPayoutResultData(CheckMode)[foundIndex].Payouts;

                    // �{�[�i�X�������Ȃ瓖���������ɕύX
                    if(bonusID == 0)
                    {
                        bonusID = GetPayoutResultData(CheckMode)[foundIndex].BonusType;
                    }

                    // ���v���C�łȂ���Γ����������ɕύX
                    if(replayStatus == false)
                    {
                        replayStatus = GetPayoutResultData(CheckMode)[foundIndex].hasReplayOrJAC;
                    }
                }
            }
        }
        // �ŏI�I�ȕ����o���������C�x���g�ɑ���

        Debug.Log("payout:" + finalPayouts);
        Debug.Log("Bonus:" + bonusID);
        Debug.Log("IsReplay:" + replayStatus);

        return new ReelManager.PayoutResultBuffer(finalPayouts, bonusID, replayStatus);
    }

    private PayoutResultData LoadPayoutResult(StreamReader streamPayout)
    {
        // �X�g���[������f�[�^�𓾂�
        byte[] byteBuffer = Array.ConvertAll(streamPayout.ReadLine().Split(','), byte.Parse);

        // �}���g�ݍ��킹�̃f�[�^�ǂݍ���(Payout�̈ʒu�܂œǂݍ���)
        byte[] combinations = new byte[ReelManager.ReelAmounts];

        // �f�o�b�O�p
        string combinationBuffer = "";

        // �ǂݍ���
        for (int i = 0; i < ReelManager.ReelAmounts; i++)
        {
            combinations[i] = byteBuffer[i + (int)PayoutResultData.ReadPos.CombinationsStart];
            combinationBuffer += combinations[i];
        }

        PayoutResultData finalResult =  new PayoutResultData(byteBuffer[(int)PayoutResultData.ReadPos.FlagID], combinations,
            byteBuffer[(int)PayoutResultData.ReadPos.Payout], byteBuffer[(int)PayoutResultData.ReadPos.Bonus],
            byteBuffer[(int)PayoutResultData.ReadPos.IsReplay] == 1);


        //�f�o�b�O�p
        Debug.Log("Combination:" + combinationBuffer + "Payouts:" + finalResult.Payouts +
            "Bonus:" + finalResult.BonusType + "HasReplay:" + finalResult.hasReplayOrJAC);

        return finalResult;
    }

    // �}���̔���(�z���Ԃ�)
    private int CheckPayoutLines(List<ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
    {
        // �S�ē����}���������Ă�����HIT��Ԃ�
        // ANY(10��)�͖���

        int indexNum = 0;
        foreach (PayoutResultData data in payoutResult)
        {
            // �����}�����J�E���g����
            int sameSymbolCount = 0;

            for (int i = 0; i < data.Combinations.Length; i++)
            {
                // �}���������Ă��邩�`�F�b�N(ANY�Ȃ玟�̐}����)
                Debug.Log(lineResult[i] + "," + data.Combinations[i]);

                if (data.Combinations[i] == PayoutResultData.AnySymbol ||
                    (byte)lineResult[i] == data.Combinations[i])
                {
                    sameSymbolCount += 1;
                }
            }

            Debug.Log(sameSymbolCount);

            if(sameSymbolCount == ReelManager.ReelAmounts)
            {
                Debug.Log("HIT!:" + payoutResult[indexNum].Payouts + "Bonus:"
                 + payoutResult[indexNum].BonusType + "Replay:" + payoutResult[indexNum].hasReplayOrJAC);

                // �z��ԍ��𑗂�

                return indexNum;
            }

            // �Ȃ������ꍇ�͎��̔ԍ���
            indexNum += 1;
        }
        return -1;
    }

    private List<PayoutResultData> GetPayoutResultData(PayoutCheckMode payoutCheckMode)
    {
        Debug.Log(payoutCheckMode.ToString());
        switch(payoutCheckMode)
        {
            case PayoutCheckMode.PayoutNormal:
                return normalPayoutDatas;

            case PayoutCheckMode.PayoutBIG:
                return bigPayoutDatas;

            case PayoutCheckMode.PayoutJAC:
                return jacPayoutDatas;
        }
        return null;
    }
}
