using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MedalSevenSegment;

public class BonusSevenSegment : MonoBehaviour
{
    // �{�[�i�X�p�Z�O�����g

    // const
    // �Z�O�����g�ʒu��ID
    public enum DigitID { JacDigit, JacHitDigit, BarDigit, GamesSecondDigit, GamesFirstDigit}

    // var
    // 7�Z�O
    private Segment[] segments;

    // func

    void Awake()
    {
        segments = GetComponentsInChildren<Segment>();
        Debug.Log("Counts:" + segments.Length);
    }

    private void Start()
    {
        ShowSegmentByNumber(3, 30);
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
        int SecondDigit = GetDigits(rightSegmentsValue, 2);
        // 1�̌��𓾂�
        int FirstDigit = GetDigits(rightSegmentsValue, 1);

        // �Z�O�����g�ɔ��f
        segments[(int)DigitID.GamesFirstDigit].TurnOnLampByNumber(FirstDigit);

        // 2���ڈȍ~��10�ȏ�łȂ��Ɣ�\��
        if (SecondDigit > 0)
        {
            segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(SecondDigit);
        }
        else
        {
            segments[(int)DigitID.GamesSecondDigit].TurnOffAll();
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
