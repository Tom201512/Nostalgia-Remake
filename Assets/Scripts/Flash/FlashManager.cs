using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Symbol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelManagerModel;

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
        // リール演出マネージャー
        public ReelEffectManager ReelEffectManager { get; private set; }

        // 最後の払い出し結果
        private PayoutResultBuffer lastPayoutResult;
        // 最後に止めたリール結果
        private LastStoppedReelData lastStoppedReelData;

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
        private void Awake()
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

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // func
        // リール演出機能のセット
        public void SetReelEffectManager(ReelEffectManager reelEffectManager) => ReelEffectManager = reelEffectManager;

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
        public void StartPayoutFlash(float waitSeconds, PayoutResultBuffer lastPayoutResult, LastStoppedReelData lastStoppedReelData)
        {
            this.lastPayoutResult = lastPayoutResult;
            this.lastStoppedReelData = lastStoppedReelData;
            currentFrame = 0;
            HasFlash = true;
            StartCoroutine(nameof(UpdatePayoutFlash));

            if (waitSeconds > 0)
            {
                HasFlashWait = true;
                StartCoroutine(nameof(SetTimeOut), waitSeconds);
            }
        }

        // フラッシュの強制停止
        public void ForceStopFlash()
        {
            HasFlash = false;
            HasFlashWait = false;
            StopAllCoroutines();
        }

        // リールライトをすべて明るくする
        public void TurnOnAllReels() => ReelEffectManager.ChangeAllReelBrightness(ReelBase.TurnOnValue);

        // リールライトをすべて暗くする
        public void TurnOffAllReels() => ReelEffectManager.ChangeAllReelBrightness(ReelBase.TurnOffValue);

        // JAC GAME時のライト点灯
        public void EnableJacGameLight() => ReelEffectManager.EnableJacGameLight();

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
                for(int i = 0; i < ReelAmount; i++)
                {
                    // 本体と枠下枠上の図柄変更
                    int bodyBright = flashData[(int)FlashData.PropertyID.Body + i * SeekOffset];
                    if (bodyBright != NoChangeValue)
                    {
                        ReelEffectManager.ChangeReelBackLight(i, (byte)bodyBright);
                        ReelEffectManager.ChangeReelSymbolLight(i, (int)ReelPosID.Lower2nd, (byte)bodyBright);
                        ReelEffectManager.ChangeReelSymbolLight(i, (int)ReelPosID.Upper2nd, (byte)bodyBright);
                    }

                    // 図柄の明るさ変更
                    for (int j = (int)ReelPosID.Lower2nd; j < (int)ReelPosID.Upper2nd; j++)
                    {
                        int symbolBright = flashData[(int)FlashData.PropertyID.SymbolLower + j + i * SeekOffset];

                        //Debug.Log("Symbol:" + j + "Bright:" + symbolBright);
                        if (symbolBright != NoChangeValue)
                        {
                            ReelEffectManager.ChangeReelSymbolLight(i, j, (byte)symbolBright);
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
            byte brightness = CalculateBrightness(SymbolLight.TurnOffValue, PayoutFlashFrames);
            //全ての払い出しのあったラインをフラッシュさせる
            foreach (PayoutLineData payoutLine in lastPayoutResult.PayoutLines)
            {
                for (int i = 0; i < payoutLine.PayoutLines.Count; i++)
                {
                    // 図柄点灯
                    ReelEffectManager.ChangeReelSymbolLight(i, payoutLine.PayoutLines[i], brightness);

                    // 左リールにチェリーがある場合はチェリーのみ点灯
                    if (lastStoppedReelData.LastSymbols[(int)ReelID.ReelLeft]
                        [SymbolChange.GetReelArrayIndex(payoutLine.PayoutLines[(int)ReelID.ReelLeft])] == ReelSymbols.Cherry)
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
            int distance = SymbolLight.TurnOnValue - turnOffValue;
            float changeValue = distance / frame;
            // 0.01秒で下げる明るさの量(0.08秒でもとに戻る)
            float result = SymbolLight.TurnOnValue - currentFrame * changeValue;
            // 数値を超えないように調整
            result = Math.Clamp(result, SymbolLight.TurnOffValue, SymbolLight.TurnOnValue);
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