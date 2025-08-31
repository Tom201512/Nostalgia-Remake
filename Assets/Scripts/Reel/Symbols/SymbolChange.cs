using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChange : MonoBehaviour
{
    // var
    // 図柄の表示用
    [SerializeField] private Sprite[] symbolImages;

    // 表示部分
    private SpriteRenderer sprite;

    // リール位置識別ID
    [SerializeField] private ReelPosID posID;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // 図柄変更
    public void ChangeSymbol(ReelSymbols symbolID) => sprite.sprite = symbolImages[(int)symbolID];

    // 位置IDを返す
    public ReelPosID GetPosID() => posID;

    // リール配列の番号を図柄へ変更
    public static ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
    // リール位置を配列要素に置き換える
    public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
}
