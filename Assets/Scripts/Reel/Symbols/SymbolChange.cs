using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using UnityEngine.UI;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

public class SymbolChange : MonoBehaviour
{
    // 表示部分
    private SpriteRenderer sprite;

    // リール位置識別ID
    [SerializeField] private ReelPosID posID;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // 図柄変更
    public void ChangeSymbol(Sprite symbolSprite) => sprite.sprite = symbolSprite;

    // 位置IDを返す
    public ReelPosID GetPosID() => posID;

    // リール配列の番号を図柄へ変更
    public static ReelSymbols ReturnSymbol(byte symbolID) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), symbolID);
    // リール位置を配列要素に置き換える
    public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
}
