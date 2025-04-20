using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Medal.Payout
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
            public int Payouts { get; private set; }
            public int BonusID { get; private set; }
            public bool IsReplayOrJacIn { get; private set; }

            public PayoutResultBuffer(int payouts, int bonusID, bool isReplayOrJac)
            {
                Payouts = payouts;
                BonusID = bonusID;
                IsReplayOrJacIn = isReplayOrJac;
            }

            public void SetPayout(int payouts) => Payouts = payouts;
            public void SetBonusID(int bonusID) => BonusID = bonusID;
            public void SetReplayStatus(bool isReplayOrJac) => IsReplayOrJacIn = isReplayOrJac;
        }

        // �����o���f�[�^�x�[�X
        [SerializeField] private PayoutDatabase payoutDatabase;

        // �Ō�ɓ�����������
        public PayoutResultBuffer LastPayoutResult { get; private set; }
        public PayoutDatabase PayoutDatabase { get {return payoutDatabase; } }

        // �I�𒆂̃e�[�u��
        public PayoutCheckMode CheckMode { get; private set; }

        private void Awake()
        {
            // �Ō�ɔ��肵�����̌���
            CheckMode = PayoutCheckMode.PayoutNormal;
            LastPayoutResult = new PayoutResultBuffer(0, 0, false);
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
            foreach (PayoutLineData lineData in payoutDatabase.PayoutLines)
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
                        int lineIndex = lineData.PayoutLines[reelIndex] + (int)ReelData.ReelPosID.Lower3rd * -1;

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
                            replayStatus = GetPayoutResultData(CheckMode)[foundIndex].HasReplayOrJac;
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

                Debug.Log(sameSymbolCount);

                // �����}��(ANY�܂�)�����[���̐��ƍ����Γ��I�Ƃ݂Ȃ�
                if (sameSymbolCount == ReelManager.ReelAmounts)
                {
                    Debug.Log("HIT!:" + payoutResult[indexNum].Payouts + "Bonus:"
                     + payoutResult[indexNum].BonusType + "Replay:" + payoutResult[indexNum].HasReplayOrJac);

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
