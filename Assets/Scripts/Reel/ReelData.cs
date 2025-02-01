using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

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
        // 5:配当チェック
        // 6:テーブル機能搭載


        // 各リールデータが持つもの

        // リールの現在位置(下段), スベリ数, 停止するか
        // そのリールの配列
        // リール制御テーブル
        // 制御条件テーブル

        // const 

        // リール配列

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
            // もし位置が0~20でなければ例外を出す
            if(lowerPos < 0 ||  lowerPos > MaxReelArray - 1)
            {
                throw new System.Exception("Invalid Position num. Must be within 0 ~ " + (MaxReelArray - 1));
            }

            currentLower = lowerPos;

            // データ読み込み
            string[] values = arrayData.ReadLine().Split(',');

            ReelArray = Array.ConvertAll(values, byte.Parse);

            foreach (byte value in ReelArray)
            {
                // リールのIDを読み込む
                Debug.Log(value + "Symbol:" + ReturnSymbol(value));
            }

            Debug.Log("ReelGenerated Position at:" + lowerPos);

            for(int i = 0; i < ReelArray.Length; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReturnSymbol(ReelArray[i]));
            }
        }


        // func

        // 指定したリールの位置番号を返す
        public int GetReelPos(sbyte posID) => OffsetReel((int)posID);

        // sbyte型から位置指定
        public ReelSymbols GetReelSymbol(sbyte posID) => ReturnSymbol(ReelArray[OffsetReel(posID)]);

        // リール位置変更 (回転速度の符号に合わせて変更)
        public void ChangeReelPos(float rotateSpeed) => currentLower = OffsetReel((int)Mathf.Sign(rotateSpeed));

        // リール配列の番号を図柄へ変更
        private ReelSymbols ReturnSymbol(byte reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);


        //オーバーフロー対策
        private int OffsetReel(int offset)
        {
            if (currentLower + offset < 0)
            {
                //Debug.Log("Lower than 0 Reel:" + MaxReelArray + currentLower + offset);
                return MaxReelArray + currentLower + offset;
            }

            else if (currentLower + offset > MaxReelArray - 1)
            {
                //Debug.Log("Higher than" + MaxReelArray + " Reel:" + (currentLower + offset - MaxReelArray));
                return currentLower + offset - MaxReelArray;
            }

            // オーバーフローがないならそのまま返す
            //Debug.Log("No Overflow" + (currentLower + offset - MaxReelArray));
            return currentLower + offset;
        }
    }
}
