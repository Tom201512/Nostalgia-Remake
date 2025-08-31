using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChange : MonoBehaviour
{
    // const
    // デフォルトの明るさ(点灯時)
    public const byte TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    public const byte TurnOffValue = 120;

    // var
    // 図柄の表示用
    [SerializeField] private Sprite[] symbolImages;

    // 明るさ
    private byte lastBrightness;
    // 明るさ変更コンポーネント
    private SymbolLight symbolLight;

    // 表示部分
    private SpriteRenderer sprite;

    // リール位置識別ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        symbolLight = GetComponent<SymbolLight>();
        lastBrightness = 0;
        ChangeBrightness(TurnOffValue);
    }

    private void Start()
    {
        sprite.material.SetColor("_Color", new Color32(symbolLight.Brightness, symbolLight.Brightness, symbolLight.Brightness, 255));
        lastBrightness = symbolLight.Brightness;
    }

    void Update()
    {
        if(symbolLight.Brightness != lastBrightness)
        {
            sprite.material.SetColor("_Color", new Color32(symbolLight.Brightness, symbolLight.Brightness, symbolLight.Brightness, 255));
            lastBrightness = symbolLight.Brightness;
        }
    }

    // 図柄変更
    public void ChangeSymbol(ReelData.ReelSymbols symbolID) => sprite.sprite = symbolImages[(int)symbolID];

    // 色変更
    public void ChangeBrightness(byte brightness)
    {
        if(lastBrightness != brightness)
        {

        }
    }

    // 位置IDを返す
    public ReelData.ReelPosID GetPosID() => posID;

    // リール配列の番号を図柄へ変更
    public static ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
    // リール位置を配列要素に置き換える
    public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
}
