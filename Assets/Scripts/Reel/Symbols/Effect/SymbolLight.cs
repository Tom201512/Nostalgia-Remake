using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
{
    // �V���{���̃��C�g����
    public class SymbolLight : MonoBehaviour
    {
        // const
        // �f�t�H���g�̖��邳(�_����)
        public const byte TurnOnValue = 255;
        // �f�t�H���g�̈Â�(������)
        public const byte TurnOffValue = 120;

        // ���邳
        // �\������
        public byte Brightness {  get; private set; }

        private void Awake()
        {
            Brightness = 120;
        }

        // �F�ύX
        public void ChangeBrightness(byte brightness) => Brightness = brightness;
    }
}

