using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using UnityEngine;

public class ReelTableManager
{
    // ���[���e�[�u���Ǘ��p

    // const

    // var
    // �Ō�Ɏg�p�������[���e�[�u��ID
    public int[] UsedReelTableID { get; private set; }

    // �R���X�g���N�^
    public ReelTableManager()
    {
        UsedReelTableID = new int[ReelManager.ReelAmounts] { 0, 0, 0 };
    }

    // func
    // ��������g�p����e�[�u���ԍ���T��
    public int FindTableToUse(ReelData reel, int flagID, int firstPush, int bonus, int bet, int random, int firstPushPos)
    {
        int condition = ReelConditionsData.ConvertConditionData(flagID, firstPush, bonus, bet, random);
        int[] orderToCheck = { flagID, firstPush, bonus, bet, random };

        // �g�p����e�[�u���z��̔ԍ�(-1�̓G���[)
        int foundTable = -1;
        // �������̃e�[�u��
        int currentIndex = 0;

        foreach (ReelConditionsData data in reel.ReelDatabase.Conditions)
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
        UsedReelTableID[(int)reel.ReelID] = foundTable;
        return foundTable;
    }

    // �w�肵�����[���̃f�B���C(�X�x��)��Ԃ�
    public byte GetDelayFromTable(ReelData reel, int pushedPos, int tableIndex)
    {
        Debug.Log("Delay:" + reel.ReelDatabase.Tables[tableIndex].TableData[pushedPos]);
        return reel.ReelDatabase.Tables[tableIndex].TableData[pushedPos];
    }
}
