using ReelSpinGame_Reels;
using UnityEngine;

public class SymbolChange : MonoBehaviour
{
    // const
    // �f�t�H���g�̖��邳(�_����)
    public const int TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    public const int TurnOffValue = 120;

    // var
    // �}���̕\���p
    [SerializeField] private Texture[] symbolImages;
    [SerializeField] private ReelData.ReelSymbols currentSymbol = ReelData.ReelSymbols.RedSeven;

    // ���邳
    public byte Brightness { get; set; }

    // �\������
    private Renderer render;

    // ���[���ʒu����ID
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

    // �}���ύX
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        render.material.mainTexture = symbolImages[(int)symbolID];
    }

    // �ʒuID��Ԃ�
    public ReelData.ReelPosID GetPosID() => posID;
}
