using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using UnityEngine.UI;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

public class SymbolChange : MonoBehaviour
{
    // �\������
    private SpriteRenderer sprite;

    // ���[���ʒu����ID
    [SerializeField] private ReelPosID posID;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // �}���ύX
    public void ChangeSymbol(Sprite symbolSprite) => sprite.sprite = symbolSprite;

    // �ʒuID��Ԃ�
    public ReelPosID GetPosID() => posID;

    // ���[���z��̔ԍ���}���֕ύX
    public static ReelSymbols ReturnSymbol(byte symbolID) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), symbolID);
    // ���[���ʒu��z��v�f�ɒu��������
    public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
}
