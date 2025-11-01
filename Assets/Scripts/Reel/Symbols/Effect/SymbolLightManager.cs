using System;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels.Symbol
{
    // リールシンボルライトのマネージャー
    public class SymbolLightManager : MonoBehaviour
    {
        // var
        public SymbolLight[] SymbolLightObj { get; private set; }

        private void Awake()
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
