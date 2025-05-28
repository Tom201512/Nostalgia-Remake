using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Reels.Payout
{
    public class PayoutChecker : MonoBehaviour
    {
        // �}���̔���p

        // const
        public enum PayoutCheckMode { PayoutNormal, PayoutBIG, PayoutJAC };

        // var
        // �����o������
        public class PayoutResultBuffer
        {
            public int Payouts { get; set; }
            public int BonusID { get; set; }
            public bool IsReplayOrJacIn { get; set; }

            // �����o���̂��������C��
            public List<PayoutLineData> PayoutLines { get; set; }

            public PayoutResultBuffer(int payouts, int bonusID, bool isReplayOrJac)
            {
                Payouts = payouts;
                BonusID = bonusID;
                IsReplayOrJacIn = isReplayOrJac;
                PayoutLines = new List<PayoutLineData>();
            }
        }

        // �����o���f�[�^�x�[�X
        [SerializeField] private PayoutDatabase payoutDatabase;

        // �Ō�ɓ�����������
        public PayoutResultBuffer LastPayoutResult { get; private set; }

        // �I�𒆂̃e�[�u��
        public PayoutCheckMode CheckMode;

        // func
        private void Awake()
        {
            CheckMode = PayoutCheckMode.PayoutNormal;
            LastPayoutResult = new PayoutResultBuffer(0, 0, false);
        }

        // �����o�����C��
        public List<PayoutLineData> GetPayoutLines() => payoutDatabase.PayoutLines;

        // ���C������
        public void CheckPayoutLines(int betAmount, LastStoppedReelData lastStoppedData)
        {
            // �ŏI�I�ȕ����o������
            int finalPayouts = 0;
            int bonusID = 0;
            bool replayStatus = false;
            List<PayoutLineData> finalPayoutLine = new List<PayoutLineData>();

            // �w�肵�����C�����ƂɃf�[�^�𓾂�
            foreach (PayoutLineData lineData in payoutDatabase.PayoutLines)
            {
                // �w�胉�C���̌��ʂ�ۊ�
                List<ReelData.ReelSymbols> lineResult = new List<ReelData.ReelSymbols>();

                // �x�b�g�����̏����𖞂����Ă��邩�`�F�b�N
                if (betAmount >= lineData.BetCondition)
                {
                    // �e���[������w�胉�C���̈ʒu�𓾂�(�g��2�i�ڂ����)
                    int reelIndex = 0;
                    foreach (List<ReelData.ReelSymbols> reelResult in lastStoppedData.LastSymbols)
                    {
                        // �}�C�i�X���l��z��ԍ��ɕϊ�
                        int lineIndex = ReelData.GetReelArrayIndex(lineData.PayoutLines[reelIndex]);

                        //Debug.Log("Symbol:" + reelResult[lineIndex]);
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
                            replayStatus = GetPayoutResultData(CheckMode)[foundIndex].HasReplayOrJac;
                        }

                        // �����������C�����L�^
                        finalPayoutLine.Add(lineData);
                    }
                }
                // �����𖞂����Ȃ��ꍇ�͏I��
                else
                {
                    break;
                }
            }
            // �ŏI�I�ȕ����o���������C�x���g�ɑ���

            //Debug.Log("payout:" + finalPayouts);
            //Debug.Log("Bonus:" + bonusID);
            //Debug.Log("IsReplay:" + replayStatus);

            // �f�o�b�O�p
            for(int i = 0; i < finalPayoutLine.Count; i++)
            {
                string buffer = "";
                for(int j = 0; j < finalPayoutLine[i].PayoutLines.Count; j++)
                {
                    buffer += finalPayoutLine[i].PayoutLines[j].ToString();

                    if(j != finalPayoutLine[i].PayoutLines.Count - 1)
                    {
                        buffer += ",";
                    }
                }
                //Debug.Log("PayoutLines" + i + ":" + buffer);
            }

            LastPayoutResult.Payouts = finalPayouts;
            LastPayoutResult.BonusID = bonusID;
            LastPayoutResult.IsReplayOrJacIn = replayStatus;
            LastPayoutResult.PayoutLines = finalPayoutLine;
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
                for (int i = 0; i < data.Combinations.Count; i++)
                {
                    // �}���������Ă��邩�`�F�b�N(ANY�Ȃ玟�̐}����)
                    if (data.Combinations[i] == PayoutResultData.AnySymbol ||
                        (byte)lineResult[i] == data.Combinations[i])
                    {
                        sameSymbolCount += 1;
                    }
                }

                // �����}��(ANY�܂�)�����[���̐��ƍ����Γ��I�Ƃ݂Ȃ�
                if (sameSymbolCount == ReelAmounts)
                {
                    //Debug.Log("HIT!:" + payoutResult[indexNum].Payouts + "Bonus:" + payoutResult[indexNum].BonusType + "Replay:" + payoutResult[indexNum].HasReplayOrJac);

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
            //Debug.Log(payoutCheckMode.ToString());
            switch (payoutCheckMode)
            {
                case PayoutCheckMode.PayoutNormal:
                    return payoutDatabase.NormalPayoutDatas;

                case PayoutCheckMode.PayoutBIG:
                    return payoutDatabase.BigPayoutDatas;

                case PayoutCheckMode.PayoutJAC:
                    return payoutDatabase.JacPayoutDatas;
            }
            return null;
        }
    }
}
