using ReelSpinGame_Reels.Util;
using System;
using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
{
    // 図柄マネージャー
    public class SymbolManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] symbolImages;         // スプライト
        [SerializeField] GameObject Underline;                  // 切れ目

        public SymbolChange[] SymbolObj { get; private set; }   // リール内の図柄

        void Awake()
        {
            SymbolObj = GetComponentsInChildren<SymbolChange>();
            Underline.SetActive(false);
        }

        // 図柄の更新
        public void UpdateSymbolsObjects(int currentLower, byte[] reelArray)
        {
            // 切れ目の位置にある図柄が止まっているか
            bool hasLastPosSymbol = false;

            // 現在のリール下段を基準として位置を更新する。
            foreach (SymbolChange symbol in SymbolObj)
            {
                symbol.ChangeSymbol(symbolImages[reelArray[ReelSymbolPosCalc.OffsetReelPos(currentLower, (sbyte)symbol.GetPosID())]]);

                // もし最後の位置にある図柄の場合は切れ目の位置を動かす
                if (!hasLastPosSymbol && currentLower == 20)
                {
                    hasLastPosSymbol = true;
                    Underline.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
                }
            }

            Underline.SetActive(hasLastPosSymbol);
        }

        // リール図柄を得る
        public ReelSymbols GetReelSymbol(int currentLower, int posID, byte[] reelArray) => ReelSymbolPosCalc.ReturnSymbol(reelArray[ReelSymbolPosCalc.OffsetReelPos(currentLower, posID)]);
        // リール配列の番号を図柄へ変更
        public ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
        // 図柄を得る
        public Sprite GetSymbolImage(ReelSymbols symbolID) => symbolImages[(int)symbolID];
    }
}
