using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Reels;
using static ReelSpinGame_Reels.ReelLogicManager;

namespace ReelSpinGame_Reels.Table
{
    // リールテーブル管理用
    public class ReelTableManager
    {
        public const int BonusAnyValueID = 3;               // いずれかのボーナスが入っている条件を示す数字
        public enum StopOrder { First, Second, Third }      // 第一、第二、第三停止の配列で使う識別子

        public int[] UsedReelTableTID { get; private set; }         // 最後に使用したリールテーブルID
        public int[] UsedReelTableCID { get; private set; }         // 各リールの最後に使用した組み合わせID
        public ReelID[] PushedReelIdOrder { get; private set; }     // 停止させたリール順のリールID
        public int[] PushedReelTidOrder { get; private set; }       // テーブルID
        public int[] PushedReelCidOrder { get; private set; }       // 組み合わせID

        public ReelTableManager()
        {
            UsedReelTableTID = new int[ReelAmount] { 0, 0, 0 };
            UsedReelTableCID = new int[ReelAmount] { 0, 0, 0 };

            PushedReelIdOrder = new ReelID[ReelAmount] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            PushedReelTidOrder = new int[ReelAmount] { 0, 0, 0 };
            PushedReelCidOrder = new int[ReelAmount] { 0, 0, 0 };
        }

        // スベリコマを得る
        public int GetDelay(ReelID pushReelID, int stoppedCount, int pushedPos, ReelDelayTableData reelDatabase, ReelMainCondition mainCondition)
        {
            int findTID = -1;       // 見つけたテーブルID
            int findCID = -1;       // 見つけた組み合わせID

            // 停止順ごとの条件判定をする
            switch (stoppedCount)
            {
                // 第一停止
                case 0:
                    foreach (ReelFirstConditions first in reelDatabase.FirstCondition)
                    {
                        // 一致するテーブルがあるたびに更新をする
                        if (first.CheckFirstReelCondition(mainCondition, pushedPos))
                        {
                            findTID = first.TID;
                            findCID = first.CID;
                        }
                    }
                    break;

                // 第二停止
                case 1:
                    foreach (ReelSecondConditions second in reelDatabase.SecondCondition)
                    {
                        if (second.CheckSecondReelCondition(mainCondition, PushedReelIdOrder[(int)StopOrder.First],
                            PushedReelCidOrder[(int)StopOrder.First], pushedPos))
                        {
                            findTID = second.TID;
                            findCID = second.CID;
                        }
                    }
                    break;

                // 第三停止
                case 2:
                    foreach (ReelThirdConditions third in reelDatabase.ThirdCondition)
                    {
                        if (third.CheckThirdReelCondition(mainCondition,
                            PushedReelIdOrder[(int)StopOrder.First], PushedReelCidOrder[(int)StopOrder.First],
                            PushedReelIdOrder[(int)StopOrder.Second], PushedReelCidOrder[(int)StopOrder.Second]))
                        {
                            findTID = third.TID;
                            findCID = third.CID;
                        }
                    }
                    break;
            }

            // テーブルと組み合わせIDが発見できたらそのテーブルからスベリコマ取得
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
