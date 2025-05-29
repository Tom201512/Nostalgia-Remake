using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // リール本体
    // const
    // デフォルトの明るさ(点灯時)
    public const int TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    public const int TurnOffValue = 200;

    // 表示部分
    private Renderer render;

    // 明るさ
    public byte Brightness { get; set; }

    void Awake()
    {
        Brightness = TurnOffValue;
        render = GetComponent<Renderer>();
    }

    private void Update()
    {
        render.material.SetColor("_Color", new Color32(Brightness, Brightness, Brightness, 255));
    }
}
