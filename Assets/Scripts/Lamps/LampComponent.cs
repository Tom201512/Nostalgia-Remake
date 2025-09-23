using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LampComponent : MonoBehaviour
{
    // �����v�p
    // const
    // �f�t�H���g�̖��邳(�_����)
    const byte DefaultTurnOn = 255;
    // �f�t�H���g�̈Â�(������)
    const byte DefaultTurnOff = 60;
    // �f�t�H���g�̓_�Ŏ��t���[����
    const byte DefaultFlashFrames = 3;
    // �����v�_���̊Ԋu(�b�Ԋu)
    const float LampFlashTime = 0.01f;

    // var
    // �_�����̖��邳
    [SerializeField] private byte turnOnValue = DefaultTurnOn;
    // �������̖��邳
    [SerializeField] private byte turnOffValue = DefaultTurnOff;

    // �_�Ŏ��̃t���[����(0.01�b)
    [SerializeField] private byte flashFrames = DefaultFlashFrames;


    // Image
    private Image image;
    // �_�����Ă��邩
    public bool IsTurnedOn { get; private set; }
    // �ŏI�t���[�����̖��邳
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
        // ���邭�Ȃ�܂ł̕ω��ʂ����߂�
        float changeValue = (float)(turnOnValue - turnOffValue) / flashFrames;

        while (lastBrightness < turnOnValue)
        {
            // ���l�𒴂��Ȃ��悤�ɒ���
            ChangeBrightness((byte)Math.Clamp(lastBrightness + changeValue, turnOffValue, turnOnValue));
            ////Debug.Log("Brightness:" + brightness);
            yield return new WaitForSeconds(LampFlashTime);
        }
    }

    private IEnumerator TurnOffLamp()
    {
        IsTurnedOn = false;
        // �Â��Ȃ�܂ł̕ω��ʂ����߂�
        float changeValue = (float)(turnOnValue - turnOffValue) / flashFrames;

        while (lastBrightness > turnOffValue)
        {
            ChangeBrightness((byte)Math.Clamp(lastBrightness - changeValue, turnOffValue, turnOnValue));
            yield return new WaitForSeconds(LampFlashTime);
        }
    }
}
