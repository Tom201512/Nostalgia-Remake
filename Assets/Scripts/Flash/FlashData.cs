using ReelSpinGame_Reels.Util;
using System.Collections.Generic;
using System.IO;

namespace ReelSpinGame_Flash
{
    // フラッシュのデータ
    public class FlashData
    {
        // プロパティのシリアライズ
        public enum PropertyID 
        { 
            FrameID = 0, 
            LoopPosition = 1,
            Body = 2,
            Lower2ndPos = 5,
        };

        // 色のシリアライズ
        public enum ColorID
        {
            R = 0,
            G = 1,
            B = 2,
        }

        const int EachReelDataLength = 18;            // 各リール明るさデータの長さ
        const int ColorLength = 3;                          // 各色明るさの長さ

        public List<int[]> FlashArray { get; private set; }     // フラッシュのデータ
        public int CurrentSeekPos { get; private set; }			// 現在の読み込み位置

        public FlashData(StringReader buffer)
        {
            CurrentSeekPos = 0;
            FlashArray = new List<int[]>();

            while (buffer.Peek() != -1)
            {
                string[] stringArray = buffer.ReadLine().Split(',');
                int[] intArray = new int[stringArray.Length];

                for (int i = 0; i < stringArray.Length; i++)
                {
                    intArray[i] = int.Parse(stringArray[i]);
                }
                FlashArray.Add(intArray);
            }
        }

        // 現在参照しているフラッシュデータを渡す
        public int[] GetCurrentFlashData() => FlashArray[CurrentSeekPos];

        // フレーム数を返す
        public int GetFrameCount() => FlashArray[CurrentSeekPos][(int)PropertyID.FrameID];

        // ループ位置を返す
        public int GetLoopValue() => FlashArray[CurrentSeekPos][(int)PropertyID.LoopPosition];

        // 指定リールIDの本体明るさを得る
        public int[] GetReelBodyBrightness(int reelID)
        {
            int indexOffset = (int)PropertyID.Body + reelID * EachReelDataLength;
            int[] colorBrightness = new int[]
            {
                 FlashArray[CurrentSeekPos][indexOffset + (int)ColorID.R],
                 FlashArray[CurrentSeekPos][indexOffset + (int)ColorID.G],
                 FlashArray[CurrentSeekPos][indexOffset + (int)ColorID.B],
            };

            return colorBrightness;
        }

        // 指定リールID図柄位置の明るさを得る
        public int[] GetReelSymbolBrightness(int reelID, int reelPosID)
        {
            int reelPosOffset = ReelSymbolPosCalc.GetReelArrayIndex(reelPosID);
            int indexOffset = (int)PropertyID.Lower2ndPos + reelID * EachReelDataLength + reelPosOffset * ColorLength;

            int[] colorBrightness = new int[]
            {
                 FlashArray[CurrentSeekPos][indexOffset + (int)ColorID.R],
                 FlashArray[CurrentSeekPos][indexOffset + (int)ColorID.G],
                 FlashArray[CurrentSeekPos][indexOffset + (int)ColorID.B],
            };

            return colorBrightness;
        }

        // データを最後まで読んだか確認する
        public bool HasSeekReachedEnd() => CurrentSeekPos == FlashArray.Count;

        // シーク位置変更
        public void MoveNextSeek() => CurrentSeekPos += 1;

        // シーク位置を戻す
        public void SetSeek(int seekPos) => CurrentSeekPos = seekPos;
    }
}