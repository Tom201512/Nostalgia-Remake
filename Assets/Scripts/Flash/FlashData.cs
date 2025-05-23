using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Flash
{
	public class FlashData
	{
		// フラッシュのデータ

		// const
		// プロパティのシリアライズ
		public enum PropertyID { FrameID, Body, SymbolLower, LoopPosition = 13};

		// フラッシュのID
		public enum FlashID { PayoutMiddle, PayoutLower, PayoutUpper, PayoutDiagonalA, PayoutDiagonalB, Flash_V}

		// var 
		// フラッシュのデータ
		public List<int[]> FlashArray { get; private set; }
		// 現在の読み込み位置
		public int CurrentSeekPos { get; private set; }

		// コンストラクタ
		public FlashData(StringReader buffer)
		{
			CurrentSeekPos = 0;
            FlashArray = new List<int[]>();

			while(buffer.Peek() != -1)
            {
                string[] stringArray = buffer.ReadLine().Split(',');
				int[] intArray = new int[stringArray.Length];

				for(int i = 0; i < stringArray.Length; i++)
				{
					intArray[i] = int.Parse(stringArray[i]);
				}
				FlashArray.Add(intArray);
			}
		}

		// func
		// 現在参照しているフラッシュデータを渡す
		public int[] GetCurrentFlashData() => FlashArray[CurrentSeekPos];

		// データを最後まで読んだか確認する
		public bool HasSeekReachedEnd() => CurrentSeekPos == FlashArray.Count - 1;

		// シーク位置変更
		public void MoveNextSeek() => CurrentSeekPos += 1;

		// シーク位置をリセットする
		public void ResetSeek() => CurrentSeekPos = 0;
	}
}