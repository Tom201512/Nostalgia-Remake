using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

        // �\������
        private SpriteRenderer sprite;

        // ���邳
        private byte lastBrightness;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            lastBrightness = 0;
        }

        private void Start()
        {
            ChangeBrightness(TurnOffValue);
        }

        // �F�ύX
        public void ChangeBrightness(byte brightness)
        {
            if(lastBrightness != brightness)
            {
                sprite.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
                lastBrightness = brightness;
            }
        }
    }
}

