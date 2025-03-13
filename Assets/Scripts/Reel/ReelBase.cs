using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // リール本体

    // 表示部分
    private Renderer render;

    void Awake()
    {
        render = GetComponent<Renderer>();
    }
    // リール本体そのものの明るさを変更
    public void SetBrightness(byte brightness) => ChangeColor(brightness, brightness, brightness);

    // リール本体そのものの色を変更
    public void ChangeColor(byte r, byte g, byte b)
    {
        render.material.SetColor("_Color", new Color32(r, g, b, 255));
        Debug.Log("SetColor" + r + "," + g + "," + b);
    }
}
