using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayer : MonoBehaviour
    {
        // リール結果を表示する

        // var
        // 図柄部分
        private SymbolDisplayUI[] reelSymbols;

        void Awake()
        {
            reelSymbols = GetComponentsInChildren<SymbolDisplayUI>();
        }

        // 下の位置を基準にリール図柄を表示
        public void DisplayReel(int lowerPos, ReelObject reel)
        {
            foreach(SymbolDisplayUI symbol in reelSymbols)
            {
                Debug.Log("Pos:" + ReelData.OffsetReel(lowerPos, (int)symbol.GetPosID()));
                symbol.ChangeSymbol(reel.GetSymbolImageAtPos(ReelData.OffsetReel(lowerPos, (int)symbol.GetPosID())));
            }
        }
    }
}
