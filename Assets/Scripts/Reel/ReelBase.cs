using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // リール本体
    // const
    // デフォルトの明るさ(点灯時)
    public const byte TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    public const byte TurnOffValue = 200;

    // 表示部分
    private Renderer render;

    // 明るさ
    private byte lastBrightness;

    void Awake()
    {
        render = GetComponent<Renderer>();
        lastBrightness = 0;
        ChangeBrightness(TurnOffValue);
    }

    public void ChangeBrightness(byte brightness)
    {
        if(lastBrightness != brightness)
        {
            render.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
            lastBrightness = brightness;
        }
    }
}
