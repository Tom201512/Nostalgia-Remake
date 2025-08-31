using ReelSpinGame_Reels.Symbol;
using UnityEngine;

namespace ReelSpinGame_Reels
{
    // ���[���G�t�F�N�g(�o�b�N���C�g�̓_�ŁA�V���{���_���Ȃ�)
    public class ReelEffect : MonoBehaviour
    {
        // ���[���{��
        [SerializeField] ReelBase reelBase;
        [SerializeField] SymbolLightManager symbols;

        // func
        // ���[���{�̖̂��邳�ύX
        public void ChangeReelBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);

        // �w�肵���ʒu�̐}���̖��邳�ύX
        public void ChangeSymbolBrightness(int posID, byte brightness) => symbols.ChangeSymbolBrightness(posID, brightness);
    }
}

