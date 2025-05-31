using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LampComponent : MonoBehaviour
{
    // ランプ用
    // const
    // デフォルトの明るさ(点灯時)
    const byte TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    const byte TurnOffValue = 60;
    // 点灯させるために必要なフレーム数
    const byte FrameCount = 3;
    // ランプ点灯の間隔(秒間隔)
    const float LampFlashTime = 0.01f;

    // var
    // Image
    private Image image;
    // 点灯しているか
    public bool IsTurnedOn { get; private set; }
    // 最終フレーム時の明るさ
    private byte lastBrightness;

    // func
    void Awake()
    {
        IsTurnedOn = false;
        image = GetComponent<Image>();
        lastBrightness = 0;
        ChangeBrightness(TurnOffValue);
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void ChangeBrightness(byte brightness)
    {
        if(lastBrightness != brightness)
        {
            image.color = new Color32(brightness, brightness, brightness, 255);
            lastBrightness = brightness;
        }
    }

    // 点灯
    public void TurnOn()
    {
        if(!IsTurnedOn)
        {
            StopCoroutine(nameof(TurnOffLamp));
            StartCoroutine(nameof(TurnOnLamp));
        }
    }

    // 消灯
    public void TurnOff()
    {
        if(IsTurnedOn)
        {
            StopCoroutine(nameof(TurnOnLamp));
            StartCoroutine(nameof(TurnOffLamp));
        }
    }

    // コルーチン用
    private IEnumerator TurnOnLamp()
    {
        IsTurnedOn = true;
        // 明るさの計算(0.03秒ずつ下げる)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        while (lastBrightness < TurnOnValue)
        {
            // 数値を超えないように調整
            ChangeBrightness((byte)Math.Clamp(lastBrightness + changeValue, TurnOffValue, TurnOnValue));
            ////Debug.Log("Brightness:" + brightness);
            yield return new WaitForSeconds(LampFlashTime);
        }
    }

    private IEnumerator TurnOffLamp()
    {
        IsTurnedOn = false;
        // 明るさの計算(0.03秒ずつ下げる)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        while (lastBrightness > TurnOffValue)
        {
            ChangeBrightness((byte)Math.Clamp(lastBrightness - changeValue, TurnOffValue, TurnOnValue));
            yield return new WaitForSeconds(LampFlashTime);
        }
    }
}
