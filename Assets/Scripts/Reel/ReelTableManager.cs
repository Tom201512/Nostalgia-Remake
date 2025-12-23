using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Reels;
using ReelSpinGame_Lots;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelLogicManager;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_Reels.Table
{
    // リールテーブル管理用
    public class ReelTableManager
    {
        // const

        public const int BonusAnyValueID = 3;               // いずれかのボーナスが入っている条件を示す数字
        public enum StopOrder { First, Second, Third }      // 第一、第二、第三停止の配列で使う識別子

        // var

        public int[] UsedReelTableTID { get; private set; }         // 最後に使用したリールテーブルID
        public int[] UsedReelTableCID { get; private set; }         // 各リールの最後に使用した組み合わせID
        public ReelID[] PushedReelIdOrder { get; private set; }     // 停止させたリール順のリールID
        public int[] PushedReelTidOrder { get; private set; }       // テーブルID
        public int[] PushedReelCidOrder { get; private set; }       // 組み合わせID

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
        // スベリコマを得る
        public int GetDelay(int stoppedCount, int pushedPos, ReelDelayTableData reelDatabase,
            FlagID flagID, ReelID pushReelID, BonusTypeID bonus, int bet, int random)
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
                    //Debug.Log("First Stop Check");
                    foreach (ReelFirstConditions first in reelDatabase.FirstCondition)
                    {
                        // 一致するテーブルがあるたびに更新をする
                        if (first.CheckFirstReelCondition((int)flagID, (int)bonus, bet, random, pushedPos))
                        {
                            findTID = first.TID;
                            findCID = first.CID;
                            //Debug.Log("Found First TID:" + first.TID + " CID:" + first.CID);
                        }
                    }
                    break;

                // 第二停止
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

                // 第三停止
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
