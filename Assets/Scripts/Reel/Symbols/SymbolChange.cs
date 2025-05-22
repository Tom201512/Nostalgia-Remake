using ReelSpinGame_Reels;
using UnityEngine;

public class SymbolChange : MonoBehaviour
{
    // const
    
    // var
    // �}���̕\���p
    [SerializeField] private Texture[] symbolImages;
    [SerializeField] private ReelData.ReelSymbols currentSymbol = ReelData.ReelSymbols.RedSeven;

    // �\������
    private Renderer render;

    // ���[���ʒu����ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        render = GetComponent<Renderer>();
        render.material.mainTexture = symbolImages[(int)currentSymbol];
        render.material.EnableKeyword("_EMISSION");
    }

    // �}���ύX
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        render.material.mainTexture = symbolImages[(int)symbolID];
    }

    // �ʒuID��Ԃ�
    public ReelData.ReelPosID GetPosID() => posID;

    // �}���̖��邳��ύX����(0~255)
    public void ChangeBrightness(byte r, byte g, byte b)
    {
        render.material.SetColor("_Color", new Color32(r, g, b, 255));
        Debug.Log("SetColor" + r + "," + g + "," + b);
    }

    // �}���̌��x��ύX����
    public void ChangeEmmision(byte r, byte g, byte b)
    {
        render.material.SetColor("_EmissionColor", new Color32(r, g, b, 255));
        Debug.Log("_Emission" + r + "," + g + "," + b);
    }
}
