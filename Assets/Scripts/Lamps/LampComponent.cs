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
    const byte FrameCount = 5;
    // ランプ点灯の間隔(秒間隔)
    const float LampFlashTime = 0.01f;

    // var
    // Image
    private Image image;
    // 点灯しているか
    public bool IsTurnedOn { get; private set; }
    // 現在の明るさ
    private byte brightness;

    // func
    void Awake()
    {
        brightness = TurnOffValue;
        IsTurnedOn = false;
        image = GetComponent<Image>();
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    // 点灯
    public void TurnOn()
    {
        if(!IsTurnedOn)
        {
            StartCoroutine(nameof(TurnOnLamp));
        }
    }

    // 消灯
    public void TurnOff()
    {
        if(IsTurnedOn)
        {
            StartCoroutine(nameof(TurnOffLamp));
        }
    }

    // コルーチン用
    private IEnumerator TurnOnLamp()
    {
        StopCoroutine(nameof(TurnOffLamp));
        IsTurnedOn = true;
        // 明るさの計算(0.03秒ずつ下げる)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        while (brightness < TurnOnValue)
        {
            // 数値を超えないように調整
            brightness = (byte)Math.Clamp(brightness + changeValue, TurnOffValue, TurnOnValue);

            //Debug.Log("Brightness:" + brightness);

            image.color = new Color32(brightness, brightness, brightness, 255);

            yield return new WaitForSeconds(LampFlashTime);
        }
    }

    private IEnumerator TurnOffLamp()
    {
        StopCoroutine(nameof(TurnOnLamp));
        IsTurnedOn = false;
        // 明るさの計算(0.03秒ずつ下げる)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        while (brightness > TurnOffValue)
        {
            brightness = (byte)Math.Clamp(brightness - changeValue, TurnOffValue, TurnOnValue);
            image.color = new Color32(brightness, brightness, brightness, 255);

            yield return new WaitForSeconds(LampFlashTime);
        }
    }
}
