using UnityEngine;

public class ReelBase : MonoBehaviour
{
    // ���[���{��
    // const
    // �f�t�H���g�̖��邳(�_����)
    public const int TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    public const int TurnOffValue = 200;

    // �\������
    private Renderer render;

    // ���邳
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
