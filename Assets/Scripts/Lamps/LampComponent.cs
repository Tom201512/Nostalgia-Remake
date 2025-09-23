using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LampComponent : MonoBehaviour
{
    // ランプ用
    // const
    // デフォルトの明るさ(点灯時)
    const byte DefaultTurnOn = 255;
    // デフォルトの暗さ(消灯時)
    const byte DefaultTurnOff = 60;
    // デフォルトの点滅時フレーム数
    const byte DefaultFlashFrames = 3;
    // ランプ点灯の間隔(秒間隔)
    const float LampFlashTime = 0.01f;

    // var
    // 点灯時の明るさ
    [SerializeField] private byte turnOnValue = DefaultTurnOn;
    // 消灯時の明るさ
    [SerializeField] private byte turnOffValue = DefaultTurnOff;

    // 点滅時のフレーム数(0.01秒)
    [SerializeField] private byte flashFrames = DefaultFlashFrames;


    // Image
    private Image image;
    // 点灯しているか
    public bool IsTurnedOn { get; private set; }
    // 最終フレーム時の明るさ
    private byte lastBrightness;

    // func
    private void Awake()
    {
        IsTurnedOn = false;
        image = GetComponent<Image>();
        lastBrightness = 0;
        ChangeBrightness(turnOffValue);
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
        // 明るくなるまでの変化量を求める
        float changeValue = (float)(turnOnValue - turnOffValue) / flashFrames;

        while (lastBrightness < turnOnValue)
        {
            // 数値を超えないように調整
            ChangeBrightness((byte)Math.Clamp(lastBrightness + changeValue, turnOffValue, turnOnValue));
            ////Debug.Log("Brightness:" + brightness);
            yield return new WaitForSeconds(LampFlashTime);
        }
    }

    private IEnumerator TurnOffLamp()
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
