using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Reels.Flash
{
    public class FlashManager : MonoBehaviour
    {
        // リールフラッシュ機能

        // const
        // リールフラッシュの間隔(秒間隔)
        public const float ReelFlashTime = 0.01f;
        // 払い出し時のフラッシュに要するフレーム数(0.01秒間隔)
        public const int PayoutFlashFrames = 15;
        // シーク位置オフセット用
        const int SeekOffset = 4;
        // 変更しないときの数値
        const int NoChangeValue = -1;

        // デフォルトのフラッシュID
        public enum FlashID { V_Flash };
        // 払い出しラインのID
        public enum PayoutLineID { PayoutMiddle, PayoutLower, PayoutUpper, PayoutDiagonalA, PayoutDiagonalB };

        // var
        // 現在のフレーム数(1フレーム0.1秒)
        public int CurrentFrame { get; set; }
        // フラッシュ中か
        public bool HasFlash { get; set; }
        // フラッシュで待機中か
        public bool HasFlashWait { get; set; }
        // 現在のフラッシュID
        public int CurrentFlashID { get; set; }
        // フラッシュデータ
        public List<FlashData> FlashDatabase { get; private set; }
        [SerializeField] private List<TextAsset> testAssetList;

        // func
        public void Awake()
        {
            HasFlash = false;
            HasFlashWait = false;
            CurrentFlashID = 0;
            FlashDatabase = new List<FlashData>();

            foreach (TextAsset textAsset in testAssetList)
            {
                StringReader buffer = new StringReader(textAsset.text);
                FlashDatabase.Add(new FlashData(buffer));
            }

            ////Debug.Log("FlashManager awaken");
        }

        // フラッシュデータの処理を反映する
        public void ReadFlashData(ReelObject[] reelObjects)
        {
            if (CurrentFlashID >= FlashDatabase.Count)
            {
                throw new Exception("FlashID is Overflow the flashDatabase");
            }

            int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();

            // 現在のフレームと一致しなければ読み込まない
            if (CurrentFrame == flashData[(int)FlashData.PropertyID.FrameID])
            {
                // リール全て変更
                foreach (ReelObject reel in reelObjects)
                {
                    // 本体と枠下枠上の図柄変更
                    int bodyBright = flashData[(int)FlashData.PropertyID.Body + reel.GetReelID() * SeekOffset];
                    if (bodyBright != NoChangeValue)
                    {
                        reel.SetReelBaseBrightness((byte)bodyBright);
                        reel.SetSymbolBrightness((int)ReelPosID.Lower2nd, (byte)bodyBright);
                        reel.SetSymbolBrightness((int)ReelPosID.Upper2nd, (byte)bodyBright);
                    }

                    // 図柄の明るさ変更
                    for (int i = (int)ReelPosID.Lower2nd; i < (int)ReelPosID.Upper3rd; i++)
                    {
                        int symbolBright = flashData[(int)FlashData.PropertyID.SymbolLower + i + reel.GetReelID() * SeekOffset];

                        //Debug.Log("Symbol:" + i + "Bright:" + symbolBright);
                        if (symbolBright != NoChangeValue)
                        {
                            reel.SetSymbolBrightness(i, (byte)symbolBright);
                        }
                    }
                }

                // データのシーク位置変更
                if (!FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
                {
                    CurrentFrame += 1;
                    FlashDatabase[CurrentFlashID].MoveNextSeek();
                }
                // ループさせるか(ループの場合は特定フレームまで移動させる)
                // しない場合は停止する。
                if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
                {
                    CurrentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
                    FlashDatabase[CurrentFlashID].SetSeek(flashData[(int)FlashData.PropertyID.LoopPosition]);
                }
                // 最終行までよんでループがない場合は終了
                else if (FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
                {
                    HasFlash = false;
                }
            }
            else
            {
                CurrentFrame += 1;
            }
        }

        // 払い出し時のフラッシュ
        public void PayoutFlash(List<PayoutLineData> lastPayoutLines, ReelObject[] reelObjects)
        {
            // 暗くする量を計算
            byte brightness = CalculateBrightness(SymbolChange.TurnOffValue, PayoutFlashFrames);
            //全ての払い出しのあったラインをフラッシュさせる
            foreach (PayoutLineData payoutLine in lastPayoutLines)
            {
                for (int i = 0; i < payoutLine.PayoutLines.Count; i++)
                {
                    // 図柄点灯
                    reelObjects[i].SetSymbolBrightness(payoutLine.PayoutLines[i], brightness);

                    // 左リールにチェリーがある場合はチェリーのみ点灯
                    if (reelObjects[(int)ReelID.ReelLeft].GetReelSymbol
                        (payoutLine.PayoutLines[(int)ReelID.ReelLeft]) == ReelSymbols.Cherry)
                    {
                        break;
                    }
                }
            }
            // ループさせる
            CurrentFrame += 1;

            if (CurrentFrame == PayoutFlashFrames)
            {
                CurrentFrame = 0;
            }
        }

        private byte CalculateBrightness(int turnOffValue, int frame)
        {
            // 明るさの計算(0.01秒で25下げる)
            int distance = SymbolChange.TurnOnValue - turnOffValue;
            float changeValue = distance / frame;
            // 0.01秒で下げる明るさの量(0.08秒でもとに戻る)
            float result = SymbolChange.TurnOnValue - CurrentFrame * changeValue;
            // 数値を超えないように調整
            result = Math.Clamp(result, SymbolChange.TurnOffValue, SymbolChange.TurnOnValue);
            // byte型に変換
            return (byte)Math.Round(result);
        }
    }

}