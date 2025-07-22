using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSaveData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_Datas.ReelConditionsData;

public class ReelTableManager
{
    // ���[���e�[�u���Ǘ��p

    // const
    // �����ꂩ�̃{�[�i�X�������Ă����������������
    public const int BonusAnyValueID = 3;

    // var
    // �Ō�Ɏg�p�������[���e�[�u��ID
    public int[] UsedReelTableID { get; private set; }

    // �R���X�g���N�^
    public ReelTableManager()
    {
        UsedReelTableID = new int[ReelAmounts] { 0, 0, 0 };
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
                if (i != (int)ConditionID.Flag && GetConditionData(data.MainConditions,i) == 0)
                {
                    continue;
                }
                // �{�[�i�X������3�Ȃ炢���ꂩ�̃{�[�i�X���������Ă���΃p�X
                else if(i == (int)ConditionID.Bonus && 
                    GetConditionData(data.MainConditions, i) == BonusAnyValueID &&
                    bonus != (int)BonusType.BonusNone)
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
            if(conditionMet)
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
        UsedReelTableID[(int)reelID] = foundTable;
        return foundTable;
    }

    // �w�肵�����[���̃f�B���C(�X�x��)��Ԃ�
    public byte GetDelayFromTable(ReelDatabase reelDatabase, int pushedPos, int tableIndex)
    {
        //Debug.Log("Delay:" + reelDatabase.Tables[tableIndex].TableData[pushedPos]);
        return reelDatabase.Tables[tableIndex].TableData[pushedPos];
    }
}
