using System;
using System.IO;
using UnityEngine;

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
        // 第二停止の停止位置読み込み位置
        private const int SecondPushReedPos = FirstPushedCIDPos + 1;
        // 第二停止のTID読み込み位置
        private const int SecondPushTIDPos = SecondPushReedPos + 1;
        // 第二停止のCID読み込み位置
        private const int SecondPushCIDPos = SecondPushTIDPos + 1;

        // var
        // 第二停止の停止条件

        // 第一停止したリールのID
        [SerializeField] private byte firstStopReelID;
        // 第一停止したリールのCID
        [SerializeField] private byte firstStopCID;
        // 第一停止したリールのCID
        [SerializeField] private int secondStopPos;

        // コンストラクタ
        public ReelSecondData(StringReader sReader)
        {
            // 最初のヘッダは読み込まない
            sReader.ReadLine();
            string[] values = sReader.ReadLine().Split(',');

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
                    secondStopPos = Convert.ToByte(value);
                }

                // 第一したリールのTID読み込み
                else if (indexNum < FirstPushedCIDPos)
                {
                    firstStopCID = Convert.ToByte(value);
                }

                // 第一リール停止
                else if (indexNum < SecondPushReedPos)
                {
                    secondStopPos = GetPosDataFromString(value);
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
        public bool CheckSecondReelCondition(int flagID, int bet, int bonus, int random, int firstStopReelID, int firstStopCID)
        {
            // メイン条件チェック
            if (CheckMainCondition(flagID, bet, bonus, random))
            {
                // 第一停止のリールIDとCIDが一致するかチェック 
                
                if (SecondReelCIDCheck(firstStopReelID, firstStopCID))
                {
                    // 第二停止の条件が一致するかチェック。0はANY
                    // 第二停止の数値をビット演算で比較できるようにする
                    int checkValue = 1 << secondStopPos + 1;
                    return secondStopPos == 0 || (checkValue & secondStopPos) != 0;
                }
            }
            return false;
        }

        // CIDチェック(第二停止用)
        private bool SecondReelCIDCheck(int firstStopReelID, int firstStopCID)
        {
            // 第一停止リールIDの指定が0(ANY)なら無視する
            if (this.firstStopReelID == 0)
            {
                return true;
            }
            else if (firstStopReelID == this.firstStopReelID)
            {
                // 第一停止のCIDが一致、または0ならtrueを返す
                return this.firstStopCID == 0 || firstStopCID == this.firstStopCID;
            }

            return false;
        }
    }
}