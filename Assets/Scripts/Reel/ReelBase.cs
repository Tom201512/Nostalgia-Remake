using UnityEngine;

namespace ReelSpinGame_Reels
{
    // リール本体
    public class ReelBase : MonoBehaviour
    {
        public const byte TurnOnValue = 255;        // デフォルトの明るさ(点灯時)
        public const byte TurnOffValue = 180;       // デフォルトの暗さ(消灯時)

        private Renderer render;        // 表示部分
        private byte lastBrightness;    // 明るさ

        void Awake()
        {
            render = GetComponent<Renderer>();
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
                render.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
                lastBrightness = brightness;
            }
        }
    }
}
