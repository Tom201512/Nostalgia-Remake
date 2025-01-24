using System.Collections.ObjectModel;
using UnityEngine;
using ReelSpinGame_Reels.ReelArray;

namespace ReelSpinGame_Reels
{
    public class ReelData
    {
        // リールのデータ *

        // 目指す目標

        // 1:リールの回転
        // 2:図柄の実装
        // 3:リールの停止
        // 4:スベリ実装
        // 5:テーブル機能搭載

        // 各リールデータが持つもの

        // リールの現在位置(下段), スベリ数, 停止するか
        // そのリールの配列
        // リール制御テーブル
        // 制御条件テーブル

        // const 

        // リール位置識別用
        public enum ReelPosID { Lower3rd = -2, Lower2nd, Lower, Center, Upper, Upper2nd, Upper3rd }


        // var

        // 現在の下段リール位置
        private int currentLower;


        // リール配列
        public ReadOnlyCollection<ReelArray.ReelArray.ReelSymbols> Array { get; private set; }


        public ReelData(int lowerPos, ReadOnlyCollection<ReelArray.ReelArray.ReelSymbols> arrayData) 
        {
            // もし位置が0~20でなければ例外を出す
            if(lowerPos < 0 ||  lowerPos > ReelArray.ReelArray.MaxReelArray - 1)
            {
                throw new System.Exception("Invalid Position num. Must be within 0 ~ " + (ReelArray.ReelArray.MaxReelArray - 1));
            }

            currentLower = lowerPos;

            Array = arrayData;

            Debug.Log("ReelGenerated Position at:" + lowerPos);

            for(int i = 0; i < arrayData.Count; i++)
            {
                Debug.Log("No." + i + " Symbol:" + arrayData[i]);
            }
        }


        // func

        // リール位置変更
        public void ChangeReelPos(float rotateSpeed)
        {
            // 回転速度の符号に合わせて位置を変更
           currentLower = OffsetReel((int)Mathf.Sign(rotateSpeed));
            Debug.Log("Changed Reel to :" + currentLower);
        }


        // 指定したリールの位置番号を返す
        public int GetReelPos(ReelPosID posID) => OffsetReel((int)posID);

        private int OffsetReel(int offset)
        {
            if (currentLower + offset < 0)
            {
                Debug.Log("Lower than 0 Reel:" + ReelArray.ReelArray.MaxReelArray + currentLower + offset);
                return ReelArray.ReelArray.MaxReelArray + currentLower + offset;
            }

            else if (currentLower + offset > ReelArray.ReelArray.MaxReelArray - 1)
            {
                Debug.Log("Higher than" + ReelArray.ReelArray.MaxReelArray + " Reel:" + (currentLower + offset - ReelArray.ReelArray.MaxReelArray));
                return currentLower + offset - ReelArray.ReelArray.MaxReelArray;
            }

            // オーバーフローがないならそのまま返す
            Debug.Log("No Overflow" + (currentLower + offset - ReelArray.ReelArray.MaxReelArray));
            return currentLower + offset;
        }
    }
}
