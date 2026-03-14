using ReelSpinGame_Scriptable.Reels;

namespace ReelSpinGame_Reel.Table
{
    // リールテーブルマネージャーのデータ
    public class ReelTableModel
    {
        public const int BonusAnyValueID = 3;               // いずれかのボーナスが入っている条件を示す数字
        public enum StopOrder { First, Second, Third }      // 第一、第二、第三停止の配列で使う識別子

        public ReelID[] PushedReelIdOrder { get; private set; }                 // 停止させたリール順のリールID
        public int[] PushedReelTidOrder { get; private set; }                   // テーブルID
        public int[] PushedReelCidOrder { get; private set; }                   // 組み合わせID

        public ReelTableModel()
        {
            PushedReelIdOrder = new ReelID[ReelManager.ReelAmount] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            PushedReelTidOrder = new int[ReelManager.ReelAmount] { 0, 0, 0 };
            PushedReelCidOrder = new int[ReelManager.ReelAmount] { 0, 0, 0 };
        }

        // スベリコマを得る
        public int GetDelay(ReelID reelID, int stoppedCount, int pushedPos, ReelDelayTableFile reelDatabase, ReelMainCondition mainCondition)
        {
            FindTableData findTableData;

            // 停止順ごとの条件判定をする
            switch (stoppedCount)
            {
                // 第一停止
                case (int)StopOrder.First:
                    findTableData = CheckFirstStopCondition(mainCondition, reelDatabase, pushedPos);
                    break;

                // 第二停止
                case (int)StopOrder.Second:
                    findTableData = CheckSecondStopCondition(mainCondition, reelDatabase, pushedPos);
                    break;

                // 第三停止
                case (int)StopOrder.Third:
                    findTableData = CheckThirdStopCondition(mainCondition, reelDatabase, pushedPos);
                    break;

                default:
                    findTableData = new FindTableData();
                    break;
            }

            // テーブルと組み合わせIDが発見できたらそのテーブルからスベリコマ取得
            if (findTableData.IsTableFound())
            {
                PushedReelIdOrder[stoppedCount] = reelID;
                PushedReelTidOrder[stoppedCount] = findTableData.TableID;
                PushedReelCidOrder[stoppedCount] = findTableData.CombinationID;

                return reelDatabase.Tables[findTableData.TableID - 1].TableData[pushedPos];
            }
            else
            {
                throw new System.Exception("No table found");
            }
        }

        // 第一停止のチェック
        private FindTableData CheckFirstStopCondition(ReelMainCondition mainCondition, ReelDelayTableFile reelDatabase, int pushedPos)
        {
            FindTableData findTableData = new FindTableData();
            foreach (ReelFirstConditions first in reelDatabase.FirstCondition)
            {
                // 一致するテーブルがあるたびに更新をする
                if (first.CheckFirstReelCondition(mainCondition, pushedPos))
                {
                    findTableData.TableID = first.TableID;
                    findTableData.CombinationID = first.CombinationID;
                }
            }

            return findTableData;
        }

        // 第二停止のチェック
        private FindTableData CheckSecondStopCondition(ReelMainCondition mainCondition, ReelDelayTableFile reelDatabase, int pushedPos)
        {
            FindTableData findTableData = new FindTableData();
            foreach (ReelSecondConditions second in reelDatabase.SecondCondition)
            {
                if (second.CheckSecondReelCondition(mainCondition, PushedReelIdOrder[(int)StopOrder.First],
                    PushedReelCidOrder[(int)StopOrder.First], pushedPos))
                {
                    findTableData.TableID = second.TableID;
                    findTableData.CombinationID = second.CombinationID;
                }
            }
            return findTableData;
        }

        // 第三停止のチェック
        private FindTableData CheckThirdStopCondition(ReelMainCondition mainCondition, ReelDelayTableFile reelDatabase, int pushedPos)
        {
            FindTableData findTableData = new FindTableData();
            foreach (ReelThirdConditions third in reelDatabase.ThirdCondition)
            {
                if (third.CheckThirdReelCondition(mainCondition,
                    PushedReelIdOrder[(int)StopOrder.First], PushedReelCidOrder[(int)StopOrder.First],
                    PushedReelIdOrder[(int)StopOrder.Second], PushedReelCidOrder[(int)StopOrder.Second]))
                {
                    findTableData.TableID = third.TableID;
                    findTableData.CombinationID = third.CombinationID;
                }
            }
            return findTableData;
        }
    }
}