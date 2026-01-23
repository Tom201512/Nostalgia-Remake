using ReelSpinGame_Lamps;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Lamps.SegmentLampUtil;

namespace ReelSpinGame_Medal.Segment
{
    // メダル用セグメント
    public class MedalSevenSegment : MonoBehaviour
    {
        public enum DigitID { SecondDigit, FirstDigit }         // 桁数のID
        public const float SegmentUpdateFrame = 0.12f;          // 数値更新の間隔(ミリ秒)

        public bool HasSegmentUpdate { get; private set; } // セグメントを更新中か

        SegmentLamp[] segments;    // 7セグ
        int tweenFromValue;  // 補完の起点
        int tweenToValue;    // 補完の終点

        void Awake()
        {
            HasSegmentUpdate = false;
            segments = GetComponentsInChildren<SegmentLamp>();
            tweenFromValue = 0;
            tweenToValue = 0;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // 指定した桁数のメダルを表示
        public void ShowSegmentByNumber(int num)
        {
            num = Math.Clamp(num, 0, 99);        // 0~99に調整
                                                 // 各桁ごとの数値を得る
            int secondDigit = GetDigitValue(num, 2);
            int firstDigit = GetDigitValue(num, 1);

            // セグメントに反映
            segments[(int)DigitID.FirstDigit].TurnOnLampByNumber(firstDigit);

            // 2桁目以降は10以上でないと非表示
            if (secondDigit > 0)
            {
                segments[(int)DigitID.SecondDigit].TurnOnLampByNumber(secondDigit);
            }
            else
            {
                segments[(int)DigitID.SecondDigit].TurnOffAll();
            }
        }

        // 台設定メモリエラーID(rr)を表示
        public void StartDisplayError()
        {
            foreach (SegmentLamp segment in segments)
            {
                segment.TurnOnR();
            }
        }

        // 指定した数値までセグメントを増減させる
        public void DoSegmentTween(int fromValue, int toValue)
        {
            tweenFromValue = fromValue;
            tweenToValue = toValue;
            StartCoroutine(nameof(SegmentTweenUpdate));
        }

        // セグメントをすべて消す
        public void TurnOffAllSegments()
        {
            foreach (SegmentLamp segment in segments)
            {
                segment.TurnOffAll();
            }
        }

        // 数値増加、減少の補完
        IEnumerator SegmentTweenUpdate()
        {
            HasSegmentUpdate = true;

            int tweenValue = tweenFromValue;
            // 補完処理
            while (tweenValue != tweenToValue)
            {
                if (tweenValue > tweenToValue)
                {
                    tweenValue -= 1;
                }
                else
                {
                    tweenValue += 1;
                }
                ShowSegmentByNumber(tweenValue);
                yield return new WaitForSeconds(SegmentUpdateFrame);
            }

            HasSegmentUpdate = false;
        }
    }
}
