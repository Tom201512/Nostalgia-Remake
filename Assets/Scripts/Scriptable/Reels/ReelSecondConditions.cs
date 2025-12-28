using ReelSpinGame_Reels;
using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelSecondConditions : ReelBaseData
    {
        // 読み込み位置
        const int FirstPushedReelIDPos = ConditionMaxRead + 1;      // 第一停止したリールID
        const int FirstPushedCIDPos = FirstPushedReelIDPos + 1;     // 第一停止したリールのCID
        const int SecondPushReadPos = FirstPushedCIDPos + 1;        // 第二停止の停止位置
        const int SecondPushTIDPos = SecondPushReadPos + 1;         // 第二停止のTID
        const int SecondPushCIDPos = SecondPushTIDPos + 1;          // 第二停止のCID

        // 第二停止の停止条件
        [SerializeField] private byte firstStopReelID;      // 第一停止したリールのID
        [SerializeField] private byte firstStopCID;         // 第一停止したリールのCID
        [SerializeField] private int secondStopPos;         // 第二停止の停止位置

        // コンストラクタ
        public ReelSecondConditions(StreamReader sReader)
        {
            string[] values = GetDataFromStream(sReader);

            int indexNum = 0;
            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // 第一したリールのID読み込み
                else if (indexNum < FirstPushedReelIDPos)
                {
                    firstStopReelID = Convert.ToByte(value);
                }

                // 第一したリールのTID読み込み
                else if (indexNum < FirstPushedCIDPos)
                {
                    firstStopCID = Convert.ToByte(value);
                }

                // 第二リール停止位置(末端まで読み込む)
                else if (indexNum < SecondPushReadPos)
                {
                    if (value != "ANY")
                    {
                        string[] stopPosData = value.Trim('"').Split(",");
                        foreach (string stop in stopPosData)
                        {
                            secondStopPos += ConvertToArrayBit(Convert.ToInt32(stop));
                        }
                    }
                    else
                    {
                        secondStopPos = 0;
                    }
                }

                // TID読み込み
                else if (indexNum < SecondPushTIDPos)
                {
                    TID = Convert.ToByte(value);
                }

                // CID読み込み
                else if (indexNum < SecondPushCIDPos)
                {
                    CID = Convert.ToByte(value);
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
        public bool CheckSecondReelCondition(ReelMainCondition mainCondition, ReelID firstStopReelID, int firstStopCID, int pushedPos)
        {
            // メイン条件チェック
            if (CheckMainCondition(mainCondition))
            {
                // 第一停止のリールIDとCIDが一致するかチェック 
                if (SecondReelCIDCheck(firstStopReelID, firstStopCID))
                {
                    // 第二停止の条件が一致するかチェック。0はANY
                    // 第二停止の数値をビット演算で比較できるようにする
                    int checkValue = 1 << pushedPos + 1;

                    // 停止条件を確認
                    if (secondStopPos == 0 || ((checkValue & secondStopPos) != 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // CIDチェック(第二停止用)
        private bool SecondReelCIDCheck(ReelID firstStopReelID, int firstStopCID)
        {
            // 第一停止リールIDの条件が正しいかチェック(0はANY)
            if (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID)
            {
                // 第一停止のCIDをチェック
                return this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }
            else
            {
                return false;
            }
        }
    }
}