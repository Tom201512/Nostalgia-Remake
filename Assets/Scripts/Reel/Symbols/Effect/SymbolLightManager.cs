using System;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels.Symbol
{
    // ���[���V���{�����C�g�̃}�l�[�W���[
    public class SymbolLightManager : MonoBehaviour
    {
        // var
        public SymbolLight[] SymbolLightObj { get; private set; }

        private void Awake()
        {
            SymbolLightObj = GetComponentsInChildren<SymbolLight>();
        }

        // �w�肵���ʒu�̖��邳�ύX
        public void ChangeSymbolBrightness(int posID, byte brightness)
        {
            SymbolLightObj[SymbolChange.GetReelArrayIndex(posID)].ChangeBrightness(brightness);
        }
    }
}
