using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections;
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
        // リールオブジェクト
        private List<ReelObject> reelObjects;
        // 最後にあった払い出しライン
        private List<PayoutLineData> lastPayoutLines;

        // 現在のフレーム数(1フレーム0.1秒)
        private int currentFrame;
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
            reelObjects = new List<ReelObject>();
            lastPayoutLines = new List<PayoutLineData>();

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

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // func
        // データを渡す
        // リールオブジェクト
        public void SetReelObjects(List<ReelObject> reelObjects) => this.reelObjects = reelObjects;

        // リールフラッシュを再生させる
        public void StartReelFlash(float waitSeconds, FlashID flashID)
        {
            currentFrame = 0;
            HasFlash = true;
            FlashDatabase[(int)flashID].SetSeek(0);
            StartCoroutine(nameof(UpdateFlash));

            if (waitSeconds > 0)
            {
                HasFlashWait = true;
                StartCoroutine(nameof(SetFlashWait), waitSeconds);
            }
        }

        // 払い出しフラッシュの再生
        public void StartPayoutFlash(float waitSeconds, List<PayoutLineData> lastPayoutLines)
        {
            this.lastPayoutLines.Clear();
            this.lastPayoutLines = lastPayoutLines;
            currentFrame = 0;
            HasFlash = true;
            StartCoroutine(nameof(UpdatePayoutFlash));

            if (waitSeconds > 0)
            {
                HasFlashWait = true;
                StartCoroutine(nameof(SetTimeOut), waitSeconds);
            }
        }

        // 払い出しフラッシュの停止
        public void StopFlash()
        {
            HasFlash = false;
            HasFlashWait = false;
        }

        // リールライトをすべて明るくする
        public void TurnOnAllReels()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.SetReelBaseBrightness(ReelBase.TurnOnValue);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.SetSymbolBrightness(i, ReelBase.TurnOnValue);
                }
            }
        }

        // リールライトをすべて暗くする
        public void TurnOffAllReels()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.SetReelBaseBrightness(ReelBase.TurnOffValue);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.SetSymbolBrightness(i, ReelBase.TurnOffValue);
                }
            }
        }

        // JAC GAME時のライト点灯
        public void EnableJacGameLight()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.SetReelBaseBrightness(SymbolChange.TurnOffValue);

                // 真ん中以外点灯
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    if (i == (int)ReelPosID.Center)
                    {
                        reel.SetSymbolBrightness(i, SymbolChange.TurnOnValue);
                    }
                    else
                    {
                        reel.SetSymbolBrightness(i, SymbolChange.TurnOffValue);
                    }
                }
            }
        }

        // 真ん中に近くなったリールを徐々に光らせる

        // フラッシュデータの処理を反映する
        private void ReadFlashData()
        {
            if (CurrentFlashID >= FlashDatabase.Count)
            {
                throw new Exception("FlashID is Overflow the flashDatabase");
            }

            int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();

            // 現在のフレームと一致しなければ読み込まない
            if (currentFrame == flashData[(int)FlashData.PropertyID.FrameID])
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
                    for (int i = (int)ReelPosID.Lower2nd; i < (int)ReelPosID.Upper2nd; i++)
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
                    currentFrame += 1;
                    FlashDatabase[CurrentFlashID].MoveNextSeek();
                }
                // ループさせるか(ループの場合は特定フレームまで移動させる)
                // しない場合は停止する。
                if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
                {
                    currentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
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
                currentFrame += 1;
            }
        }

        // 払い出し時のフラッシュ
        private void PayoutFlash()
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
            currentFrame += 1;

            if (currentFrame == PayoutFlashFrames)
            {
                currentFrame = 0;
            }
        }

        private byte CalculateBrightness(int turnOffValue, int frame)
        {
            // 明るさの計算(0.01秒で25下げる)
            int distance = SymbolChange.TurnOnValue - turnOffValue;
            float changeValue = distance / frame;
            // 0.01秒で下げる明るさの量(0.08秒でもとに戻る)
            float result = SymbolChange.TurnOnValue - currentFrame * changeValue;
            // 数値を超えないように調整
            result = Math.Clamp(result, SymbolChange.TurnOffValue, SymbolChange.TurnOnValue);
            // byte型に変換
            return (byte)Math.Round(result);
        }

        // リールフラッシュのイベント
        private IEnumerator UpdateFlash()
        {
            while (HasFlash)
            {
                ReadFlashData();
                yield return new WaitForSeconds(ReelFlashTime);
            }
        }

        // 払い出しフラッシュのイベント
        private IEnumerator UpdatePayoutFlash()
        {
            while (HasFlash)
            {
                PayoutFlash();
                yield return new WaitForSeconds(ReelFlashTime);
            }
            yield break;
        }

        // タイムアウト用イベント(時間経過でフラッシュ終了)
        private IEnumerator SetTimeOut(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            ////Debug.Log("Replay Finished");
            HasFlashWait = false;
            HasFlash = false;
            TurnOnAllReels();
        }

        // フラッシュ用ウェイトイベント
        private IEnumerator SetFlashWait(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            ////Debug.Log("Replay Finished");
            HasFlashWait = false;
        }
    }
}