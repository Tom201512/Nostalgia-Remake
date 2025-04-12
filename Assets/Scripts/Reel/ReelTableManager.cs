using ReelSpinGame_Reels.Conditions;
using ReelSpinGame_Reels.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReelTableManager
{
    // ���[���e�[�u���Ǘ��p

    // const

    // var
    private List<List<ReelConditionsData>> reelConditions;
    private List<List<ReelTableData>> reelDelayTables;

    // �Ō�Ɏg�p�������[���e�[�u��ID
    public int[] UsedReelTableID { get; private set; }

    // �R���X�g���N�^
    public ReelTableManager(List<StringReader> conditions, List<StringReader> tables)
    {
        // ���X�g�쐬
        reelConditions = new List<List<ReelConditionsData>>();
        reelDelayTables = new List<List<ReelTableData>>();

        UsedReelTableID = new int[ReelManager.ReelAmounts] { 0, 0, 0 };

        // ���[�������ƃe�[�u���̐����������`�F�b�N
        if (conditions.Count != tables.Count)
        {
            throw new Exception("Condition counts and table counts doesn't match");
        }
        
        // �����̓ǂݍ���
        for(int i = 0; i < conditions.Count; i++)
        {
            // �����ǂݍ���
            reelConditions.Add(new List<ReelConditionsData>());

            while (conditions[i].Peek() != -1)
            {
                reelConditions[i].Add(new ReelConditionsData(conditions[i]));
            }

            Debug.Log("Condition:" + i + "Read done" + reelConditions[i].Count);

            // �����ǂݍ���
            reelDelayTables.Add(new List<ReelTableData>());

            while (tables[i].Peek() != -1)
            {
                reelDelayTables[i].Add(new ReelTableData(tables[i]));
            }

            Debug.Log("DelayTable:" + i + "Read done" + reelDelayTables[i].Count);
        }

        Debug.Log("ReelConditions reading done");
        Debug.Log("ReelTable reading done");
    }

    // func
    // ��������g�p����e�[�u���ԍ���T��
    public int FindTableToUse(ReelManager.ReelID reelID, int flagID, int firstPush, int bonus, int bet, int random, int firstPushPos)
    {
        int condition = ReelConditionsData.ConvertConditionData(flagID, firstPush, bonus, bet, random);
        int[] orderToCheck = { flagID, firstPush, bonus, bet, random };

        // �g�p����e�[�u���z��̔ԍ�(-1�̓G���[)
        int foundTable = -1;
        // �������̃e�[�u��
        int currentIndex = 0;

        foreach (ReelConditionsData data in reelConditions[(int)reelID])
        {
            Debug.Log("Search:" + currentIndex);

            // �����������Ă��邩
            bool conditionMet = true;

            for (int i = 0; i < orderToCheck.Length; i++)
            {
                // �t���OID�ȊO�̏�����0�Ȃ�p�X
                if (i != (int)ReelConditionsData.ConditionID.Flag && data.GetConditionData(i) == 0)
                {
                    continue;
                }
                else if (orderToCheck[i] != data.GetConditionData(i))
                {
                    conditionMet = false;
                }
            }

            // �����������Ă����
            if(conditionMet)
            {
                Debug.Log("All conditions are met");

                // ���͑���~�̃��[����~�ʒu������
                // ��~�ʒu������0�Ȃ疳��

                // ����~�̈ʒu�̕�����1�����V�t�g���A�����̃r�b�g��AND�Z���ď���������(0�ɂȂ�Ȃ���Ώ����𖞂���)
                int checkValue = 1 << firstPushPos + 1;

                Debug.Log(checkValue);
                Debug.Log(data.FirstReelPosition);
                Debug.Log(checkValue & data.FirstReelPosition);

                if (data.FirstReelPosition == 0 || (checkValue & data.FirstReelPosition) != 0)
                {
                    if (data.FirstReelPosition == 0)
                    {
                        Debug.Log("No condition");
                    }
                    // �����܂ł�����e�[�u�������B�����ɍX�V����
                    Debug.Log("Found:" + currentIndex);
                    foundTable = data.ReelTableNumber;
                }
            }
            currentIndex += 1;
        }
        // ���������[���e�[�u�����L�^
        Debug.Log("Final Found:" + foundTable);
        UsedReelTableID[(int)reelID] = foundTable;
        return foundTable;
    }

    // �w�肵�����[���̃f�B���C(�X�x��)��Ԃ�
    public byte GetDelayFromTable(ReelManager.ReelID reelID, int pushedPos, int tableIndex)
    {
        Debug.Log("Delay:" + reelDelayTables[(int)reelID][tableIndex].TableData[pushedPos]);
        return reelDelayTables[(int)reelID][tableIndex].TableData[pushedPos];
    }
}
