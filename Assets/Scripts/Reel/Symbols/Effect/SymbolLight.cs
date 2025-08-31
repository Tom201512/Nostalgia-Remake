using UnityEngine;

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

        // 明るさ
        // 表示部分
        public byte Brightness {  get; private set; }

        private void Awake()
        {
            Brightness = 120;
        }

        // 色変更
        public void ChangeBrightness(byte brightness) => Brightness = brightness;
    }
}

