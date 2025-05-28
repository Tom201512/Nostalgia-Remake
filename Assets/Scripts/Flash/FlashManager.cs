using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

public class FlashManager : MonoBehaviour
{
    // フラッシュ機能

    // const
    // リールフラッシュの間隔(秒間隔)
    const float ReelFlashTime = 0.01f;
    // 払い出し時のフラッシュに要するフレーム数(0.01秒間隔)
    const int PayoutFlashFrames = 15;
    // リプレイ時に待機する時間(秒)
    const int ReplayWaitTime = 1;
    // デフォルトの明るさ(点灯時)
    const int TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    const int TurnOffSymbolValue = 120;
    // デフォルトの暗さ(リール本体消灯時)
    const int TurnOffBodyValue = 80;
    // シーク位置オフセット用
    const int SeekOffset = 4;
    // 変更しないときの数値
    const int NoChangeValue = -1;

    // デフォルトのフラッシュID
    public enum FlashID { V_Flash};
    // 払い出しラインのID
    public enum PayoutLineID {PayoutMiddle, PayoutLower, PayoutUpper, PayoutDiagonalA, PayoutDiagonalB};

    // var
    // 現在のフレーム数(1フレーム0.1秒)
    public int CurrentFrame { get; private set; }
    // フラッシュ中か
    public bool HasFlash { get; private set; }
    // リプレイフラッシュで待機中か
    public bool HasReplayWait { get; private set; }
    // 現在のフラッシュID
    public int CurrentFlashID { get; private set; }

    // リールオブジェクト
    public ReelObject[] ReelObjects { get; private set; }
    // フラッシュデータ
    public List<FlashData> FlashDatabase { get; private set; }
    [SerializeField] private List<TextAsset> testAssetList;

    // func
    public void Awake()
    {
        HasFlash = false;
        HasReplayWait = false;
        CurrentFlashID = 0;
        FlashDatabase = new List<FlashData>();

        foreach(TextAsset textAsset in testAssetList)
        {
            StringReader buffer = new StringReader(textAsset.text);
            FlashDatabase.Add(new FlashData(buffer));
        }

        Debug.Log("FlashManager awaken");
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        Debug.Log("Coroutines are stopped");
    }

    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // フラッシュ再生
    public void StartFlash(int flashID)
    {
        CurrentFrame = 0;
        HasFlash = true;
        FlashDatabase[flashID].SetSeek(0);
        StartCoroutine("FlashUpdate");
        Debug.Log("Flash started");
    }

    // 払い出しフラッシュ開始
    public void StartPayoutFlash(List<PayoutLineData> lastPayoutLines, bool waitForReplay)
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("PayoutFlashUpdate",lastPayoutLines);

