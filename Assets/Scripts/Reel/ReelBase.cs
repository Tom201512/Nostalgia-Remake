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
    private byte brightness;
    private byte lastBrightness;

    void Awake()
    {
        render = GetComponent<Renderer>();
        lastBrightness = 0;
        ChangeBrightness(TurnOffValue);
    }

    private void Update()
    {
        if(brightness != lastBrightness)
        {
            render.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
            lastBrightness = brightness;
            Debug.Log("Bright:" + brightness);
        }
    }

    public void ChangeBrightness(byte brightness) =>this.brightness = brightness;
}
