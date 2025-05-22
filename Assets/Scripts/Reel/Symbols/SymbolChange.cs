using ReelSpinGame_Reels;
using UnityEngine;

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
        render.material.EnableKeyword("_EMISSION");
    }

    // 図柄変更
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        render.material.mainTexture = symbolImages[(int)symbolID];
    }

    // 位置IDを返す
    public ReelData.ReelPosID GetPosID() => posID;

    // 図柄の明るさを変更する(0~255)
    public void ChangeBrightness(byte r, byte g, byte b)
    {
        render.material.SetColor("_Color", new Color32(r, g, b, 255));
        Debug.Log("SetColor" + r + "," + g + "," + b);
    }

    // 図柄の光度を変更する
    public void ChangeEmmision(byte r, byte g, byte b)
    {
        render.material.SetColor("_EmissionColor", new Color32(r, g, b, 255));
        Debug.Log("_Emission" + r + "," + g + "," + b);
    }
}
