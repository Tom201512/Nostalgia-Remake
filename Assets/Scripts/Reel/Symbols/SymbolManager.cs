using ReelSpinGame_Reels;
using System;
using UnityEngine;

using static ReelSpinGame_Reels.Array.ReelArrayModel;

public class SymbolManager : MonoBehaviour
{
    // �}���}�l�[�W���[

    // const

    // var
    // �}���\���p
    [SerializeField] private Sprite[] symbolImages;
    // �؂��
    [SerializeField] GameObject Underline;

    // ���[�����̐}��
    public SymbolChange[] SymbolObj { get; private set; }

    // ������
    private void Awake()
    {
        SymbolObj = GetComponentsInChildren<SymbolChange>();
        Underline.SetActive(false);
    }

    // func

    // �}���̍X�V
    public void UpdateSymbolsObjects(int currentLower, byte[] reelArray)
    {
        // �؂�ڂ̈ʒu�ɂ���}�����~�܂��Ă��邩
        bool hasLastPosSymbol = false;

        // ���݂̃��[�����i����Ƃ��Ĉʒu���X�V����B
        foreach (SymbolChange symbol in SymbolObj)
        {
            symbol.ChangeSymbol(symbolImages[(int)reelArray[ReelObjectPresenter.OffsetReelPos(currentLower, (sbyte)symbol.GetPosID())]]);

            // �����Ō�̈ʒu�ɂ���}���̏ꍇ�͐؂�ڂ̈ʒu�𓮂���
            if(!hasLastPosSymbol && currentLower == 20)
            {
                hasLastPosSymbol = true;
                Underline.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
            }
        }

        Underline.SetActive(hasLastPosSymbol);
    }

    // ���[���}���𓾂�
    public ReelSymbols GetReelSymbol(int currentLower, int posID, byte[] reelArray) => SymbolChange.ReturnSymbol(reelArray[ReelObjectPresenter.OffsetReelPos(currentLower, posID)]);

    // ���[���z��̔ԍ���}���֕ύX
    public ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);

    // �}���𓾂�
    public Sprite GetSymbolImage(ReelSymbols symbolID) => symbolImages[(int)symbolID];
}
