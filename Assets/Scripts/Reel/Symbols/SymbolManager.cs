using ReelSpinGame_Reel.Util;
using System;
using UnityEngine;

namespace ReelSpinGame_Reel.Symbol
{
    // 図柄マネージャー
    public class SymbolManager : MonoBehaviour
    {
        // 図柄の順序
        public static readonly ReelPosID[] ReelPosOrder =
        {
            ReelPosID.Lower2nd,
            ReelPosID.Lower,
            ReelPosID.Center,
            ReelPosID.Upper,
            ReelPosID.Upper2nd,
        };

        [SerializeField] private Sprite[] symbolImages;         // 図柄画像
        [SerializeField] SpriteRenderer underLine;              // 切れ目
        [SerializeField] SpriteRenderer reelMarker;             // マーカー

        public SymbolChange[] SymbolObj { get; private set; }   // リール内の図柄
        public int CurrentMarkerPos { get; set; }       // 現在のマーカー位置

        void Awake()
        {
            SymbolObj = GetComponentsInChildren<SymbolChange>();
            underLine.gameObject.SetActive(false);
            reelMarker.gameObject.SetActive(false);
            CurrentMarkerPos = -1;
        }

        // 図柄の更新
        public void UpdateSymbolsObjects(int currentLower, int[] reelArray)
        {
            // 切れ目の位置にある図柄が止まっているか
            bool hasLastPosSymbol = false;
            // マーカー位置の図柄があるか
            bool hasMarkerPosSymbol = false;

            // 現在のリール下段を基準として位置を更新する。
            foreach (SymbolChange symbol in SymbolObj)
            {
                symbol.ChangeSymbol(symbolImages[reelArray[ReelSymbolPosCalc.OffsetReelPos(currentLower, (int)symbol.GetPosID())]]);

                // もし最後の位置にある図柄の場合は切れ目の位置を動かす
                if (!hasLastPosSymbol && ReelSymbolPosCalc.OffsetReelPos(currentLower, (int)symbol.GetPosID()) == 20)
                {
                    hasLastPosSymbol = true;
                    underLine.transform.SetPositionAndRotation(symbol.transform.position + new Vector3(0, 0, -0.2f), symbol.transform.rotation);
                }

                // もしマーカーで指定した図柄があればマーカー表示する
                if (!hasMarkerPosSymbol && ReelSymbolPosCalc.OffsetReelPos(currentLower, (int)symbol.GetPosID()) == CurrentMarkerPos)
                {
                    hasMarkerPosSymbol = true;
                    reelMarker.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
                }
            }

            underLine.gameObject.SetActive(hasLastPosSymbol);
            reelMarker.gameObject.SetActive(hasMarkerPosSymbol);
        }

        // リール図柄を得る
        public ReelSymbols GetReelSymbol(int currentLower, ReelPosID posID, int[] reelArray) => ReelSymbolPosCalc.ReturnSymbol(reelArray[ReelSymbolPosCalc.OffsetReelPos(currentLower, (int)posID)]);
        // リール配列の番号を図柄へ変更
        public ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
        // 図柄を得る
        public Sprite GetSymbolImage(ReelSymbols symbolID) => symbolImages[(int)symbolID];
    }
}
