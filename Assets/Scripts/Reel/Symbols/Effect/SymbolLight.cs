using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ReelSpinGame_Reels.Symbol
{
    // シンボルのライト部分
    public class SymbolLight : MonoBehaviour
    {
        // const
        // デフォルトの明るさ(点灯時)
        public const byte TurnOnValue = 255;
        // デフォルトの暗さ(消灯時)
        public const byte TurnOffValue = 120;

        // 表示部分
        private SpriteRenderer sprite;

        // 明るさ
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

        // 色変更
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

