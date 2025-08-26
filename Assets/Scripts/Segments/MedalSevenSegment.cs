using System;
using UnityEngine;

public class MedalSevenSegment : MonoBehaviour
{
    // メダル用セグメント

    // const
    // 桁数のID
    public enum DigitID{SecondDigit, FirstDigit}

    // var
    // 7セグ
    private Segment[] segments;

    // func

    void Awake()
    {
        segments = GetComponentsInChildren<Segment>();
    }

    // 指定した桁数のメダルを表示
    public void ShowSegmentByNumber(int num)
    {
        // 0~99に調整
        num = Math.Clamp(num, 0, 99);

        // 10の桁を得る
        int SecondDigit = GetDigitValue(num, 2);

        // 1の桁を得る
        int FirstDigit = GetDigitValue(num, 1);

        // セグメントに反映
        segments[(int)DigitID.FirstDigit].TurnOnLampByNumber(FirstDigit);

        // 2桁目以降は10以上でないと非表示
        if(SecondDigit > 0)
        {
            segments[(int)DigitID.SecondDigit].TurnOnLampByNumber(SecondDigit);
        }
        else
        {
            segments[(int)DigitID.SecondDigit].TurnOffAll();
        }
    }

    // セグメントをすべて消す
    public void TurnOffAllSegments()
    {
        foreach(Segment segment in segments)
        {
            segment.TurnOffAll();
        }
    }

    // 桁数を計算する
    public static int GetDigitCount(int value)
    {
        int sum = 0;
        int digitsCount = 0;

        // 0の場合は1桁を返す
        if(value == 0)
        {
            return 1;
        }
        // 指定桁数まで数字を出す
        while(value != 0)
        {
            sum = (value % 10);
            value = (value / 10);
            digitsCount += 1;
        }

        return digitsCount;
    }

    // 指定した桁にある数字を求める
    public static int GetDigitValue(int value, int digit)
    {
        int sum = 0;
        // 指定桁数まで数字を出す
        for (int i = 0; i < digit; i++)
        {
            sum = (value % 10);
            value = (value / 10);
        }

        return sum;
    }

}
