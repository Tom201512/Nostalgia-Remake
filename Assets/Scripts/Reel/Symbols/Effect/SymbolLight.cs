using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
{
    // シンボルのライト部分
    public class SymbolLight : MonoBehaviour
    {
        public const byte TurnOnValue = 255;        // デフォルトの明るさ(点灯時)
        public const byte TurnOffValue = 120;       // デフォルトの暗さ(消灯時)

        private SpriteRenderer sprite;      // 表示部分
        private byte lastBrightness;        // 表示部分

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            lastBrightness = 0;
        }

        void Start()
        {
            ChangeBrightness(TurnOffValue);
        }

        // 色変更
        public void ChangeBrightness(byte brightness)
        {
            if (lastBrightness != brightness)
            {
                sprite.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
                lastBrightness = brightness;
            }
        }
    }
}

