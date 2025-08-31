using ReelSpinGame_Reels.Symbol;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_Reels.Effect
{
    // リールエフェクトマネージャー
    public class ReelEffectManager : MonoBehaviour
    {
        // var
        // リールエフェクトの配列
        [SerializeField] private List<ReelEffect> reelEffectList;

        private void Awake()
        {
            
        }

        // 指定したリールのバックライト変更
        public void ChangeReelBackLight(int reelID, byte brightness) => reelEffectList[reelID].ChangeReelBrightness(brightness);

        // 指定したリールと図柄位置のライト変更
        public void ChangeReelSymbolLight(int reelID, int posID, byte brightness) => reelEffectList[reelID].ChangeSymbolBrightness(posID, brightness);

        // 全リールの明るさ一括変更
        public void ChangeAllReelBrightness(byte brightness)
        {
            foreach (ReelEffect reel in reelEffectList)
            {
                reel.ChangeReelBrightness(brightness);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.ChangeSymbolBrightness(i, brightness);
                }
            }
        }

        // JAC GAME時のライト点灯
        public void EnableJacGameLight()
        {
            foreach (ReelEffect reel in reelEffectList)
            {
                reel.ChangeReelBrightness(ReelBase.TurnOffValue);

                // 真ん中以外点灯
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    if (i == (int)ReelPosID.Center)
                    {
                        reel.ChangeSymbolBrightness(i, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reel.ChangeSymbolBrightness(i, SymbolLight.TurnOffValue);
                    }
                }
            }
        }
    }
}

