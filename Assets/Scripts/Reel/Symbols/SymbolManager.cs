using ReelSpinGame_Reels;
using UnityEngine;
using UnityEngine.UI;

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
    public void UpdateSymbolsObjects(ReelData data)
    {
        // �؂�ڂ̈ʒu�ɂ���}�����~�܂��Ă��邩
        bool hasLastPosSymbol = false;

        // ���݂̃��[�����i����Ƃ��Ĉʒu���X�V����B
        foreach (SymbolChange symbol in SymbolObj)
        {
            symbol.ChangeSymbol(symbolImages[(int)data.GetReelSymbol((sbyte)symbol.GetPosID())]);

            // �����Ō�̈ʒu�ɂ���}���̏ꍇ�͐؂�ڂ̈ʒu�𓮂���
            if(!hasLastPosSymbol && data.GetReelPos((sbyte)symbol.GetPosID()) == 20)
            {
                hasLastPosSymbol = true;
                Underline.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
            }
        }

        Underline.SetActive(hasLastPosSymbol);
    }

    // �}���𓾂�
    public Sprite GetSymbolImage(byte symbolID) => symbolImages[symbolID];
}
