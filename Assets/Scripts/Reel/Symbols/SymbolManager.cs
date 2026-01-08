using ReelSpinGame_Reels.Util;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Reels.Symbol
{
    // 図柄マネージャー
    public class SymbolManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] symbolImages;         // 図柄画像
        [SerializeField] SpriteRenderer underLine;              // 切れ目
        [SerializeField] SpriteRenderer reelMarker;             // マーカー

        public SymbolChange[] SymbolObj { get; private set; }   // リール内の図柄
        public int CurrentMarkerPos { get;  set; }       // 現在のマーカー位置

        void Awake()
        {
            SymbolObj = GetComponentsInChildren<SymbolChange>();
            underLine.gameObject.SetActive(false);
            reelMarker.gameObject.SetActive(false);
            CurrentMarkerPos = -1;
        }

        // 図柄の更新
        public void UpdateSymbolsObjects(int currentLower, byte[] reelArray)
        {
            // 切れ目の位置にある図柄が止まっているか
            bool hasLastPosSymbol = false;
            // マーカー位置の図柄があるか
            bool hasMarkerPosSymbol = false;

            // 現在のリール下段を基準として位置を更新する。
            foreach (SymbolChange symbol in SymbolObj)
            {
                symbol.ChangeSymbol(symbolImages[reelArray[ReelSymbolPosCalc.OffsetReelPos(currentLower, (sbyte)symbol.GetPosID())]]);

                // もし最後の位置にある図柄の場合は切れ目の位置を動かす
                if (!hasLastPosSymbol && ReelSymbolPosCalc.OffsetReelPos(currentLower, (sbyte)symbol.GetPosID()) == 20)
                {
                    hasLastPosSymbol = true;
                    underLine.transform.SetPositionAndRotation(symbol.transform.position + new Vector3(0,0,-0.2f), symbol.transform.rotation);
                }

                // もしマーカーで指定した図柄があればマーカー表示する
                if (!hasMarkerPosSymbol && ReelSymbolPosCalc.OffsetReelPos(currentLower, (sbyte)symbol.GetPosID()) == CurrentMarkerPos)
                {
                    hasMarkerPosSymbol = true;
                    reelMarker.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
                }
            }

            underLine.gameObject.SetActive(hasLastPosSymbol);
            reelMarker.gameObject.SetActive(hasMarkerPosSymbol);
        }

        // リール図柄を得る
        public ReelSymbols GetReelSymbol(int currentLower, int posID, byte[] reelArray) => ReelSymbolPosCalc.ReturnSymbol(reelArray[ReelSymbolPosCalc.OffsetReelPos(currentLower, posID)]);
        // リール配列の番号を図柄へ変更
        public ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
        // 図柄を得る
        public Sprite GetSymbolImage(ReelSymbols symbolID) => symbolImages[(int)symbolID];
    }
}
