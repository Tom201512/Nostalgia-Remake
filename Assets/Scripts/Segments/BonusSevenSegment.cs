using System;
using System.Collections.Generic;
using UnityEngine;
using static MedalSevenSegment;

public class BonusSevenSegment : MonoBehaviour
{
    // �{�[�i�X�p�Z�O�����g

    // const
    // �Z�O�����g�ʒu��ID
    public enum DigitID { JacDigit, JacHitDigit, BarDigit, GamesSecondDigit, GamesFirstDigit}
    // �����ʒu��ID
    public enum DigitNumID { First, Second, Third, Fourth, Fifth}

    // var
    // 7�Z�O
    private Segment[] segments;

    // func

    private void Awake()
    {
        segments = GetComponentsInChildren<Segment>();
    }

    // BIG���̃{�[�i�X�\��
    public void ShowBigStatus(int remainingJac, int remainingBigGames) => ShowSegmentByNumber(remainingJac, remainingBigGames);
    // JAC���̃{�[�i�X�\��
    public void ShowJacStatus(int remainingJac, int remainingJacHits) => ShowSegmentByNumber(remainingJac, remainingJacHits);

    // �{�[�i�X��Ԃ�\��
    private void ShowSegmentByNumber(int leftSegmentsValue, int rightSegmentsValue)
    {
        // 0~99�ɒ���
        leftSegmentsValue = Math.Clamp(leftSegmentsValue, 0, 9);
        rightSegmentsValue = Math.Clamp(rightSegmentsValue, 0, 99);

        // JAC�����̌�����\��
        segments[(int)DigitID.JacDigit].TurnOnJAC();
        segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(leftSegmentsValue);

        // �n�C�t��
        segments[(int)DigitID.BarDigit].TurnOnBar();

        // �Q�[�����\��(BIG�c��Q�[����/JAC�c�蓖�I��)
        // 10�̌��𓾂�
        int SecondDigit = GetDigitValue(rightSegmentsValue, 2);
        // 1�̌��𓾂�
        int FirstDigit = GetDigitValue(rightSegmentsValue, 1);

        // �Z�O�����g�ɔ��f
        segments[(int)DigitID.GamesFirstDigit].TurnOnLampByNumber(FirstDigit);
        segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(SecondDigit);
    }

    // �l��������\��
    public void ShowTotalPayout(int totalPayout)
    {
        int result = Math.Clamp(totalPayout, 0, 99999);

        List<int> digits = new List<int>();

        // �����������������l�𓾂�
        for(int i = 0; i < GetDigitCount(totalPayout); i++)
        {
            digits.Add(GetDigitValue(result, i + 1));
        }

        //Debug.Log("Digit Count:" + digits.Count);
        // �����ɍ��킹�ĕ\��
        // 4���ȏ�̏ꍇ�͉E�l�߂ŕ\�����A0���߂͂��Ȃ�
        if(digits.Count >= 4)
        {
            // 5���ڂ�����ꍇ
            if(digits.Count == 5)
            {
                segments[(int)DigitID.JacDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Fifth]);
            }
            else
            {
                segments[(int)DigitID.JacDigit].TurnOffAll();
            }

            segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Fourth]);
            segments[(int)DigitID.BarDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Third]);
            segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Second]);
            segments[(int)DigitID.GamesFirstDigit].TurnOnLampByNumber(digits[(int)DigitNumID.First]);
        }
        // 3���̏ꍇ�͐^��3���ɕ\��(�Ȃ��ꍇ��0���߂�����)
        else
        {
            segments[(int)DigitID.JacDigit].TurnOnBar();

            // 3���Ȃ��ꍇ��0��\��
            if (digits.Count >= 3)
            {
                segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Third]);
            }
            else
            {
                segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(0);
            }

            // 2���Ȃ��ꍇ��0��\��
            if (digits.Count >= 2)
            {
                segments[(int)DigitID.BarDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Second]);
            }
            else
            {
                segments[(int)DigitID.BarDigit].TurnOnLampByNumber(0);
            }

            segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(digits[(int)DigitNumID.First]);
            segments[(int)DigitID.GamesFirstDigit].TurnOnBar();
        }
    }

    // �Z�O�����g�����ׂď���
    public void TurnOffAllSegments()
    {
        foreach (Segment segment in segments)
        {
            segment.TurnOffAll();
        }
    }
}
