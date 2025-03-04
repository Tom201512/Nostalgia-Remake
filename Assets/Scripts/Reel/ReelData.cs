using ReelSpinGame_Rules;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Reels
{
    public class ReelData
    {
        // リールのデータ

        // const 
        // リール配列要素数
        public const int MaxReelArray = 21;

        // 図柄
        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay }
        // リール位置識別用
        public enum ReelPosID { Lower3rd = -2, Lower2nd, Lower, Center, Upper, Upper2nd, Upper3rd }

        // var
        // リール配列
        public byte[] ReelArray { get; private set; }
        // 現在の下段リール位置
        private int currentLower;

        // コンストラクタ
        public ReelData(int lowerPos, StreamReader arrayData) 
        {
            // 位置設定
            // もし位置が0~20でなければ例外を出す
            if(lowerPos < 0 ||  lowerPos > MaxReelArray - 1)
            {
                throw new System.Exception("Invalid Position num. Must be within 0 ~ " + (MaxReelArray - 1));
            }
            currentLower = lowerPos;

            // 配列読み込み
            string[] values = arrayData.ReadLine().Split(',');
            // 配列に変換
            ReelArray = Array.ConvertAll(values, byte.Parse);
            
            foreach (byte value in ReelArray)
            {
                //Debug.Log(value + "Symbol:" + ReturnSymbol(value));
            }

            Debug.Log("ReelGenerated Position at:" + lowerPos);

            for(int i = 0; i < ReelArray.Length; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReturnSymbol(ReelArray[i]));
            }
        }

        // func

        // リール配列の番号を図柄へ変更
        private ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);

        // 指定したリールの位置番号を返す
        public int GetReelPos(int posID) => OffsetReel(posID);
        // リールの位置から図柄を返す
        public ReelSymbols GetReelSymbol(int posID) => ReturnSymbol(ReelArray[OffsetReel(posID)]);
        // リール位置変更 (回転速度の符号に合わせて変更)
        public void ChangeReelPos(float rotateSpeed) => currentLower = OffsetReel((sbyte)Mathf.Sign(rotateSpeed));

        // リール位置をオーバーフローしない数値で返す
        private int OffsetReel(int offset)
        {
            if (currentLower + offset < 0)
            {
                return MaxReelArray + currentLower + offset;
            }

            else if (currentLower + offset > MaxReelArray - 1)
            {
                return currentLower + offset - MaxReelArray;
            }
            // オーバーフローがないならそのまま返す
            return currentLower + offset;
        }
    }
}
