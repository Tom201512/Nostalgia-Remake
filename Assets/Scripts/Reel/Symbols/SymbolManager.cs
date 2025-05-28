using ReelSpinGame_Reels;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    // �}�����܂Ƃ߂�}�l�[�W���[

    // const

    // var
    // ���[�����̐}��
    public SymbolChange[] SymbolObj { get; private set; }
    // �؂��
    [SerializeField] GameObject Underline;
    // ���[�����
    private ReelData reelData;

    // ������
    private void Awake()
    {
        SymbolObj = GetComponentsInChildren<SymbolChange>();
        Underline.SetActive(false);
    }

    // func
    // ���[�������Z�b�g����
    public void SetReelData(ReelData reelData)
    {
        this.reelData = reelData;
    }

    // �}���̍X�V
    public void UpdateSymbolsObjects()
    {
        // �؂�ڂ̈ʒu�ɂ���}�����~�܂��Ă��邩
        bool hasLastPosSymbol = false;

        // ���݂̃��[�����i����Ƃ��Ĉʒu���X�V����B
        foreach (SymbolChange symbol in SymbolObj)
        {
            symbol.ChangeSymbol(reelData.GetReelSymbol((sbyte)symbol.GetPosID()));

            // �����Ō�̈ʒu�ɂ���}���̏ꍇ�͐؂�ڂ̈ʒu�𓮂���
            if(!hasLastPosSymbol && reelData.GetReelPos((sbyte)symbol.GetPosID()) == 20)
            {
                hasLastPosSymbol = true;
                Underline.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
            }
        }

        Underline.SetActive(hasLastPosSymbol);
    }
}
