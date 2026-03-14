using ReelSpinGame_Reel.Table;
using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Scriptable.Reels
{
    // 第一停止時の条件クラス
    [Serializable]
    public class ReelFirstConditions : ReelBaseConditionData
    {
        // 読み込み位置
        private const int FirstPushReadPos = ConditionMaxRead + 1;          // 第一停止の停止条件
        private const int FirstPushTIDPos = FirstPushReadPos + 1;           // 第一停止のTID
        private const int FirstPushCIDPos = FirstPushTIDPos + 1;            // 第一停止のCID読み込み位置

        [SerializeField] private int firstStopPos;        // 第一停止の停止条件

        public ReelFirstConditions(StreamReader sReader)
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

                // 第一リール停止位置(末端まで読み込む)
                else if (indexNum < FirstPushReadPos)
                {
                    if (value != "ANY")
                    {
                        string[] stopPosData = value.Trim('"').Split(",");
                        foreach (string stop in stopPosData)
                        {
                            firstStopPos += ConvertToArrayBit(int.Parse(stop));
                        }
                    }
                    else
                    {
                        firstStopPos = 0;
                    }
                }

                // TID読み込み
                else if (indexNum < FirstPushTIDPos)
                {
                    TableID = int.Parse(value);
                }

                // CID読み込み
                else if (indexNum < FirstPushCIDPos)
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
        public bool CheckFirstReelCondition(ReelMainCondition mainCondition, int pushedPos)
        {
            // メイン条件チェック
            if (CheckMainCondition(mainCondition))
            {
                // 第一停止の条件が一致するかチェック。0はANY
                // 第一停止の数値をビット演算で比較できるようにする
                int checkValue = 1 << pushedPos + 1;

                // 停止条件を確認する
                if (firstStopPos == NoConditionValue || ((checkValue & firstStopPos) != NoConditionValue))
                {
                    return true;
                }
            }
            return false;
        }
    }
}