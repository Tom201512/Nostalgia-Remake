using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // ���[���{��
    // const
    // �f�t�H���g�̖��邳(�_����)
    public const byte TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    public const byte TurnOffValue = 200;

    // �\������
    private Renderer render;

    // ���邳
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
