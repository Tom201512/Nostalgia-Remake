using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
{
    // ���[���V���{�����C�g�̃}�l�[�W���[
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

        // �w�肵���ʒu�̖��邳�ύX
        public void ChangeSymbolBrightness(int posID, byte brightness) => SymbolLightObj[SymbolChange.GetReelArrayIndex(posID)].ChangeBrightness(brightness);
    }
}
