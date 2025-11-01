using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Reels;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_Reels.Table
{
    public class ReelTableManager
    {
        // ���[���e�[�u���Ǘ��p

        // const
        // �����ꂩ�̃{�[�i�X�������Ă����������������
        public const int BonusAnyValueID = 3;

        // ���A���A��O��~�̔z��Ŏg�����ʎq
        public enum StopOrder { First, Second, Third }

        // var
        // �Ō�Ɏg�p�������[���e�[�u��ID
        public int[] UsedReelTableTID { get; private set; }
        // �e���[���̍Ō�Ɏg�p�����g�ݍ��킹ID
        public int[] UsedReelTableCID { get; private set; }

        // ��~���������[�����̃��[��ID
        public ReelID[] PushedReelIdOrder { get; private set; }
        // �e�[�u��ID
        public int[] PushedReelTidOrder { get; private set; }
        // �g�ݍ��킹ID
        public int[] PushedReelCidOrder { get; private set; }

        // �R���X�g���N�^
        public ReelTableManager()
        {
            UsedReelTableTID = new int[ReelAmount] { 0, 0, 0 };
            UsedReelTableCID = new int[ReelAmount] { 0, 0, 0 };

            PushedReelIdOrder = new ReelID[ReelAmount] {ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            PushedReelTidOrder = new int[ReelAmount] { 0, 0, 0 };
            PushedReelCidOrder = new int[ReelAmount] { 0, 0, 0 };
        }

        // func
        // �X�x���R�}�𓾂�
        public int GetDelay(int stoppedCount, int pushedPos, ReelDatabase reelDatabase,
            FlagId flagID, ReelID pushReelID, BonusTypeID bonus, int bet, int random)
        {
            // �������e�[�u��ID
            int findTID = -1;
            // �������g�ݍ��킹ID
            int findCID = -1;

            // ��~�����Ƃ̏������������
            switch (stoppedCount)
            {
                // ����~
                case 0:
                    //Debug.Log("First Stop Check");
                    foreach (ReelFirstConditions first in reelDatabase.FirstCondition)
                    {
                        // ��v����e�[�u�������邽�тɍX�V������
                        if (first.CheckFirstReelCondition((int)flagID, (int)bonus, bet, random, pushedPos))
                        {
                            findTID = first.TID;
                            findCID = first.CID;
                            //Debug.Log("Found First TID:" + first.TID + " CID:" + first.CID);
                        }
                    }
                    break;

                // ����~
                case 1:
                    //Debug.Log("Second Stop Check");
                    foreach (ReelSecondConditions second in reelDatabase.SecondCondition)
                    {
                        if (second.CheckSecondReelCondition((int)flagID, (int)bonus, bet, random,
                            PushedReelIdOrder[(int)StopOrder.First], PushedReelCidOrder[(int)StopOrder.First], pushedPos))
                        {
                            findTID = second.TID;
                            findCID = second.CID;
                            //Debug.Log("Found Second TID:" + second.TID + " CID:" + second.CID);
                        }
                    }
                    break;

                // ��O��~
                case 2:
                    //Debug.Log("Third Stop Check");
                    foreach (ReelThirdConditions third in reelDatabase.ThirdCondition)
                    {
                        if (third.CheckThirdReelCondition((int)flagID, (int)bonus, bet, random,
                            PushedReelIdOrder[(int)StopOrder.First], PushedReelCidOrder[(int)StopOrder.First],
                            PushedReelIdOrder[(int)StopOrder.Second], PushedReelCidOrder[(int)StopOrder.Second]))
                        {
                            findTID = third.TID;
                            findCID = third.CID;
                            //Debug.Log("Found Third TID:" + third.TID + " CID:" + third.CID);
                        }
                    }
                    break;
            }

            // �e�[�u���Ƒg�ݍ��킹ID�������ł����炻�̃e�[�u������X�x���R�}�擾
            if (findTID != -1 && findCID != -1)
            {
                PushedReelIdOrder[stoppedCount] = pushReelID;
                PushedReelTidOrder[stoppedCount] = findTID;
                PushedReelCidOrder[stoppedCount] = findCID;
                UsedReelTableTID[(int)pushReelID] = findTID;
                UsedReelTableCID[(int)pushReelID] = findCID;

                return reelDatabase.Tables[findTID - 1].TableData[pushedPos];
            }
            else
            {
                throw new System.Exception("No table found");
            }
        }
    }
}
