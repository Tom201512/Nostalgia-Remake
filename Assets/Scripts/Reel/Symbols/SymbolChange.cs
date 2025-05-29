using ReelSpinGame_Reels;
using UnityEngine;

public class SymbolChange : MonoBehaviour
{
    // const
    // デフォルトの明るさ(点灯時)
    public const int TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    public const int TurnOffValue = 120;

    // var
    // 図柄の表示用
    [SerializeField] private Texture[] symbolImages;
    [SerializeField] private ReelData.ReelSymbols currentSymbol = ReelData.ReelSymbols.RedSeven;

    // 明るさ
    public byte Brightness { get; set; }

    // 表示部分
    private Renderer render;

    // リール位置識別ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        Brightness = TurnOffValue;
        render = GetComponent<Renderer>();
        render.material.mainTexture = symbolImages[(int)currentSymbol];
        render.material.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        render.material.SetColor("_Color", new Color32(Brightness, Brightness, Brightness, 255));
    }

    // 図柄変更
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        render.material.mainTexture = symbolImages[(int)symbolID];
    }

    // 位置IDを返す
    public ReelData.ReelPosID GetPosID() => posID;
}
