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
        // リールテーブル管理用

        // const
        // いずれかのボーナスが入っている条件を示す数字
        public const int BonusAnyValueID = 3;

        // 第一、第二、第三停止の配列で使う識別子
        public enum StopOrder { First, Second, Third }

        // var
        // 最後に使用したリールテーブルID
        public int[] UsedReelTableTID { get; private set; }
        // 各リールの最後に使用した組み合わせID
        public int[] UsedReelTableCID { get; private set; }

        // 停止させたリール順のリールID
        public ReelID[] PushedReelIdOrder { get; private set; }
        // テーブルID
        public int[] PushedReelTidOrder { get; private set; }
        // 組み合わせID
        public int[] PushedReelCidOrder { get; private set; }

        // コンストラクタ
        public ReelTableManager()
        {
            UsedReelTableTID = new int[ReelAmount] { 0, 0, 0 };
            UsedReelTableCID = new int[ReelAmount] { 0, 0, 0 };

            PushedReelIdOrder = new ReelID[ReelAmount] {ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            PushedReelTidOrder = new int[ReelAmount] { 0, 0, 0 };
            PushedReelCidOrder = new int[ReelAmount] { 0, 0, 0 };
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
                    if (i != (int)ConditionID.Flag && GetConditionData(data.MainConditions, i) == 0)
                    {
                        continue;
                    }
                    // ボーナス条件は3ならいずれかのボーナスが成立していればパス
                    else if (i == (int)ConditionID.Bonus &&
                        GetConditionData(data.MainConditions, i) == BonusAnyValueID &&
                        bonus != (int)BonusTypeID.BonusNone)
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
                if (conditionMet)
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
            UsedReelTableTID[(int)reelID] = foundTable;
            return foundTable;
        }

        // 指定したリールのディレイ(スベリ)を返す
        public byte GetDelayFromTable(ReelDatabase reelDatabase, int pushedPos, int tableIndex)
        {
            //Debug.Log("Delay:" + reelDatabase.Tables[tableIndex].TableData[pushedPos]);
            return reelDatabase.Tables[tableIndex].TableData[pushedPos];
        }

        // 新規フォーマットでの読み込み

        // 
        public int GetDelay(int stoppedCount, int pushedPos, ReelDatabase reelDatabase,
            FlagId flagID, ReelID pushReelID, BonusTypeID bonus, int bet, int random)
        {
            // 見つけたテーブルID
            int findTID = -1;
            // 見つけた組み合わせID
            int findCID = -1;

            // 停止順ごとの条件判定をする
            switch (stoppedCount)
            {
                // 第一停止
                case 0:
                    Debug.Log("First Stop Check");
                    foreach (ReelFirstData first in reelDatabase.FirstCondition)
                    {
                        // 一致するテーブルがあるたびに更新をする
                        if (first.CheckFirstReelCondition((int)flagID, (int)bonus, bet, random, pushedPos))
                        {
                            findTID = first.TID;
                            findCID = first.CID;
                            Debug.Log("Found First TID:" + first.TID + " CID:" + first.CID);
                        }
                    }
                    break;

                // 第二停止
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

                // 第三停止
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

            // テーブルと組み合わせIDが発見できたらそのテーブルからスベリコマ取得
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
