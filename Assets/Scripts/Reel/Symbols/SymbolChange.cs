using ReelSpinGame_Reels;
using UnityEngine;

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
    // 表示部分
    private SpriteRenderer sprite;

    // リール位置識別ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        lastBrightness = 0;
        ChangeBrightness(TurnOffValue);
    }

    // 図柄変更
    public void ChangeSymbol(ReelData.ReelSymbols symbolID) => sprite.sprite = symbolImages[(int)symbolID];

    // 色変更
    public void ChangeBrightness(byte brightness)
    {
        if(lastBrightness != brightness)
        {
            sprite.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
            lastBrightness = brightness;
        }
    }

    // 位置IDを返す
    public ReelData.ReelPosID GetPosID() => posID;
}
