using System.Collections.Generic;
using System.IO;

namespace ReelSpinGame_Flash
{
    // フラッシュのデータ
    public class FlashData
    {
        // プロパティのシリアライズ
        public enum PropertyID { FrameID, Body, SymbolLower, LoopPosition = 13 };

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

        // データを最後まで読んだか確認する
        public bool HasSeekReachedEnd() => CurrentSeekPos == FlashArray.Count;

        // シーク位置変更
        public void MoveNextSeek() => CurrentSeekPos += 1;

        // シーク位置を戻す
        public void SetSeek(int seekPos) => CurrentSeekPos = seekPos;
    }
}