        // リプレイタイマーをつける設定をした場合
        if(waitForReplay)
        {
            StartCoroutine("EnableReplayTimer");
        }
        Debug.Log("Flash started");
    }

    // フラッシュ停止
    public void StopFlash()
    {
        HasFlash = false;
    }

    // リールライトをすべて明るくする
    public void TurnOnAllReels()
    {
        foreach(ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOnValue);
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
            }
        }
        Debug.Log("All reels are turned on");
    }

    // リールライトをすべて暗くする
    public void TurnOffAllReels()
    {
        foreach (ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOffBodyValue);
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                Debug.Log("PosID:" + i);
                reel.SetSymbolBrightness(i, TurnOffSymbolValue, TurnOffSymbolValue, TurnOffSymbolValue);
            }
        }
        Debug.Log("All reels are turned off");
    }

    // JAC GAME時のライト
    public void EnableJacGameLight()
    {
        foreach (ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOffSymbolValue);

            // 真ん中以外点灯
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                if (i == (int)ReelPosID.Center)
                {
                    reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
                }
                else
                {
                    reel.SetSymbolBrightness(i, TurnOffSymbolValue, TurnOffSymbolValue, TurnOffSymbolValue);
                }
            }
        }
        Debug.Log("turned on JACGAME lights");
    }

    // フラッシュの更新処理
    public IEnumerator FlashUpdate()
    {
        Debug.Log("Coroutine called");
        while (HasFlash)
        {
            ReadFlashData();
            Debug.Log("Flash:" + CurrentFrame);
            yield return new WaitForSeconds(ReelFlashTime);
        }

        Debug.Log("Flash Stopped by Insert");
        yield break;
    }

    public IEnumerator PayoutFlashUpdate(List<PayoutLineData> lastPayoutLines)
    {
        Debug.Log("Payout Coroutine called");
        while (HasFlash)
        {
            PayoutFlash(lastPayoutLines);
            Debug.Log("Flash:" + CurrentFrame);
            yield return new WaitForSeconds(ReelFlashTime);
        }

        Debug.Log("Flash Stopped by Insert");
        yield break;
    }

    // フラッシュデータの処理を反映する
    public void ReadFlashData()
    {
        if(CurrentFlashID >= FlashDatabase.Count)
        {
            throw new Exception("FlashID is Overflow the flashDatabase");
        }

        int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();

        // 現在のフレームと一致しなければ読み込まない
        Debug.Log("Segment:" + flashData[(int)FlashData.PropertyID.FrameID]);
        if (CurrentFrame == flashData[(int)FlashData.PropertyID.FrameID])
        {
            // リール全て変更
            foreach (ReelObject reel in ReelObjects)
            {
                // 本体変更
                int bodyBright = flashData[(int)FlashData.PropertyID.Body + reel.GetReelID() * SeekOffset];
                Debug.Log("Change Body:" + reel.GetReelID() + "Bright:" + bodyBright);
                if (bodyBright != NoChangeValue)
                {
                    reel.SetReelBaseBrightness((byte)bodyBright);
                }
                else
                {
                    Debug.Log("No changes");
                }

                Debug.Log("Change Symbols");
                // 図柄の明るさ変更
                for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
                {
                    int symbolBright = flashData[(int)FlashData.PropertyID.SymbolLower + i + reel.GetReelID() * SeekOffset];

                    Debug.Log("Symbol:" + i + "Bright:" + symbolBright);
                    if(symbolBright != NoChangeValue)
                    {
                        reel.SetSymbolBrightness(i, (byte)symbolBright, (byte)symbolBright, (byte)symbolBright);
                    }
                    else
                    {
                        Debug.Log("No changes");
                    }
                }
            }

            // データのシーク位置変更
            if (!FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
            {
                CurrentFrame += 1;
                FlashDatabase[CurrentFlashID].MoveNextSeek();
                Debug.Log("SeekMoved");
                Debug.Log("NextFrame");
            }
            // ループさせるか(ループの場合は特定フレームまで移動させる)
            // しない場合は停止する。
            if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
            {
                CurrentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
                FlashDatabase[CurrentFlashID].SetSeek(flashData[(int)FlashData.PropertyID.LoopPosition]);
                Debug.Log("LoopFlash");
            }
            // 最終行までよんでループがない場合は終了
            else if(FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
            {
                Debug.Log("Finish Flash");
                HasFlash = false;
            }
        }
        else
        {
            CurrentFrame += 1;
            Debug.Log("NextFrame");
        }
    }

    // 払い出し時のフラッシュ
    public void PayoutFlash(List<PayoutLineData> lastPayoutLines)
    {
        // 明るさの計算(0.01秒で25下げる)
        int distance = TurnOnValue - TurnOffSymbolValue;
        float changeValue = distance / PayoutFlashFrames;
        // 0.01秒で下げる明るさの量(0.08秒でもとに戻る)
        float result = TurnOnValue - CurrentFrame * changeValue;
        // 数値を超えないように調整
        result = Math.Clamp(result, TurnOffSymbolValue, TurnOnValue);
        // byte型に変換
        byte brightness = (byte)Math.Round(result);

        //全ての払い出しのあったラインをフラッシュさせる
        foreach (PayoutLineData payoutLine in lastPayoutLines)
        {
            for(int i = 0; i < payoutLine.PayoutLines.Count; i++)
            {
                // 図柄点灯
                ReelObjects[i].SetSymbolBrightness(payoutLine.PayoutLines[i],brightness, brightness, brightness);

                // 左リールにチェリーがある場合はチェリーのみ点灯
                if (ReelObjects[(int)ReelID.ReelLeft].GetReelSymbol
                    (payoutLine.PayoutLines[(int)ReelID.ReelLeft]) == ReelSymbols.Cherry)
                {
                    break;
                }
            }
        }

        // ループさせる
        CurrentFrame += 1;

        if(CurrentFrame == PayoutFlashFrames)
        {
            CurrentFrame = 0;
        }
    }

    // リプレイ時のフラッシュ切り
    private IEnumerator EnableReplayTimer()
    {
        HasReplayWait = true;
        Debug.Log("Replay Timer Start");
        yield return new WaitForSeconds(ReplayWaitTime);
        Debug.Log("Replay Finished");
        HasReplayWait = false;
        HasFlash = false;

        TurnOnAllReels();
    }
}
