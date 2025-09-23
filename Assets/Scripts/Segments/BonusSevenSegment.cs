using System;
using System.Collections.Generic;
using UnityEngine;
using static MedalSevenSegment;

public class BonusSevenSegment : MonoBehaviour
{
    // ボーナス用セグメント

    // const
    // セグメント位置のID
    public enum DigitID { JacDigit, JacHitDigit, BarDigit, GamesSecondDigit, GamesFirstDigit}
    // 数字位置のID
    public enum DigitNumID { First, Second, Third, Fourth, Fifth}

    // var
    // 7セグ
    private Segment[] segments;

    // func

    private void Awake()
    {
        segments = GetComponentsInChildren<Segment>();
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
        int SecondDigit = GetDigitValue(rightSegmentsValue, 2);
        // 1の桁を得る
        int FirstDigit = GetDigitValue(rightSegmentsValue, 1);

        // セグメントに反映
        segments[(int)DigitID.GamesFirstDigit].TurnOnLampByNumber(FirstDigit);
        segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(SecondDigit);
    }

    // 獲得枚数を表示
    public void ShowTotalPayout(int totalPayout)
    {
        int result = Math.Clamp(totalPayout, 0, 99999);

        List<int> digits = new List<int>();

        // 見つかった桁数分数値を得る
        for(int i = 0; i < GetDigitCount(totalPayout); i++)
        {
            digits.Add(GetDigitValue(result, i + 1));
        }

        //Debug.Log("Digit Count:" + digits.Count);
        // 桁数に合わせて表示
        // 4桁以上の場合は右詰めで表示し、0埋めはしない
        if(digits.Count >= 4)
        {
            // 5桁目がある場合
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
        // 3桁の場合は真ん中3桁に表示(ない場合は0埋めをする)
        else
        {
            segments[(int)DigitID.JacDigit].TurnOnBar();

            // 3桁ない場合は0を表示
            if (digits.Count >= 3)
            {
                segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(digits[(int)DigitNumID.Third]);
            }
            else
            {
                segments[(int)DigitID.JacHitDigit].TurnOnLampByNumber(0);
            }

            // 2桁ない場合は0を表示
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

    // セグメントをすべて消す
    public void TurnOffAllSegments()
    {
        foreach (Segment segment in segments)
        {
            segment.TurnOffAll();
        }
    }
}
