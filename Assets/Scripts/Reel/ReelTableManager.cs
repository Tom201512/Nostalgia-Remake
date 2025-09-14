using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Reels;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Datas.ReelConditionsData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

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
        // ��������g�p����e�[�u���ԍ���T��
        public int FindTableToUse(ReelID reelID, ReelDatabase reelDatabase, FlagId flagID, ReelID firstPushReel, int bet, int bonus, int random, int firstPushPos)
        {
            // �������ɂ���(����~��0���Ɣ��肵�Ȃ��̂�1�𑫂�)
            int condition = ConvertConditionData((int)flagID, (int)firstPushReel + 1, bet, bonus, random);

            // �g�p����e�[�u���z��̔ԍ�(-1�̓G���[)
            int foundTable = -1;
            // �������̃e�[�u��
            int currentIndex = 0;

            // Debug.Log("Flag:" + flagID);
            //Debug.Log("FirstPush:" + firstPushReel);
            //Debug.Log("bet:" + bet);
            //Debug.Log("Bonus:" + bonus);
            //Debug.Log("Random:" + random);
            //Debug.Log("Pressed:" + firstPushPos);
            //Debug.Log("Condition:" + condition);

            foreach (ReelConditionsData data in reelDatabase.Conditions)
            {
                //Debug.Log("Search:" + currentIndex);
                // �����������Ă��邩
                bool conditionMet = true;

                for (int i = 0; i < ConditionMaxRead; i++)
                {
                    //Debug.Log("Condition1:" + GetConditionData(condition, i));
                    //Debug.Log("Condition2:" + GetConditionData(data.MainConditions, i));
                    // �t���OID�ȊO�̏�����0���������ꍇ�̓p�X����
                    if (i != (int)ConditionID.Flag && GetConditionData(data.MainConditions, i) == 0)
                    {
                        continue;
                    }
                    // �{�[�i�X������3�Ȃ炢���ꂩ�̃{�[�i�X���������Ă���΃p�X
                    else if (i == (int)ConditionID.Bonus &&
                        GetConditionData(data.MainConditions, i) == BonusAnyValueID &&
                        bonus != (int)BonusTypeID.BonusNone)
                    {
                        //Debug.Log(bonus + "ANY BONUS");
                        continue;
                    }
                    // ����ȊO�͎󂯎�������̂Ə������������m�F����
                    else if (GetConditionData(condition, i) != GetConditionData(data.MainConditions, i))
                    {
                        conditionMet = false;
                    }
                }

                // �����������Ă����
                if (conditionMet)
                {
                    // Debug.Log("All conditions are met");
                    //Debug.Log("FirstReelPosition:" + data.FirstReelPosition);
                    // ���͑���~�̃��[����~�ʒu������
                    // ��~�ʒu������0�Ȃ疳��

                    // ����~�̈ʒu�̕�����1�����V�t�g���A�����̃r�b�g��AND�Z���ď���������(0�ɂȂ�Ȃ���Ώ����𖞂���)
                    int checkValue = 1 << firstPushPos + 1;

                    if (data.FirstReelPosition == 0 || (checkValue & data.FirstReelPosition) != 0)
                    {
                        if (data.FirstReelPosition == 0)
                        {
                            // Debug.Log("No condition");
                        }
                        // �����܂ł�����e�[�u�������B�����ɍX�V����
                        //Debug.Log("Found:" + currentIndex);
                        foundTable = data.ReelTableNumber;
                    }
                }
                currentIndex += 1;
            }
            // ���������[���e�[�u�����L�^
            //Debug.Log("Final Found:" + foundTable);
            UsedReelTableTID[(int)reelID] = foundTable;
            return foundTable;
        }

        // �w�肵�����[���̃f�B���C(�X�x��)��Ԃ�
        public byte GetDelayFromTable(ReelDatabase reelDatabase, int pushedPos, int tableIndex)
        {
            //Debug.Log("Delay:" + reelDatabase.Tables[tableIndex].TableData[pushedPos]);
            return reelDatabase.Tables[tableIndex].TableData[pushedPos];
        }

        // �V�K�t�H�[�}�b�g�ł̓ǂݍ���

        // 
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
                    Debug.Log("First Stop Check");
                    foreach (ReelFirstData first in reelDatabase.FirstCondition)
                    {
                        // ��v����e�[�u�������邽�тɍX�V������
                        if (first.CheckFirstReelCondition((int)flagID, (int)bonus, bet, random, pushedPos))
                        {
                            findTID = first.TID;
                            findCID = first.CID;
                            Debug.Log("Found First TID:" + first.TID + " CID:" + first.CID);
                        }
                    }
                    break;

                // ����~
                case 1:
                    Debug.Log("Second Stop Check");
                    foreach (ReelSecondData second in reelDatabase.SecondCondition)
                    {
                        if (second.CheckSecondReelCondition((int)flagID, (int)bonus, bet, random,
                            PushedReelIdOrder[(int)StopOrder.First], PushedReelCidOrder[(int)StopOrder.First], pushedPos))
                        {
                            findTID = second.TID;
                            findCID = second.CID;
                            Debug.Log("Found Second TID:" + second.TID + " CID:" + second.CID);
                        }
                    }
                    break;

                // ��O��~
                case 2:
                    Debug.Log("Third Stop Check");
                    foreach (ReelThirdData third in reelDatabase.ThirdCondition)
                    {
                        if (third.CheckThirdReelCondition((int)flagID, (int)bonus, bet, random,
                            PushedReelIdOrder[(int)StopOrder.First], PushedReelCidOrder[(int)StopOrder.First],
                            PushedReelIdOrder[(int)StopOrder.Second], PushedReelCidOrder[(int)StopOrder.Second]))
                        {
                            findTID = third.TID;
                            findCID = third.CID;
                            Debug.Log("Found Third TID:" + third.TID + " CID:" + third.CID);
                        }
                    }
                    break;
            }

            // �e�[�u���Ƒg�ݍ��킹ID�������ł����炻�̃e�[�u������X�x���R�}�擾
            if(findTID != -1 && findCID != -1)
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
