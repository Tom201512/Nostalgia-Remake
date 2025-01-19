using UnityEngine;
using ReelSpinGame_Reels.Array;

public class SymbolChange : MonoBehaviour
{
    // �}���̕\���p
    [SerializeField] Material[] symbols;
    [SerializeField] private ReelArray.ReelSymbols currentSymbol = ReelArray.ReelSymbols.RedSeven;

    // �\������
    private MeshRenderer mesh;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        Debug.Log(mesh);
        mesh.material = symbols[(int)currentSymbol];
    }
}
