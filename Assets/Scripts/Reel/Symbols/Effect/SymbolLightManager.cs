using ReelSpinGame_Reel.Util;
using UnityEngine;

namespace ReelSpinGame_Reel.Symbol
{
    // リールシンボルライトのマネージャー
    public class SymbolLightManager : MonoBehaviour
    {
        public SymbolLight[] SymbolLightObj { get; private set; }       // 図柄のライト部分

        void Awake()
        {
            SymbolLightObj = GetComponentsInChildren<SymbolLight>();
        }

        // 指定した位置の明るさ変更
        public void ChangeSymbolBrightness(ReelPosID posID, int r, int g, int b)
        {
            SymbolLightObj[ReelSymbolPosCalc.GetReelArrayIndex(posID)].ChangeBrightness(r,g,b);
        }
    }
}
