using ReelSpinGame_Flash;
using UnityEngine;

namespace ReelSpinGame_Reel
{
    // リール本体
    public class ReelBase : MonoBehaviour
    {
        public const byte TurnOnValue = 255;        // デフォルトの明るさ(点灯時)
        public const byte TurnOffValue = 80;        // デフォルトの暗さ(消灯時)

        private Renderer render;                    // 表示部分
        private byte[] previousBrightness;           // 前フレームの明るさ
        
        void Awake()
        {
            render = GetComponent<Renderer>();
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
            byte r = previousBrightness[(int)FlashData.ColorID.R];
            byte g = previousBrightness[(int)FlashData.ColorID.G];
            byte b = previousBrightness[(int)FlashData.ColorID.B];

            if (red != -1 && red != r)
            {
                r = (byte)red;
                previousBrightness[(int)FlashData.ColorID.R] = (byte)red;
            }

            if (green != -1 && green != g)
            {
                g = (byte)green;
                previousBrightness[(int)FlashData.ColorID.G] = (byte)green;
            }

            if (blue != -1 && blue != b)
            {
                b = (byte)blue;
                previousBrightness[(int)FlashData.ColorID.B] = (byte)blue;
            }

            render.material.SetColor("_Color", new Color32(r, g, b, 255));
        }
    }
}