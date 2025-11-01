using ReelSpinGame_Reels;
using System;
using UnityEngine;

using static ReelSpinGame_Reels.Array.ReelArrayModel;

public class SymbolManager : MonoBehaviour
{
    // 図柄マネージャー

    // const

    // var
    // 図柄表示用
    [SerializeField] private Sprite[] symbolImages;
    // 切れ目
    [SerializeField] GameObject Underline;

    // リール内の図柄
    public SymbolChange[] SymbolObj { get; private set; }

    // 初期化
    private void Awake()
    {
        SymbolObj = GetComponentsInChildren<SymbolChange>();
        Underline.SetActive(false);
    }

    // func

    // 図柄の更新
    public void UpdateSymbolsObjects(int currentLower, byte[] reelArray)
    {
        // 切れ目の位置にある図柄が止まっているか
        bool hasLastPosSymbol = false;

        // 現在のリール下段を基準として位置を更新する。
        foreach (SymbolChange symbol in SymbolObj)
        {
            symbol.ChangeSymbol(symbolImages[(int)reelArray[ReelObjectPresenter.OffsetReelPos(currentLower, (sbyte)symbol.GetPosID())]]);

            // もし最後の位置にある図柄の場合は切れ目の位置を動かす
            if(!hasLastPosSymbol && currentLower == 20)
            {
                hasLastPosSymbol = true;
                Underline.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
            }
        }

        Underline.SetActive(hasLastPosSymbol);
    }

    // リール図柄を得る
    public ReelSymbols GetReelSymbol(int currentLower, int posID, byte[] reelArray) => SymbolChange.ReturnSymbol(reelArray[ReelObjectPresenter.OffsetReelPos(currentLower, posID)]);

    // リール配列の番号を図柄へ変更
    public ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);

    // 図柄を得る
    public Sprite GetSymbolImage(ReelSymbols symbolID) => symbolImages[(int)symbolID];
}
