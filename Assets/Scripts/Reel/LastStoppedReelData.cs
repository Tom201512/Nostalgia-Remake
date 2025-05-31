using System.Collections.Generic;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_Reels
{
	public class LastStoppedReelData
	{
        // 最後に止めたリールのデータ

        // const

        // var
        // 最後に止めた位置
        public List<int> LastPos { get; private set; }
        // 最後に止まった出目
        public List<List<ReelSymbols>> LastSymbols { get; private set; }

        // コンストラクタ
        public LastStoppedReelData()
        {
            LastPos = new List<int>();
            LastSymbols = new List<List<ReelSymbols>>();
        }

        // 最後に止めた結果を作る
        public void GenerateLastStopped(ReelObject[] reelObjects)
        {
            // 初期化
            LastPos.Clear();
            LastSymbols.Clear();

            string posBuffer = "";

            // リール図柄を作成する
            for (int i = 0; i < reelObjects.Length; i++)
            {
                LastPos.Add(reelObjects[i].GetReelPos(ReelPosID.Lower));
                posBuffer += LastPos[i];

                //Debug.Log("Position:" + LastPos[i]);

                // データ作成
                LastSymbols.Add(new List<ReelSymbols>());

                // 各位置の図柄を得る(枠下2段目から枠上2段目まで)
                for (sbyte j = (int)ReelPosID.Lower2nd; j < (int)ReelPosID.Upper2nd; j++)
                {
                    LastSymbols[i].Add(reelObjects[i].GetReelSymbol(j));
                    //Debug.Log("Symbol:" + reelObjects[i].GetReelSymbol(j));
                }
            }
            //Debug.Log("Final ReelPosition" + posBuffer);

            // 各リールごとに表示(デバッグ)
            foreach (List<ReelSymbols> reelResult in LastSymbols)
            {
                //Debug.Log("Reel:");
                for (int i = 0; i < reelResult.Count; i++)
                {
                    //Debug.Log(reelResult[i]);
                }
            }
        }

        // 最後に止めた図柄を得る
        public ReelSymbols GetLastStoppedSymbol(int reelID, int posID) => LastSymbols[reelID][GetReelArrayIndex(posID)];
    }
}