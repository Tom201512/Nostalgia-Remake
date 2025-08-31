using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChange : MonoBehaviour
{
    // const
    // �f�t�H���g�̖��邳(�_����)
    public const byte TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    public const byte TurnOffValue = 120;

    // var
    // �}���̕\���p
    [SerializeField] private Sprite[] symbolImages;

    // ���邳
    private byte lastBrightness;
    // ���邳�ύX�R���|�[�l���g
    private SymbolLight symbolLight;

    // �\������
    private SpriteRenderer sprite;

    // ���[���ʒu����ID
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

    // �}���ύX
    public void ChangeSymbol(ReelData.ReelSymbols symbolID) => sprite.sprite = symbolImages[(int)symbolID];

    // �F�ύX
    public void ChangeBrightness(byte brightness)
    {
        if(lastBrightness != brightness)
        {

        }
    }

    // �ʒuID��Ԃ�
    public ReelData.ReelPosID GetPosID() => posID;

    // ���[���z��̔ԍ���}���֕ύX
    public static ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
    // ���[���ʒu��z��v�f�ɒu��������
    public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
}
