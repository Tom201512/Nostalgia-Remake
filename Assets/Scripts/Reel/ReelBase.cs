using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // ���[���{��
    // const
    // �f�t�H���g�̖��邳(�_����)
    public const byte TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    public const byte TurnOffValue = 180;

    // �\������
    private Renderer render;

    // ���邳
    private byte lastBrightness;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        lastBrightness = 0;
    }

    private void Start()
    {
        ChangeBrightness(TurnOffValue);
    }

    // �F�ύX
    public void ChangeBrightness(byte brightness)
    {
        if (lastBrightness != brightness)
        {
            render.material.SetColor("_Color", new Color32(brightness, brightness, brightness, 255));
            lastBrightness = brightness;
        }
    }
}
