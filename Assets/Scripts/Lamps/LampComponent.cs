using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LampComponent : MonoBehaviour
{
    // �����v�p
    // const
    // �f�t�H���g�̖��邳(�_����)
    const byte TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    const byte TurnOffValue = 60;
    // �_�������邽�߂ɕK�v�ȃt���[����
    const byte FrameCount = 5;
    // �����v�_���̊Ԋu(�b�Ԋu)
    const float LampFlashTime = 0.01f;

    // var
    // Image
    private Image image;
    // �_�����Ă��邩
    public bool IsTurnedOn { get; private set; }
    // ���݂̖��邳
    private byte brightness;

    // func
    void Awake()
    {
        brightness = TurnOffValue;
        IsTurnedOn = false;
        image = GetComponent<Image>();
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    // �_��
    public void TurnOn()
    {
        if(!IsTurnedOn)
        {
            StopCoroutine(nameof(TurnOffLamp));
            StartCoroutine(nameof(TurnOnLamp));
        }
    }

    // ����
    public void TurnOff()
    {
        if(IsTurnedOn)
        {
            StopCoroutine(nameof(TurnOnLamp));
            StartCoroutine(nameof(TurnOffLamp));
        }
    }

    // �R���[�`���p
    private IEnumerator TurnOnLamp()
    {
        IsTurnedOn = true;
        // ���邳�̌v�Z(0.03�b��������)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        while (brightness < TurnOnValue)
        {
            // ���l�𒴂��Ȃ��悤�ɒ���
            brightness = (byte)Math.Clamp(brightness + changeValue, TurnOffValue, TurnOnValue);

            ////Debug.Log("Brightness:" + brightness);

            image.color = new Color32(brightness, brightness, brightness, 255);

            yield return new WaitForSeconds(LampFlashTime);
        }
    }

    private IEnumerator TurnOffLamp()
    {
        IsTurnedOn = false;
        // ���邳�̌v�Z(0.03�b��������)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        while (brightness > TurnOffValue)
        {
            brightness = (byte)Math.Clamp(brightness - changeValue, TurnOffValue, TurnOnValue);
            image.color = new Color32(brightness, brightness, brightness, 255);

            yield return new WaitForSeconds(LampFlashTime);
        }
    }
}
