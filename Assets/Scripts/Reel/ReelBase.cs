using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // ���[���{��

    // �\������
    private Renderer render;

    void Awake()
    {
        render = GetComponent<Renderer>();
    }
    // ���[���{�̂��̂��̖̂��邳��ύX
    public void SetBrightness(byte brightness) => ChangeColor(brightness, brightness, brightness);

    // ���[���{�̂��̂��̂̐F��ύX
    public void ChangeColor(byte r, byte g, byte b)
    {
        render.material.SetColor("_Color", new Color32(r, g, b, 255));
        Debug.Log("SetColor" + r + "," + g + "," + b);
    }
}
