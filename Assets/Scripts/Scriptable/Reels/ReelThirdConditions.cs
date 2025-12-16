using System;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelThirdConditions : ReelBaseData
    {
        // const
        // 第一停止したリールIDを読み込む位置
        private const int FirstPushedReelIDPos = ConditionMaxRead + 1;
        // 第一停止したリールのCIDを読み込む位置
        private const int FirstPushedCIDPos = FirstPushedReelIDPos + 1;
        // 第二停止したリールIDを読み込む位置
        private const int SecondPushedReedPos = FirstPushedCIDPos + 1;
        // 第二停止したリールのCIDを読み込む位置
        private const int SecondPushedTIDPos = SecondPushedReedPos + 1;
        // 第三停止のTID読み込み位置
        private const int ThirdPushTIDPos = SecondPushedTIDPos + 1;
        // 第三停止のCID読み込み位置
        private const int ThirdPushCIDPos = ThirdPushTIDPos + 1;

        // var
        // 第二停止の停止条件

        // 第一停止したリールのID
        [SerializeField] private byte firstStopReelID;
        // 第一停止したリールのCID
        [SerializeField] private byte firstStopCID;
        // 第二停止したリールのID
        [SerializeField] private byte secondStopReelID;
        // 第二停止したリールのCID
        [SerializeField] private byte secondStopCID;

        // コンストラクタ
        public ReelThirdConditions(StreamReader sReader)
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

                // 第二したリールのID読み込み
                else if (indexNum < SecondPushedReedPos)
                {
                    secondStopReelID = Convert.ToByte(value);
                }

                // 第二したリールのTID読み込み
                else if (indexNum < SecondPushedTIDPos)
                {
                    secondStopCID = Convert.ToByte(value);
                }

                // 第三停止のTID
                else if (indexNum < ThirdPushTIDPos)
                {
                    TID = Convert.ToByte(value);
                }

                // 第三停止のCID
                else if (indexNum < ThirdPushCIDPos)
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
            //Debug.Log("SecondPushedReelID:" + secondStopReelID);
            //Debug.Log("SecondPushedCID:" + secondStopCID);
            //Debug.Log("TID:" + TID);
            //Debug.Log("CID:" + CID);

            //Debug.Log("Third Push Load Done");
        }

        // 条件チェック
        public bool CheckThirdReelCondition(int flagID, int bonus, int bet, int random, 
            ReelID firstStopReelID, int firstStopCID, ReelID secondStopReelID, int secondStopReelCID)
        {
            // メイン条件チェック
            if (CheckMainCondition(flagID, bonus, bet, random))
            {
                // 第一と第二停止のリールID, CIDが一致するかチェック
                return ThirdReelCIDCheck(firstStopReelID, firstStopCID, secondStopReelID, secondStopReelCID);
            }
            return false;
        }

        // CIDチェック(第三停止用)
        private bool ThirdReelCIDCheck(ReelID firstStopReelID, int firstStopCID, ReelID secondStopReelID, int secondStopCID)
        {
            bool first = false;
            bool second = false;

            // 第一停止

            //Debug.Log("FirstStopReel Condition: " + this.firstStopReelID);
            //Debug.Log("FirstStop Check: " + (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID));

            // 第一停止リールIDの条件が正しいかチェック(0はANY)
            if (this.firstStopReelID == 0 || (int)firstStopReelID + 1 == this.firstStopReelID)
            {
                // 第一停止のCIDをチェック
                //Debug.Log("CID Check" + (this.firstStopCID == 0 || firstStopCID == this.firstStopCID));
                first = this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }
            else
            {
                return false;
            }

            // 第二停止

            //Debug.Log("SecondStopReel Condition: " + this.secondStopReelID);
            //Debug.Log("SecondStop Check: " + (this.secondStopReelID == 0 || (int)secondStopReelID + 1 == this.secondStopReelID));

            // 第二停止リールIDの条件が正しいかチェック(0はANY)
            if (this.secondStopReelID == 0 || (int)secondStopReelID + 1 == this.secondStopReelID)
            {
                // 第二停止のCIDをチェック
                //Debug.Log("CID Check" + (this.secondStopCID == 0 || firstStopCID == this.secondStopCID));
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
