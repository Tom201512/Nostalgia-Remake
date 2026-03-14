using UnityEngine;
using static ReelSpinGame_Flash.FlashData;

namespace ReelSpinGame_Reel.Symbol
{
    // シンボルのライト部分
    public class SymbolLight : MonoBehaviour
    {
        public const byte TurnOnValue = 255;        // デフォルトの明るさ(点灯時)
        public const byte TurnOffValue = 120;       // デフォルトの暗さ(消灯時)

        private SpriteRenderer sprite;              // 表示部分
        private byte[] previousBrightness;          // 前フレームの明るさ

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            previousBrightness = new byte[]
            {
                TurnOffValue,
                TurnOffValue,
                TurnOffValue,
            };
        }

        void Start()
        {
            ChangeBrightness(TurnOffValue, TurnOffValue, TurnOffValue);
        }

        // 色変更
        public void ChangeBrightness(int red, int green, int blue)
        {
            byte r = previousBrightness[(int)ColorID.R];
            byte g = previousBrightness[(int)ColorID.G];
            byte b = previousBrightness[(int)ColorID.B];

            if (red != -1 && red != r)
            {
                r = (byte)red;
                previousBrightness[(int)ColorID.R] = (byte)red;
            }

            if (green != -1 && green != g)
            {
                g = (byte)green;
                previousBrightness[(int)ColorID.G] = (byte)green;
            }

            if (blue != -1 && blue != b)
            {
                b = (byte)blue;
                previousBrightness[(int)ColorID.B] = (byte)blue;
            }

            sprite.material.SetColor("_Color", new Color32(r, g, b, 255));
        }
    }
}

