using UnityEngine;

namespace ReelSpinGame_Reel.Table
{
    // リールテーブル管理用
    public class ReelTableManager : MonoBehaviour
    {
        [SerializeField] ReelGroupAccessor reelGroup;       // リールオブジェクトのグループ
        private ReelTableModel reelTableModel;              // リールテーブルのモデル

        private void Awake()
        {
            reelTableModel = new ReelTableModel();
        }

        // スベリコマを得る
        public int GetDelay(ReelID reelID, int stoppedCount, int pushedPos, ReelMainCondition mainCondition)
        {
            return reelTableModel.GetDelay(reelID, stoppedCount, pushedPos, reelGroup.GetReelTableData(reelID), mainCondition);
        }
    }
}