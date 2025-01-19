using UnityEngine;
using ReelSpinGame_Reels.Array;

public class SymbolChange : MonoBehaviour
{
    // }•¿‚Ì•\¦—p
    [SerializeField] Material[] symbols;
    [SerializeField] private ReelArray.ReelSymbols currentSymbol = ReelArray.ReelSymbols.RedSeven;

    // •\¦•”•ª
    private MeshRenderer mesh;

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        Debug.Log(mesh);
        mesh.material = symbols[(int)currentSymbol];
    }
}
