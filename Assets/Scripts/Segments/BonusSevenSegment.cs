using ReelSpinGame_Lamps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Lamps.SegmentLampUtil;

public class BonusSevenSegment : MonoBehaviour
{
    // ボーナス用セグメント

    // const
    const float PayoutSegFlashTime = 0.5f;      // 獲得枚数表示点滅時間(ミリ秒)
    const float DisplayChangeTime = 2.0f;       // 獲得枚数表示切替時間(ミリ秒)

    // セグメント位置のID
    enum DigitID 
    { 
        JacDigit, 
        JacHitDigit, 
        BarDigit, 
        GamesSecondDigit, 
        GamesFirstDigit
    }
    // 数字位置のID
    enum DigitNumID 
    { 
        First, 
        Second, 
        Third, 
        Fourth, 
        Fifth
    }

    // var
    public int TotalPayoutValue { get; private set; }   // ボーナスの獲得枚数
    public int ZonePayoutValue { get; private set; }    // ゾーン区間の獲得枚数
    public bool HasZone {  get; private set; }          // ゾーン区間にいるか
    public bool IsDisplaying { get; private set; }      // 獲得枚数を表示しているか

    bool isDisplayingZone;      // ゾーン区間を表示するか

    SegmentLamp[] segments;    // 7セグ

    // func

    void Awake()
    {
        segments = GetComponentsInChildren<SegmentLamp>();
        IsDisplaying = false;
        isDisplayingZone = false;
        TotalPayoutValue = 0;
        ZonePayoutValue = 0;
        HasZone = false;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    // BIG中のボーナス表示
    public void ShowBigStatus(int remainingJac, int remainingBigGames) => ShowSegmentByNumber(remainingJac, remainingBigGames);
    // JAC中のボーナス表示
    public void ShowJacStatus(int remainingJac, int remainingJacHits) => ShowSegmentByNumber(remainingJac, remainingJacHits);

    // 獲得枚数の表示を行う
    public void StartDisplayBonusPayout(int totalPayoutValue, int zonePayoutValue, bool hasZone)
    {
        TotalPayoutValue = totalPayoutValue;
        ZonePayoutValue = zonePayoutValue;
        HasZone = hasZone;
        StartCoroutine(nameof(UpdateShowPayout));
    }

    // セグメントをすべて消す
    public void TurnOffAllSegments()
    {
        foreach (SegmentLamp segment in segments)
        {
            segment.TurnOffAll();
        }
    }

    // 獲得枚数表示を終了する
    public void EndDisplayBonusPayout()
    {
        IsDisplaying = false;
        isDisplayingZone = false;
        StopAllCoroutines();
    }

    // ボーナス状態を表示
    void ShowSegmentByNumber(int leftSegmentsValue, int rightSegmentsValue)
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
        // 桁数を得る
        int SecondDigit = GetDigitValue(rightSegmentsValue, 2);
        int FirstDigit = GetDigitValue(rightSegmentsValue, 1);

        // セグメントに反映
        segments[(int)DigitID.GamesFirstDigit].TurnOnLampByNumber(FirstDigit);
        segments[(int)DigitID.GamesSecondDigit].TurnOnLampByNumber(SecondDigit);
    }

    // 獲得枚数を表示
    void ShowTotalPayout(int totalPayout)
    {
        int result = Math.Clamp(totalPayout, 0, 99999);

        List<int> digits = new List<int>();

        // 見つかった桁数分数値を得る
        for(int i = 0; i < GetDigitCount(totalPayout); i++)
        {
            digits.Add(GetDigitValue(result, i + 1));
        }

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

    // 獲得枚数を点滅させる
    IEnumerator UpdateShowPayout()
    {
        IsDisplaying = true;
        StartCoroutine(nameof(ChangeShowType));

        while (IsDisplaying)
        {
            if (isDisplayingZone)
            {
                ShowTotalPayout(ZonePayoutValue);
            }
            else
            {
                ShowTotalPayout(TotalPayoutValue);
            }

            yield return new WaitForSeconds(PayoutSegFlashTime);
            TurnOffAllSegments();
            yield return new WaitForSeconds(PayoutSegFlashTime);
        }
    }

    // 獲得枚数とゾーン区間を切り替える
    IEnumerator ChangeShowType()
    {
        while (IsDisplaying)
        {
            isDisplayingZone = false;
            yield return new WaitForSeconds(DisplayChangeTime);
            isDisplayingZone = true;
            yield return new WaitForSeconds(DisplayChangeTime);
        }
    }
}
