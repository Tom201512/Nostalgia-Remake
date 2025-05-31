using ReelSpinGame_Reels;
using UnityEngine;

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
    // �\������
    private SpriteRenderer sprite;

    // ���[���ʒu����ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        lastBrightness = 0;
        ChangeBrightness(TurnOffValue);
    }

    // �}���ύX
    public void ChangeSymbol(ReelData.ReelSymbols symbolID) => sprite.sprite = symbolImages[(int)symbolID];

    // �F�ύX
    public void ChangeBrightness(byte brightness)
    {
        if(lastBrightness != brightness)
        {
            sprite.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
            lastBrightness = brightness;
        }
    }

    // �ʒuID��Ԃ�
    public ReelData.ReelPosID GetPosID() => posID;
}
