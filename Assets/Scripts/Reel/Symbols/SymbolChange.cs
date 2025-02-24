using UnityEngine;
using ReelSpinGame_Reels;

public class SymbolChange : MonoBehaviour
{
    // const
    
    // var
    // 図柄の表示用
    [SerializeField] private Material[] symbolMaterials;
    [SerializeField] private ReelData.ReelSymbols currentSymbol = ReelData.ReelSymbols.RedSeven;

    // 表示部分
    private MeshRenderer mesh;

    // リール位置識別ID
    [SerializeField] private ReelData.ReelPosID posID;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material = symbolMaterials[(int)currentSymbol];
    }

    // 図柄変更
    public void ChangeSymbol(ReelData.ReelSymbols symbolID)
    {
        mesh.material = symbolMaterials[(int)symbolID];
    }

    // 位置IDを返す
    public ReelData.ReelPosID GetPosID() => posID;
}
