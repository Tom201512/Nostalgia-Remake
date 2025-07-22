using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSaveData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_Datas.ReelConditionsData;

public class ReelTableManager
{
    // リールテーブル管理用

    // const
    // いずれかのボーナスが入っている条件を示す数字
    public const int BonusAnyValueID = 3;

    // var
    // 最後に使用したリールテーブルID
    public int[] UsedReelTableID { get; private set; }

    // コンストラクタ
    public ReelTableManager()
    {
        UsedReelTableID = new int[ReelAmounts] { 0, 0, 0 };
    }

    // func
    // 条件から使用するテーブル番号を探す
    public int FindTableToUse(ReelID reelID, ReelDatabase reelDatabase, FlagId flagID, ReelID firstPushReel, int bet, int bonus, int random, int firstPushPos)
    {
        // 条件文にする(第一停止は0だと判定しないので1を足す)
        int condition = ConvertConditionData((int)flagID, (int)firstPushReel + 1, bet, bonus, random);

        // 使用するテーブル配列の番号(-1はエラー)
        int foundTable = -1;
        // 検索中のテーブル
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
            // 条件が合っているか
            bool conditionMet = true;

            for (int i = 0; i < ConditionMaxRead; i++)
            {
                //Debug.Log("Condition1:" + GetConditionData(condition, i));
                //Debug.Log("Condition2:" + GetConditionData(data.MainConditions, i));
                // フラグID以外の条件で0があった場合はパスする
                if (i != (int)ConditionID.Flag && GetConditionData(data.MainConditions,i) == 0)
                {
                    continue;
                }
                // ボーナス条件は3ならいずれかのボーナスが成立していればパス
                else if(i == (int)ConditionID.Bonus && 
                    GetConditionData(data.MainConditions, i) == BonusAnyValueID &&
                    bonus != (int)BonusType.BonusNone)
                {
                    //Debug.Log(bonus + "ANY BONUS");
                    continue;
                }
                // それ以外は受け取ったものと条件が合うか確認する
                else if (GetConditionData(condition, i) != GetConditionData(data.MainConditions, i))
                {
                    conditionMet = false;
                }
            }

            // 条件が合っていれば
            if(conditionMet)
            {
               // Debug.Log("All conditions are met");
                //Debug.Log("FirstReelPosition:" + data.FirstReelPosition);
                // 次は第一停止のリール停止位置を見る
                // 停止位置条件が0なら無視

                // 第一停止の位置の分だけ1を左シフトし、条件のビットとAND算して条件を見る(0にならなければ条件を満たす)
                int checkValue = 1 << firstPushPos + 1;

                if (data.FirstReelPosition == 0 || (checkValue & data.FirstReelPosition) != 0)
                {
                    if (data.FirstReelPosition == 0)
                    {
                       // Debug.Log("No condition");
                    }
                    // ここまできたらテーブル発見。すぐに更新する
                    //Debug.Log("Found:" + currentIndex);
                    foundTable = data.ReelTableNumber;
                }
            }
            currentIndex += 1;
        }
        // 見つけたリールテーブルを記録
        //Debug.Log("Final Found:" + foundTable);
        UsedReelTableID[(int)reelID] = foundTable;
        return foundTable;
    }

    // 指定したリールのディレイ(スベリ)を返す
    public byte GetDelayFromTable(ReelDatabase reelDatabase, int pushedPos, int tableIndex)
    {
        //Debug.Log("Delay:" + reelDatabase.Tables[tableIndex].TableData[pushedPos]);
        return reelDatabase.Tables[tableIndex].TableData[pushedPos];
    }
}
