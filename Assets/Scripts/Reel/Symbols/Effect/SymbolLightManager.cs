using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
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
        public void ChangeSymbolBrightness(int posID, byte brightness)
        {
            SymbolLightObj[SymbolChange.GetReelArrayIndex(posID)].ChangeBrightness(brightness);
        }
    }
}
