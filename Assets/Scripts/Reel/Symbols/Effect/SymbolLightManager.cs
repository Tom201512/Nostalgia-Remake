using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
{
    // リールシンボルライトのマネージャー
    public class SymbolLightManager : MonoBehaviour
    {
        // const

        // var
        public SymbolLight[] SymbolLightObj { get; private set; }

        private void Awake()
        {
            SymbolLightObj = GetComponentsInChildren<SymbolLight>();
            Debug.Log("Count:" + SymbolLightObj.Length);
        }

        // 指定した位置の明るさ変更
        public void ChangeSymbolBrightness(int posID, byte brightness) => SymbolLightObj[SymbolChange.GetReelArrayIndex(posID)].ChangeBrightness(brightness);
    }
}
