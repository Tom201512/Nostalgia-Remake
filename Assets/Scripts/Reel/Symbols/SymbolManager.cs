using ReelSpinGame_Reels;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    // 図柄をまとめるマネージャー

    // const

    // var
    // リール内の図柄
    public SymbolChange[] SymbolObj { get; private set; }
    // 切れ目
    [SerializeField] GameObject Underline;

    // 初期化
    private void Awake()
    {
        SymbolObj = GetComponentsInChildren<SymbolChange>();
        Underline.SetActive(false);
    }

    // func

    // 図柄の更新
    public void UpdateSymbolsObjects(ReelData data)
    {
        // 切れ目の位置にある図柄が止まっているか
        bool hasLastPosSymbol = false;

        // 現在のリール下段を基準として位置を更新する。
        foreach (SymbolChange symbol in SymbolObj)
        {
            symbol.ChangeSymbol(data.GetReelSymbol((sbyte)symbol.GetPosID()));

            // もし最後の位置にある図柄の場合は切れ目の位置を動かす
            if(!hasLastPosSymbol && data.GetReelPos((sbyte)symbol.GetPosID()) == 20)
            {
                hasLastPosSymbol = true;
                Underline.transform.SetPositionAndRotation(symbol.transform.position, symbol.transform.rotation);
            }
        }

        Underline.SetActive(hasLastPosSymbol);
    }
}
