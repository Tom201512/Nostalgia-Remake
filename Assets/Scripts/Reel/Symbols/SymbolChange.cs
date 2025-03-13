using UnityEngine;
using ReelSpinGame_Reels;
using UnityEngine.UIElements;

public class SymbolChange : MonoBehaviour
{
    // const
    
    // var
    // 図柄の表示用
    [SerializeField] private Texture[] symbolImages;
    [SerializeField] private ReelData.ReelSymbols currentSymbol = ReelData.ReelSymbols.RedSeven;

    // 表示部分
    private Renderer render;

    // リール位置識別ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        render = GetComponent<Renderer>();
        render.material.mainTexture = symbolImages[(int)currentSymbol];
    }

    // 図柄変更
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        render.material.mainTexture = symbolImages[(int)symbolID];
    }

    // 位置IDを返す
    public ReelData.ReelPosID GetPosID() => posID;

    // 図柄の明るさを変更する(0~255)
    public void ChangeBrightness(byte brightness) => ChangeMaterialColor(brightness, brightness, brightness);

    // 図柄の色変更
    public void ChangeMaterialColor(byte r, byte g, byte b)
    {
        render.material.SetColor("_Color", new Color32(r, g, b, 255));
        Debug.Log("SetColor" + r + "," + g + "," + b);
    }
}
