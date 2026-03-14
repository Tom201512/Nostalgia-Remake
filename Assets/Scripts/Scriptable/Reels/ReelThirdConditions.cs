using ReelSpinGame_Reel;
using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Scriptable.Reels
{
    // 第三停止時の条件クラス
    [Serializable]
    public class ReelThirdConditions : ReelBaseConditionData
    {
        // 読み込み位置
        const int FirstPushedReelIDPos = ConditionMaxRead + 1;          // 第一停止したリールID
        const int FirstPushedCIDPos = FirstPushedReelIDPos + 1;         // 第一停止したリールのCID
        const int SecondPushedReedPos = FirstPushedCIDPos + 1;          // 第二停止したリールID
        const int SecondPushedTIDPos = SecondPushedReedPos + 1;         // 第二停止したリールのCID
        const int ThirdPushTIDPos = SecondPushedTIDPos + 1;             // 第三停止のTID
        const int ThirdPushCIDPos = ThirdPushTIDPos + 1;                // 第三停止のCID

        [SerializeField] private int firstStopReelID;      // 第一停止したリールのID
        [SerializeField] private int firstStopCID;         // 第一停止したリールのCID
        [SerializeField] private int secondStopReelID;     // 第二停止したリールのID
        [SerializeField] private int secondStopCID;        // 第二停止したリールのCID

        public ReelThirdConditions(StreamReader sReader)
        {
            string[] values = GetDataFromStream(sReader);

            int indexNum = 0;
            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    MainConditions += int.Parse(value) << (4 * indexNum);
                }

                // 第一したリールのID読み込み
                else if (indexNum < FirstPushedReelIDPos)
                {
                    firstStopReelID = int.Parse(value);
                }

                // 第一したリールのTID読み込み
                else if (indexNum < FirstPushedCIDPos)
                {
                    firstStopCID = int.Parse(value);
                }

                // 第二したリールのID読み込み
                else if (indexNum < SecondPushedReedPos)
                {
                    secondStopReelID = int.Parse(value);
                }

                // 第二したリールのTID読み込み
                else if (indexNum < SecondPushedTIDPos)
                {
                    secondStopCID = int.Parse(value);
                }

                // 第三停止のTID
                else if (indexNum < ThirdPushTIDPos)
                {
                    TableID = int.Parse(value);
                }

                // 第三停止のCID
                else if (indexNum < ThirdPushCIDPos)
                {
                    CombinationID = int.Parse(value);
                }

                // 最後の部分は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum += 1;
            }
        }

        // 条件チェック
        public bool CheckThirdReelCondition(ReelMainCondition mainCondition, ReelID firstStopReelID, int firstStopCID, ReelID secondStopReelID, int secondStopReelCID)
        {
            // メイン条件チェック
            if (CheckMainCondition(mainCondition))
            {
                // 第一と第二停止のリールID, CIDが一致するかチェック
                return ThirdReelCIDCheck(firstStopReelID, firstStopCID, secondStopReelID, secondStopReelCID);
            }
            return false;
        }

        // CIDチェック(第三停止用)
        private bool ThirdReelCIDCheck(ReelID firstStopReelID, int firstStopCID, ReelID secondStopReelID, int secondStopCID)
        {
            bool first;
            bool second;

            // 第一停止リールIDの条件が正しいかチェック(0はANY)
            if (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID)
            {
                // 第一停止のCIDをチェック
                first = this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }
            else
            {
                return false;
            }

            // 第二停止リールIDの条件が正しいかチェック(0はANY)
            if (this.secondStopReelID == 0 || (int)secondStopReelID + 1 == this.secondStopReelID)
            {
                // 第二停止のCIDをチェック
                second = this.secondStopCID == 0 || secondStopCID == this.secondStopCID;
            }
            else
            {
                return false;
            }

            // 双方の結果がtrueなら条件一致
            return first && second;
        }
    }
}