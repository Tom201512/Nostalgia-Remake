using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Reels.Tables
{
    // リール制御テーブル
    public class ReelTableData
    {
        // リール制御テーブル(ディレイを格納する)
        public byte[] TableData { get; private set; } = new byte[ReelData.MaxReelArray];

        public ReelTableData(StringReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // デバッグ用
            string debugBuffer = "";

            // 読み込み開始
            foreach (string value in values)
            {
                // リールデータを読み込む
                if (indexNum < ReelData.MaxReelArray)
                {
                    TableData[indexNum] = Convert.ToByte(value);
                    debugBuffer += TableData[indexNum];
                }

                // 最後の一行は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum++;
            }

            Debug.Log("Array:" + debugBuffer);
        }
    }
}