using System;
using UnityEngine;

public class MedalSevenSegment : MonoBehaviour
{
    // ���_���p�Z�O�����g

    // const
    // ������ID
    public enum DigitID{SecondDigit, FirstDigit}

    // var
    // 7�Z�O
    private Segment[] segments;

    // func

    void Awake()
    {
        segments = GetComponentsInChildren<Segment>();
    }

    // �w�肵�������̃��_����\��
    public void ShowSegmentByNumber(int num)
    {
        // 0~99�ɒ���
        num = Math.Clamp(num, 0, 99);

        // 10�̌��𓾂�
        int SecondDigit = GetDigitValue(num, 2);

        // 1�̌��𓾂�
        int FirstDigit = GetDigitValue(num, 1);

        // �Z�O�����g�ɔ��f
        segments[(int)DigitID.FirstDigit].TurnOnLampByNumber(FirstDigit);

        // 2���ڈȍ~��10�ȏ�łȂ��Ɣ�\��
        if(SecondDigit > 0)
        {
            segments[(int)DigitID.SecondDigit].TurnOnLampByNumber(SecondDigit);
        }
        else
        {
            segments[(int)DigitID.SecondDigit].TurnOffAll();
        }
    }

    // �Z�O�����g�����ׂď���
    public void TurnOffAllSegments()
    {
        foreach(Segment segment in segments)
        {
            segment.TurnOffAll();
        }
    }

    // �������v�Z����
    public static int GetDigitCount(int value)
    {
        int sum = 0;
        int digitsCount = 0;

        // 0�̏ꍇ��1����Ԃ�
        if(value == 0)
        {
            return 1;
        }
        // �w�茅���܂Ő������o��
        while(value != 0)
        {
            sum = (value % 10);
            value = (value / 10);
            digitsCount += 1;
        }

        return digitsCount;
    }

    // �w�肵�����ɂ��鐔�������߂�
    public static int GetDigitValue(int value, int digit)
    {
        int sum = 0;
        // �w�茅���܂Ő������o��
        for (int i = 0; i < digit; i++)
        {
            sum = (value % 10);
            value = (value / 10);
        }

        return sum;
    }

}
