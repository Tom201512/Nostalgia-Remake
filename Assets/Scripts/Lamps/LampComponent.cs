using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Lamps
{
    // ランプ部分のコンポーネント
    public class LampComponent : MonoBehaviour
    {
        const byte DefaultTurnOn = 255;         // デフォルトの明るさ(点灯時)
        const byte DefaultTurnOff = 60;         // デフォルトの暗さ(消灯時)
        const byte DefaultFlashFrames = 3;      // デフォルトの点滅時フレーム数
        const float LampFlashTime = 0.01f;      // ランプ点灯の間隔(秒間隔)

        [SerializeField] private byte turnOnValue = DefaultTurnOn;        // 点灯時の明るさ
        [SerializeField] private byte turnOffValue = DefaultTurnOff;      // 消灯時の明るさ
        [SerializeField] private byte flashFrames = DefaultFlashFrames;   // 点滅時のフレーム数(0.01秒)

        public bool IsTurnedOn { get; private set; }    // 点灯しているか

        private Image image;                // 画像部分    
        private byte lastBrightness;        // 最終フレーム時の明るさ

        void Awake()
        {
            IsTurnedOn = false;
            image = GetComponent<Image>();
            lastBrightness = 0;
            ChangeBrightness(turnOffValue);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // 明るさ変更
        public void ChangeBrightness(byte brightness)
        {
            if (lastBrightness != brightness)
            {
                image.color = new Color32(brightness, brightness, brightness, 255);
                lastBrightness = brightness;
            }
        }

        // 点灯
        public void TurnOn()
        {
            if (!IsTurnedOn)
            {
                StopCoroutine(nameof(TurnOffLamp));
                StartCoroutine(nameof(TurnOnLamp));
            }
        }

        // 消灯
        public void TurnOff()
        {
            if (IsTurnedOn)
            {
                StopCoroutine(nameof(TurnOnLamp));
                StartCoroutine(nameof(TurnOffLamp));
            }
        }

        // コルーチン用
        IEnumerator TurnOnLamp()
        {
            IsTurnedOn = true;
            // 明るくなるまでの変化量を求める
            float changeValue = (float)(turnOnValue - turnOffValue) / flashFrames;

            while (lastBrightness < turnOnValue)
            {
                // 数値を超えないように調整
                ChangeBrightness((byte)Math.Clamp(lastBrightness + changeValue, turnOffValue, turnOnValue));
                yield return new WaitForSeconds(LampFlashTime);
            }
        }

        IEnumerator TurnOffLamp()
        {
            IsTurnedOn = false;
            // 暗くなるまでの変化量を求める
            float changeValue = (float)(turnOnValue - turnOffValue) / flashFrames;

            while (lastBrightness > turnOffValue)
            {
                ChangeBrightness((byte)Math.Clamp(lastBrightness - changeValue, turnOffValue, turnOnValue));
                yield return new WaitForSeconds(LampFlashTime);
            }
        }
    }
}
