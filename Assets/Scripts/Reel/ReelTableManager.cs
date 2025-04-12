using ReelSpinGame_Reels.Conditions;
using ReelSpinGame_Reels.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReelTableManager
{
    // リールテーブル管理用

    // const

    // var
    private List<List<ReelConditionsData>> reelConditions;
    private List<List<ReelTableData>> reelDelayTables;

    // 最後に使用したリールテーブルID
    public int[] UsedReelTableID { get; private set; }

    // コンストラクタ
    public ReelTableManager(List<StringReader> conditions, List<StringReader> tables)
    {
        // リスト作成
        reelConditions = new List<List<ReelConditionsData>>();
        reelDelayTables = new List<List<ReelTableData>>();

        UsedReelTableID = new int[ReelManager.ReelAmounts] { 0, 0, 0 };

        // リール条件とテーブルの数が合うかチェック
        if (conditions.Count != tables.Count)
        {
            throw new Exception("Condition counts and table counts doesn't match");
        }
        
        // 条件の読み込み
        for(int i = 0; i < conditions.Count; i++)
        {
            // 条件読み込み
            reelConditions.Add(new List<ReelConditionsData>());

            while (conditions[i].Peek() != -1)
            {
                reelConditions[i].Add(new ReelConditionsData(conditions[i]));
            }

            Debug.Log("Condition:" + i + "Read done" + reelConditions[i].Count);

            // 条件読み込み
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
    // 条件から使用するテーブル番号を探す
    public int FindTableToUse(ReelManager.ReelID reelID, int flagID, int firstPush, int bonus, int bet, int random, int firstPushPos)
    {
        int condition = ReelConditionsData.ConvertConditionData(flagID, firstPush, bonus, bet, random);
        int[] orderToCheck = { flagID, firstPush, bonus, bet, random };

        // 使用するテーブル配列の番号(-1はエラー)
        int foundTable = -1;
        // 検索中のテーブル
        int currentIndex = 0;

        foreach (ReelConditionsData data in reelConditions[(int)reelID])
        {
            Debug.Log("Search:" + currentIndex);

            // 条件が合っているか
            bool conditionMet = true;

            for (int i = 0; i < orderToCheck.Length; i++)
            {
                // フラグID以外の条件は0ならパス
                if (i != (int)ReelConditionsData.ConditionID.Flag && data.GetConditionData(i) == 0)
                {
                    continue;
                }
                else if (orderToCheck[i] != data.GetConditionData(i))
                {
                    conditionMet = false;
                }
            }

            // 条件が合っていれば
            if(conditionMet)
            {
                Debug.Log("All conditions are met");

                // 次は第一停止のリール停止位置を見る
                // 停止位置条件が0なら無視

                // 第一停止の位置の分だけ1を左シフトし、条件のビットとAND算して条件を見る(0にならなければ条件を満たす)
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
                    // ここまできたらテーブル発見。すぐに更新する
                    Debug.Log("Found:" + currentIndex);
                    foundTable = data.ReelTableNumber;
                }
            }
            currentIndex += 1;
        }
        // 見つけたリールテーブルを記録
        Debug.Log("Final Found:" + foundTable);
        UsedReelTableID[(int)reelID] = foundTable;
        return foundTable;
    }

    // 指定したリールのディレイ(スベリ)を返す
    public byte GetDelayFromTable(ReelManager.ReelID reelID, int pushedPos, int tableIndex)
    {
        Debug.Log("Delay:" + reelDelayTables[(int)reelID][tableIndex].TableData[pushedPos]);
        return reelDelayTables[(int)reelID][tableIndex].TableData[pushedPos];
    }
}
