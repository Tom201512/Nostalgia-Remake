using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class FlashManager : MonoBehaviour
{
    // フラッシュ機能

    // const
    // リールフラッシュの間隔(秒間隔)
    const float ReelFlashTime = 0.01f;
    // 払い出し時のフラッシュに要するフレーム数(0.01秒間隔)
    const int PayoutFlashFrames = 15;
    // デフォルトの明るさ(点灯時)
    const int TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    const int TurnOffValue = 180;
    // 図柄を変更する際のループ回数
    const int SymbolLoops = 3;
    // シーク位置オフセット用
    const int SeekOffset = 4;
    // 変更しないときの数値
    const int NoChangeValue = -1;

    // 払い出しラインのID
    public enum PayoutLineID {PayoutMiddle, PayoutLower, PayoutUpper, PayoutDiagonalA, PayoutDiagonalB};

    // var
    // 現在のフレーム数(1フレーム0.1秒)
    public int CurrentFrame { get; private set; }
    // フラッシュ中か
    public bool HasFlash { get; private set; }
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
        CurrentFlashID = 0;
        FlashDatabase = new List<FlashData>();

        foreach(TextAsset textAsset in testAssetList)
        {
            StringReader buffer = new StringReader(textAsset.text);
            FlashDatabase.Add(new FlashData(buffer));
        }

        Debug.Log("FlashManager awaken");
    }

    public void Start()
    {
        //StartFlash(0);
    }

    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // フラッシュ再生
    public void StartFlash(int flashID)
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("FlashUpdate");
        Debug.Log("Flash started");
    }

    public void StartPayoutFlash(List<PayoutLineData> lastPayoutLines)
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("PayoutFlashUpdate",lastPayoutLines);
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
            reel.SetReelBaseBrightness(TurnOffValue);
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                Debug.Log("PosID:" + i);
                reel.SetSymbolBrightness(i, TurnOffValue, TurnOffValue, TurnOffValue);
            }
        }
        Debug.Log("All reels are turned off");
    }

    // JAC GAME時のライト
    public void EnableJacGameLight()
    {
        foreach (ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOffValue);

            // 真ん中以外点灯
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                if (i == GetReelArrayIndex((int)ReelPosID.Center))
                {
                    reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
                }
                else
                {
                    reel.SetSymbolBrightness(i, TurnOffValue, TurnOffValue, TurnOffValue);
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
            throw new System.Exception("FlashID is Overflow the flashDatabase");
        }

        int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();
        Debug.Log("Seek:"+ FlashDatabase[CurrentFlashID].CurrentSeekPos);

        // 現在のフレームと一致しなければ読み込まない
        Debug.Log("Segment:" + flashData[(int)FlashData.PropertyID.FrameID]);
        if (CurrentFrame == flashData[(int)FlashData.PropertyID.FrameID])
        {
            // リール全て変更
            foreach (ReelObject reel in ReelObjects)
            {
                // 本体変更
                int bodyBright = flashData[(int)FlashData.PropertyID.Body + reel.ReelData.ReelID * SeekOffset];
                Debug.Log("Change Body:" + reel.ReelData.ReelID + "Bright:" + bodyBright);
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
                for (int i = 0; i < SymbolLoops; i++)
                {
                    int symbolBright = flashData[(int)FlashData.PropertyID.SymbolLower + i + reel.ReelData.ReelID * SeekOffset];

                    Debug.Log("Symbol:" + ((int)FlashData.PropertyID.SymbolLower + i) + "Bright:" + symbolBright);
                    if(symbolBright != NoChangeValue)
                    {
                        reel.SetSymbolBrightness((int)FlashData.PropertyID.SymbolLower + i, (byte)symbolBright, (byte)symbolBright, (byte)symbolBright);
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
                FlashDatabase[CurrentFlashID].ResetSeek();
                Debug.Log("LoopFlash");
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
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = distance / PayoutFlashFrames;
        // 0.01秒で下げる明るさの量(0.08秒でもとに戻る)
        float result = TurnOnValue - CurrentFrame * changeValue;
        // 数値を超えないように調整
        result = Math.Clamp(result, TurnOffValue, TurnOnValue);
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
                if (ReelObjects[(int)ReelManager.ReelID.ReelLeft].GetReelSymbol
                    (payoutLine.PayoutLines[(int)ReelManager.ReelID.ReelLeft]) == ReelSymbols.Cherry)
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
}
