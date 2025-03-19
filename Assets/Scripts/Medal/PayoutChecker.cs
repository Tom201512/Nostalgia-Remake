using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

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
        public PayoutLineData(byte betCondition, sbyte[] lines)
        {
            this.BetCondition = betCondition;
            this.PayoutLine = lines;
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
    }

    // �����o������
    public class PayoutResultBuffer
    {
        public int Payouts { get; private set; }
        public int BonusID { get; private set; }
        public bool IsReplayOrJAC { get; private set; }

        public PayoutResultBuffer(int payouts, int bonusID, bool isReplayOrJac)
        {
            Payouts = payouts;
            BonusID = bonusID;
            IsReplayOrJAC = isReplayOrJac;
        }

        public void SetPayout(int payouts) => Payouts = payouts;
        public void SetBonusID(int bonusID) => BonusID = bonusID;
        public void SetReplayStatus(bool isReplayOrJac) => IsReplayOrJAC = isReplayOrJac;
    }

    // �Ō�ɓ�����������
    public PayoutResultBuffer LastPayoutResult { get; private set; }

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
    public PayoutChecker(string normalPayoutPath, string bigPayoutPath, string jacPayoutPath,
        string payoutLineDataPath, PayoutCheckMode payoutMode)
    {
        StreamReader normalPayout = new StreamReader(normalPayoutPath);
        StreamReader bigPayout = new StreamReader(bigPayoutPath);
        StreamReader jacPayout = new StreamReader(jacPayoutPath);
        StreamReader payoutLine = new StreamReader(payoutLineDataPath);

        // ���胂�[�h�ǂݍ���
        CheckMode = payoutMode;

        // �����o���\���쐬
        normalPayoutDatas = new List<PayoutResultData>();
        bigPayoutDatas = new List<PayoutResultData>();
        jacPayoutDatas = new List<PayoutResultData>();

        // �Ō�ɔ��肵�����̌���
        LastPayoutResult = new PayoutResultBuffer(0, 0, false);

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

        Debug.Log("JacPayoutData loaded");

        // �����o�����C���̓ǂݍ���
        payoutLineDatas = new List<PayoutLineData>();

        // �f�[�^�ǂݍ���
        while(!payoutLine.EndOfStream)
        {
            payoutLineDatas.Add(LoadPayoutLines(payoutLine));
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

    //���胂�[�h�ύX
    public void ChangePayoutCheckMode(PayoutCheckMode checkMode) => this.CheckMode = checkMode;

    // ���C������
    public void CheckPayoutLines(int betAmount, List<List<ReelData.ReelSymbols>> lastSymbols)
    {
        // �ŏI�I�ȕ����o������
        int finalPayouts = 0;
        int bonusID = 0;
        bool replayStatus = false;

        // ���邱��
        // 1:�w�肳�ꂽ�����o�����C�����ƂɃf�[�^���i�[����(�x�b�g�����𖞂����Ă���K�v������)
        // 2:�z���\�̐}���������Ă��邩���m�F����
        // 3:�����Ă���Ε����o���A�{�[�i�X�A�܂��̓��v���C�̏�Ԃ�ω�������

        // �w�肵�����C�����ƂɃf�[�^�𓾂�
        foreach (PayoutLineData lineData in payoutLineDatas)
        {
            // �w�胉�C���̌��ʂ�ۊ�
            List<ReelSymbols> lineResult = new List<ReelSymbols>();

            // �x�b�g�����̏����𖞂����Ă��邩�`�F�b�N
            if (betAmount >= lineData.BetCondition)
            {
                // �e���[������w�胉�C���̈ʒu�𓾂�(�g��2�i�ڂ����)
                int reelIndex = 0;
                foreach (List<ReelSymbols> reelResult in lastSymbols)
                {
                    // �}�C�i�X���l��z��ԍ��ɕϊ�
                    int lineIndex = lineData.PayoutLine[reelIndex] + (int)ReelPosID.Lower3rd * -1;

                    Debug.Log("Symbol:" + reelResult[lineIndex]);
                   lineResult.Add(reelResult[lineIndex]);
                    reelIndex += 1;
                }

                // �}���\�����X�g�ƌ���ׂĊY��������̂�����Γ��I�B�����o���A�{�[�i�X�A���v���C����������B
                // �{�[�i�X�͔񓖑I�ł��X�g�b�N�����
                int foundIndex = CheckPayoutLines(lineResult, GetPayoutResultData(CheckMode));

                // �f�[�^��ǉ�(�����o�����������������ǉ�����)
                // ���������f�[�^������΋L�^(-1�ȊO)
                if (foundIndex != -1)
                {
                    // �����o���͏�ɃJ�E���g(15���𒴂��Ă��؂�̂Ă���)
                    finalPayouts += GetPayoutResultData(CheckMode)[foundIndex].Payouts;

                    // �{�[�i�X�������Ȃ瓖���������ɕύX
                    if (bonusID == 0)
                    {
                        bonusID = GetPayoutResultData(CheckMode)[foundIndex].BonusType;
                    }
                    // ���v���C�łȂ���Γ����������ɕύX
                    if (replayStatus == false)
                    {
                        replayStatus = GetPayoutResultData(CheckMode)[foundIndex].hasReplayOrJAC;
                    }
                }
            }
            
            // �����𖞂����Ȃ��ꍇ�͏I��
            else
            {
                break;
            }
        }
        // �ŏI�I�ȕ����o���������C�x���g�ɑ���

        Debug.Log("payout:" + finalPayouts);
        Debug.Log("Bonus:" + bonusID);
        Debug.Log("IsReplay:" + replayStatus);

        LastPayoutResult.SetPayout(finalPayouts);
        LastPayoutResult.SetBonusID(bonusID);
        LastPayoutResult.SetReplayStatus(replayStatus);
    }

    // �����o�����C���̃f�[�^�ǂݍ���
    private PayoutLineData LoadPayoutLines(StreamReader streamLines)
    {
        // �X�g���[������f�[�^�𓾂�
        sbyte[] byteBuffer = Array.ConvertAll(streamLines.ReadLine().Split(','), sbyte.Parse);
        // �����o�����C���̃f�[�^
        sbyte[] lineData = new sbyte[ReelManager.ReelAmounts];
        // �f�o�b�O�p
        string combinationBuffer = "";

        // �ǂݍ���
        for (int i = 0; i < ReelManager.ReelAmounts; i++)
        {
            lineData[i] = byteBuffer[i + (int)PayoutLineData.ReadPos.PayoutLineStart];
            combinationBuffer += lineData[i];
        }

        PayoutLineData finalResult = new PayoutLineData((byte)byteBuffer[(int)PayoutLineData.ReadPos.BetCondition], lineData);

        //�f�o�b�O�p
        Debug.Log("Condition:" + finalResult.BetCondition + "Lines" + combinationBuffer);

        return finalResult;
    }

    // �����o�����ʂ̃f�[�^�ǂݍ���
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

        // �f�[�^�쐬
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

        // ���������f�[�^�̈ʒu
        int indexNum = 0;
        
        // �����o�����ʂ̔���
        foreach (PayoutResultData data in payoutResult)
        {
            // �����}�����J�E���g����
            int sameSymbolCount = 0;

            // �}���̃`�F�b�N
            for (int i = 0; i < data.Combinations.Length; i++)
            {
                // �}���������Ă��邩�`�F�b�N(ANY�Ȃ玟�̐}����)
                if (data.Combinations[i] == PayoutResultData.AnySymbol ||
                    (byte)lineResult[i] == data.Combinations[i])
                {
                    sameSymbolCount += 1;
                }
            }

            Debug.Log(sameSymbolCount);

            // �����}��(ANY�܂�)�����[���̐��ƍ����Γ��I�Ƃ݂Ȃ�
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
        // ������Ȃ��ꍇ��-1��Ԃ�(�͂���ƂȂ�)
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
