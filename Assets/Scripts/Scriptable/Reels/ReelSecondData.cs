using System;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelSecondData : ReelBaseData
    {
        // const
        // 第一停止したリールIDを読み込む位置
        private const int FirstPushedReelIDPos = ConditionMaxRead + 1;
        // 第一停止したリールのCIDを読み込む位置
        private const int FirstPushedCIDPos = FirstPushedReelIDPos + 1;
        // 第二停止の停止位置条件読み込み位置
        private const int SecondPushReadPos = FirstPushedCIDPos + 1;
        // 第二停止のTID読み込み位置
        private const int SecondPushTIDPos = SecondPushReadPos + 1;
        // 第二停止のCID読み込み位置
        private const int SecondPushCIDPos = SecondPushTIDPos + 1;

        // var
        // 第二停止の停止条件

        // 第一停止したリールのID
        [SerializeField] private byte firstStopReelID;
        // 第一停止したリールのCID
        [SerializeField] private byte firstStopCID;
        // 第二停止の停止条件
        [SerializeField] private int secondStopPos;

        // コンストラクタ
        public ReelSecondData(StreamReader sReader)
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
                    //Debug.Log("SecondStopPos:" + secondStopPos);
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

            //Debug.Log("MainCondition:" + MainConditions);
            //Debug.Log("FirstPushedReelID:" + firstStopReelID);
            //Debug.Log("FirstPushedCID:" + firstStopCID);
            //Debug.Log("SecondStopPos:" + secondStopPos);
            //Debug.Log("TID:" + TID);
            //Debug.Log("CID:" + CID);

            //Debug.Log("Second Push Load Done");
        }

        // 条件チェック
        public bool CheckSecondReelCondition(int flagID, int bonus, int bet, int random, ReelID firstStopReelID, int firstStopCID, int pushedPos)
        {
            // メイン条件チェック
            if (CheckMainCondition(flagID, bonus, bet, random))
            {
                // 第一停止のリールIDとCIDが一致するかチェック 

                //Debug.Log("FirstPush:" + firstStopReelID + "CID:" + firstStopCID);

                if (SecondReelCIDCheck(firstStopReelID, firstStopCID))
                {
                    //Debug.Log("Pushed:" + pushedPos);
                    // 第二停止の条件が一致するかチェック。0はANY
                    // 第二停止の数値をビット演算で比較できるようにする
                    int checkValue = 1 << pushedPos + 1;
                    //Debug.Log("check:" + checkValue);
                    //Debug.Log("SecondPushPos:" + secondStopPos);

                    // 停止条件を確認
                    if (secondStopPos == 0 || ((checkValue & secondStopPos) != 0))
                    {
                        //Debug.Log("Stop Pos has match with condition");
                        // 条件一致
                        return true;
                    }
                }
            }
            return false;
        }

        // CIDチェック(第二停止用)
        private bool SecondReelCIDCheck(ReelID firstStopReelID, int firstStopCID)
        {
            //Debug.Log("FirstStopReel Condition: " + this.firstStopReelID);
            //Debug.Log("FirstStop Check: " + (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID));

            // 第一停止リールIDの条件が正しいかチェック(0はANY)
            if (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID)
            {
                // 第一停止のCIDをチェック
                //Debug.Log("CID Check" + (this.firstStopCID == 0 || firstStopCID == this.firstStopCID));
                return this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }
            else
            {
                return false;
            }
        }
    }
}