using ReelSpinGame_Datas;
using ReelSpinGame_Medal;
using ReelSpinGame_Reels.Symbol;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

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
            public int Payout { get; set; }
            public int BonusID { get; set; }
            public bool IsReplayOrJacIn { get; set; }

            // �����o���̂��������C��
            public List<PayoutLineData> PayoutLines { get; set; }

            public PayoutResultBuffer(int payout, int bonusID, bool isReplayOrJac)
            {
                Payout = payout;
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
        public PayoutCheckMode CheckMode { get; set; }

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
            int finalPayout = 0;
            int bonusID = 0;
            bool replayStatus = false;
            List<PayoutLineData> finalPayoutLine = new List<PayoutLineData>();

            // �w�肵�����C�����ƂɃf�[�^�𓾂�
            foreach (PayoutLineData lineData in payoutDatabase.PayoutLines)
            {
                // �w�胉�C���̌��ʂ�ۊ�
                List<ReelSymbols> lineResult = new List<ReelSymbols>();

                // �x�b�g�����̏����𖞂����Ă��邩�`�F�b�N
                if (betAmount >= lineData.BetCondition)
                {
                    // �e���[������w�胉�C���̈ʒu�𓾂�(�g��2�i�ڂ����)
                    int reelIndex = 0;
                    foreach (List<ReelSymbols> reelResult in lastStoppedData.LastSymbols)
                    {
                        // �}�C�i�X���l��z��ԍ��ɕϊ�
                        int lineIndex = SymbolChange.GetReelArrayIndex(lineData.PayoutLines[reelIndex]);

                        //Debug.Log("Symbol:" + reelResult[lineIndex]);
                        lineResult.Add(reelResult[lineIndex]);
                        reelIndex += 1;
                    }

                    // �}���\�����X�g�ƌ���ׂĊY��������̂�����Γ��I�B�����o���A�{�[�i�X�A���v���C����������B
                    // �{�[�i�X�͔񓖑I�ł��X�g�b�N�����
                    int foundIndex = CheckHasPayout(lineResult, GetPayoutResultData(CheckMode));

                    // �f�[�^��ǉ�(�����o�����������������ǉ�����)
                    // ���������f�[�^������΋L�^(-1�ȊO)
                    if (foundIndex != -1)
                    {
                        // �����o���͏�ɃJ�E���g(15���𒴂��Ă��؂�̂Ă���)
                        finalPayout += GetPayoutResultData(CheckMode)[foundIndex].Payout;

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

            //Debug.Log("payout:" + finalPayout);
            //Debug.Log("Bonus:" + bonusID);
            //Debug.Log("IsReplay:" + replayStatus);

            // �f�o�b�O�p
            /*for(int i = 0; i < finalPayoutLine.Count; i++)
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
            }*/

            // �ő啥���o���𒴂��閇���������ꍇ�͐؂�̂Ă�
            if(finalPayout > MedalBehavior.MaxPayout)
            {
                finalPayout = MedalBehavior.MaxPayout;
            }

            LastPayoutResult.Payout = finalPayout;
            LastPayoutResult.BonusID = bonusID;
            LastPayoutResult.IsReplayOrJacIn = replayStatus;
            LastPayoutResult.PayoutLines = finalPayoutLine;
        }

        // �}���̔���(�z���Ԃ�)
        private int CheckHasPayout(List<ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
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
                for (int i = 0; i < data.Combination.Count; i++)
                {
                    // �}���������Ă��邩�`�F�b�N(ANY�Ȃ玟�̐}����)
                    if (data.Combination[i] == PayoutResultData.AnySymbol ||
                        (byte)lineResult[i] == data.Combination[i])
                    {
                        sameSymbolCount += 1;
                    }
                }

                // �����}��(ANY�܂�)�����[���̐��ƍ����Γ��I�Ƃ݂Ȃ�
                if (sameSymbolCount == ReelAmount)
                {
                    //Debug.Log("HIT!:" + payoutResult[indexNum].Payout + "Bonus:" + payoutResult[indexNum].BonusType + "Replay:" + payoutResult[indexNum].HasReplayOrJac);

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
