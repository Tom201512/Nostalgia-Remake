using UnityEngine;
using ReelSpinGame_Reels;

public class SymbolChange : MonoBehaviour
{
    // const
    
    // var
    // �}���̕\���p
    [SerializeField] private Material[] symbolMaterials;
    [SerializeField] private ReelData.ReelSymbols currentSymbol = ReelData.ReelSymbols.RedSeven;

    // �\������
    private MeshRenderer mesh;

    // ���[���ʒu����ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material = symbolMaterials[(int)currentSymbol];
    }

    // �}���ύX
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        mesh.material = symbolMaterials[(int)symbolID];
    }

    // �ʒuID��Ԃ�
    public ReelData.ReelPosID GetPosID() => posID;
}
