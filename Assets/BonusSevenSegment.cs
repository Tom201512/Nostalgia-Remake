using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MedalSevenSegment;

public class BonusSevenSegment : MonoBehaviour
{
    // ボーナス用セグメント

    // const
    // セグメント位置のID
    public enum DigitID { JacDigit, JacHitDigit, BarDigit, GamesSecondDigit, GamesFirstDigit}

    // var
    // 7セグ
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

    // BIG中のボーナス表示
    public void ShowBigStatus(int remainingJac, int remainingBigGames) => ShowSegmentByNumber(remainingJac, remainingBigGames);
    // JAC中のボーナス表示
    public void ShowJacStatus(int remainingJac, int remainingJacHits) => ShowSegmentByNumber(remainingJac, remainingJacHits);

    // ボーナス状態を表示
    private void ShowSegmentByNumber(int leftSegmentsValue, int rightSegmentsValue)
    {
        // 0~99に調整
        leftSegmentsValue = Math.Clamp(leftSegmentsValue, 0, 9);
        rightSegmentsValue = Math.Clamp(rightSegmentsValue, 0, 99);

        // JAC部分の桁数を表示
        segments[(int)DigitID.JacDigit].TurnOnJAC();
        segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(leftSegmentsValue);

        // ハイフン
        segments[(int)DigitID.BarDigit].TurnOnBar();

        // ゲーム数表示(BIG残りゲーム数/JAC残り当選回数)
        // 10の桁を得る
        int SecondDigit = GetDigits(rightSegmentsValue, 2);
        // 1の桁を得る
        int FirstDigit = GetDigits(rightSegmentsValue, 1);

        // セグメントに反映
        segments[(int)DigitID.GamesFirstDigit].TurnOnLampByNumber(FirstDigit);

        // 2桁目以降は10以上でないと非表示
        if (SecondDigit > 0)
        {
            segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(SecondDigit);
        }
        else
        {
            segments[(int)DigitID.GamesSecondDigit].TurnOffAll();
        }
    }

    // セグメントをすべて消す
    public void TurnOffAllSegments()
    {
        foreach (Segment segment in segments)
        {
            segment.TurnOffAll();
        }
    }
}
