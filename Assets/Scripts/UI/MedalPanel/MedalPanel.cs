using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalPanel : MonoBehaviour
{
    // const
    // �f�t�H���g�̖��邳(�_����)
    const byte TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    const byte TurnOffValue = 120;
    // �_�������邽�߂ɕK�v�ȃt���[����
    const byte FrameCount = 3;
    // �����v�_���̊Ԋu(�b�Ԋu)
    const float LampFlashTime = 0.01f;

    // ���_���p�l������
    // var
    // ���_��1�������v
    [SerializeField] private Image medal1;
    // ���_��2�������vA(��)
    [SerializeField] private Image medal2A;
    // ���_��2�������vB(��)
    [SerializeField] private Image medal2B;
    // ���_��3�������vA(��)
    [SerializeField] private Image medal3A;
    // ���_��3�������vB(��)
    [SerializeField] private Image medal3B;

    private bool isMedal1TurnedOn;
    private bool isMedal2TurnedOn;
    private bool isMedal3TurnedOn;

    // func
    public void Awake()
    {
        isMedal1TurnedOn = false;
        isMedal2TurnedOn = false;
        isMedal3TurnedOn = false;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void TurnOnMedal1Lamp() => StartCoroutine(nameof(TurnOnLamp), new Image[] { medal1 });
    public void TurnOnMedal2Lamp() => StartCoroutine(nameof(TurnOnLamp), new Image[] { medal2A, medal2B });
    public void TurnOnMedal3Lamp() => StartCoroutine(nameof(TurnOnLamp), new Image[] { medal3A, medal3B });

    private IEnumerator TurnOnLamp(Image[] lamp)
    {
        Debug.Log("Start TurnOn");

        // ���݂̖��邳
        byte brightness = TurnOffValue;
        // ���邳�̌v�Z(0.01�b��25������)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;
        Debug.Log("ChangeValue:" + changeValue);

        while (brightness < TurnOnValue)
        {
            // ���l�𒴂��Ȃ��悤�ɒ���
            brightness = (byte)Math.Clamp(brightness + changeValue, TurnOffValue, TurnOnValue);

            Debug.Log("Brightness:" + brightness);

            foreach (Image i in lamp)
            {
                i.color = new Color32(brightness, brightness, brightness, 255);
            }

            yield return new WaitForSeconds(LampFlashTime);
        }
        Debug.Log("Lamp turned on");
    }

    private IEnumerator TurnOffLamp(Image[] lamp)
    {
        Debug.Log("Start TurnOff");

        // ���݂̖��邳
        byte brightness = TurnOnValue;
        // ���邳�̌v�Z(0.01�b��25������)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = (float)distance / FrameCount;

        // 0.01�b�ŏグ�閾�邳�̗�
        float result = brightness + changeValue;
        // ���l�𒴂��Ȃ��悤�ɒ���
        result = Math.Clamp(result, TurnOffValue, TurnOnValue);

        while (brightness < TurnOffValue)
        {
            // byte�^�ɕϊ����đ������킹��
            brightness += (byte)result;

            Debug.Log("Brightness:" + brightness);

            foreach (Image i in lamp)
            {
                i.color = new Color32(brightness, brightness, brightness, 255);
            }

            yield return new WaitForSeconds(LampFlashTime);
        }
        Debug.Log("Lamp turned on");
    }
}
