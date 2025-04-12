using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ReelSpinGame_Medal.Payout.Lines;
using ReelSpinGame_Medal.Payout.Results;
using ReelSpinGame_Main.File;

namespace ReelSpinGame_Medal.Payout
{
    public class PayoutChecker
    {
        // �}���̔���p

        // const
        public enum PayoutCheckMode { PayoutNormal, PayoutBIG, PayoutJAC };

        // var
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

        // �����o���\�Ɛ������C���f�[�^
        private string NormalPayoutPath = Application.streamingAssetsPath + "/DataFile/Payouts/NormalPayoutData.csv";
        private string BigPayoutPath = Application.streamingAssetsPath + "/DataFile/Payouts/BigPayoutData.csv";
        private string JacPayoutPath = Application.streamingAssetsPath + "/DataFile/Payouts/JACPayout.csv";
        private string PayoutLinePath = Application.streamingAssetsPath + "/DataFile/Payouts/PayoutLine.csv";

        // �I�𒆂̃e�[�u��
        public PayoutCheckMode CheckMode { get; private set; }

        // �R���X�g���N�^
        public PayoutChecker(PayoutCheckMode payoutMode)
        {
            StreamReader normalPayout = new StreamReader(NormalPayoutPath) ?? 
                throw new Exception("NormalPayout file is missing");
            StreamReader bigPayout = new StreamReader(BigPayoutPath) ??
                throw new Exception("BigPayout file is missing");
            StreamReader jacPayout = new StreamReader(JacPayoutPath) ??
                throw new Exception("JacPayout file is missing");
            StreamReader payoutLine = new StreamReader(PayoutLinePath) ??
                throw new Exception("PayoutLine file is missing");

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

            Debug.Log("NormalPayoutData loaded");

            // BIG�����Q�[����
            while (!bigPayout.EndOfStream)
            {
                bigPayoutDatas.Add(LoadPayoutResult(bigPayout));
            }

            Debug.Log("BigPayoutData loaded");

            // JAC�Q�[����
            while (!jacPayout.EndOfStream)
            {
                jacPayoutDatas.Add(LoadPayoutResult(jacPayout));
            }

            Debug.Log("JacPayoutData loaded");

            // �����o�����C���̓ǂݍ���
            payoutLineDatas = new List<PayoutLineData>();

            // �f�[�^�ǂݍ���
            while (!payoutLine.EndOfStream)
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

            // �w�肵�����C�����ƂɃf�[�^�𓾂�
            foreach (PayoutLineData lineData in payoutLineDatas)
            {
                // �w�胉�C���̌��ʂ�ۊ�
                List<ReelData.ReelSymbols> lineResult = new List<ReelData.ReelSymbols>();

                // �x�b�g�����̏����𖞂����Ă��邩�`�F�b�N
                if (betAmount >= lineData.BetCondition)
                {
                    // �e���[������w�胉�C���̈ʒu�𓾂�(�g��2�i�ڂ����)
                    int reelIndex = 0;
                    foreach (List<ReelData.ReelSymbols> reelResult in lastSymbols)
                    {
                        // �}�C�i�X���l��z��ԍ��ɕϊ�
                        int lineIndex = lineData.PayoutLine[reelIndex] + (int)ReelData.ReelPosID.Lower3rd * -1;

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
            PayoutResultData finalResult = new PayoutResultData(byteBuffer[(int)PayoutResultData.ReadPos.FlagID], combinations,
                byteBuffer[(int)PayoutResultData.ReadPos.Payout], byteBuffer[(int)PayoutResultData.ReadPos.Bonus],
                byteBuffer[(int)PayoutResultData.ReadPos.IsReplay] == 1);


            //�f�o�b�O�p
            Debug.Log("Combination:" + combinationBuffer + "Payouts:" + finalResult.Payouts +
                "Bonus:" + finalResult.BonusType + "HasReplay:" + finalResult.hasReplayOrJAC);

            return finalResult;
        }

        // �}���̔���(�z���Ԃ�)
        private int CheckPayoutLines(List<ReelData.ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
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
                if (sameSymbolCount == ReelManager.ReelAmounts)
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

        // �����o�����ʂ��e�[�u�����Ƃɓ���
        private List<PayoutResultData> GetPayoutResultData(PayoutCheckMode payoutCheckMode)
        {
            Debug.Log(payoutCheckMode.ToString());
            switch (payoutCheckMode)
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
}
