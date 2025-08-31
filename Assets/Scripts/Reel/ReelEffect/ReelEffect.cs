using ReelSpinGame_Reels.Symbol;
using UnityEngine;

namespace ReelSpinGame_Reels
{
    // リールエフェクト(バックライトの点滅、シンボル点灯など)
    public class ReelEffect : MonoBehaviour
    {
        // リール本体
        [SerializeField] ReelBase reelBase;
        [SerializeField] SymbolLightManager symbolLight;

        // func
        // リール本体の明るさ変更
        public void ChangeReelBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);

        // 指定した位置の図柄の明るさ変更
        public void ChangeSymbolBrightness(int posID, byte brightness) => symbolLight.ChangeSymbolBrightness(posID, brightness);
    }
}

