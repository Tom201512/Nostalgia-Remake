using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalPanel : MonoBehaviour
{
    // const
    // デフォルトの明るさ(点灯時)
    const byte TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    const byte TurnOffValue = 120;
    // 点灯させるために必要なフレーム数
    const byte FrameCount = 3;
    // ランプ点灯の間隔(秒間隔)
    const float LampFlashTime = 0.01f;

    // メダルパネル部分
    // var
    // メダル1枚ランプ
    [SerializeField] private Image medal1;
    // メダル2枚ランプA(上)
    [SerializeField] private Image medal2A;
    // メダル2枚ランプB(下)
    [SerializeField] private Image medal2B;
    // メダル3枚ランプA(上)
    [SerializeField] private Image medal3A;
    // メダル3枚ランプB(下)
    [SerializeField] private Image medal3B;

    private bool isMedal1TurnedOn;
    private bool isMedal2TurnedOn;
    private bool isMedal3TurnedOn;

    // func
    public void Awake()
    {
        isMedal1TurnedOn = false;
        isMedal2TurnedOn = false;
        isMedal3TurnedOn = false;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void TurnOnMedal1Lamp() => StartCoroutine(nameof(TurnOnLamp), new Image[] { medal1 });
    public void TurnOnMedal2Lamp() => StartCoroutine(nameof(TurnOnLamp), new Image[] { medal2A, medal2B });
    public void TurnOnMedal3Lamp() => StartCoroutine(nameof(TurnOnLamp), new Image[] { medal3A, medal3B });

    private IEnumerator TurnOnLamp(Image[] lamp)
    {
        Debug.Log("Start TurnOn");

        // 現在の明るさ
        byte brightness = TurnOffValue;
        // 明るさの計算(0.01秒で25下げる)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;
        Debug.Log("ChangeValue:" + changeValue);

        while (brightness < TurnOnValue)
        {
            // 数値を超えないように調整
            brightness = (byte)Math.Clamp(brightness + changeValue, TurnOffValue, TurnOnValue);

            Debug.Log("Brightness:" + brightness);

            foreach (Image i in lamp)
            {
                i.color = new Color32(brightness, brightness, brightness, 255);
            }

            yield return new WaitForSeconds(LampFlashTime);
        }
        Debug.Log("Lamp turned on");
    }

    private IEnumerator TurnOffLamp(Image[] lamp)
    {
        Debug.Log("Start TurnOff");

        // 現在の明るさ
        byte brightness = TurnOnValue;
        // 明るさの計算(0.01秒で25下げる)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        // 0.01秒で上げる明るさの量
        float result = brightness + changeValue;
        // 数値を超えないように調整
        result = Math.Clamp(result, TurnOffValue, TurnOnValue);

        while (brightness < TurnOffValue)
        {
            // byte型に変換して足し合わせる
            brightness += (byte)result;

            Debug.Log("Brightness:" + brightness);

            foreach (Image i in lamp)
            {
                i.color = new Color32(brightness, brightness, brightness, 255);
            }

            yield return new WaitForSeconds(LampFlashTime);
        }
        Debug.Log("Lamp turned on");
    }
}
