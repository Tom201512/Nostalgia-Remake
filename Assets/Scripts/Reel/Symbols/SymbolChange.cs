using UnityEngine;
using ReelSpinGame_Reels;
using ReelSpinGame_Reels.ReelArray;

public class SymbolChange : MonoBehaviour
{
    // const
    
    // var

    // 図柄の表示用
    [SerializeField] private Material[] symbolMaterials;
    [SerializeField] private ReelArray.ReelSymbols currentSymbol = ReelArray.ReelSymbols.RedSeven;

    // 表示部分
    private MeshRenderer mesh;

    // リール位置識別ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material = symbolMaterials[(int)currentSymbol];
    }

    public void ChangeSymbol(ReelArray.ReelSymbols symbolID)
    {
        mesh.material = symbolMaterials[(int)symbolID];
    }

    public ReelData.ReelPosID GetPosID() => posID;
}
