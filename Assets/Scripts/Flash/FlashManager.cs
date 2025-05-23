using ReelSpinGame_Flash;
using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlashManager : MonoBehaviour
{
    // フラッシュ機能

    // const
    // リールフラッシュの間隔(秒間隔)
    const float ReelFlashTime = 0.01f;
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

    // var
    // 現在のフレーム数(1フレーム0.1秒)
    public int CurrentFrame { get; private set; }
    // フラッシュ中か
    public bool HasFlash { get; private set; }

    // リールオブジェクト
    public ReelObject[] ReelObjects { get; private set; }
    // フラッシュデータ
    public List<FlashData> FlashDatabase { get; private set; }
    [SerializeField] private TextAsset testAsset;

    // func
    public void Awake()
    {
        HasFlash = false;
        FlashDatabase = new List<FlashData>();
        StringReader buffer = new StringReader(testAsset.text);
        FlashDatabase.Add(new FlashData(buffer));
        Debug.Log("FlashManager awaken");
    }

    public void Start()
    {
        StartFlash();
    }

    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // フラッシュ再生
    public void StartFlash()
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("FlashUpdate");
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
            for (int i = 0; i < (int)ReelData.ReelPosArrayID.Upper3rd; i++)
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
            for (int i = 0; i < (int)ReelData.ReelPosArrayID.Upper3rd; i++)
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
            for (int i = 0; i < (int)ReelData.ReelPosArrayID.Upper3rd; i++)
            {
                if(i == (int)ReelData.ReelPosArrayID.Center)
                {
                    reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
                }
                else
                {
                    reel.SetSymbolBrightness(i, TurnOffValue, TurnOffValue, TurnOffValue);
                }
            }
        }
        Debug.Log("All reels are turned on");
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

    // フラッシュデータの処理を反映する
    public void ReadFlashData()
    {
        int[] flashData = FlashDatabase[0].GetCurrentFlashData();
        Debug.Log("Seek:"+ FlashDatabase[0].CurrentSeekPos);

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
            if (!FlashDatabase[0].HasSeekReachedEnd())
            {
                CurrentFrame += 1;
                FlashDatabase[0].MoveNextSeek();
                Debug.Log("SeekMoved");
                Debug.Log("NextFrame");
            }

            // ループさせるか(ループの場合は特定フレームまで移動させる)
            // しない場合は停止する。
            if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
            {
                CurrentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
                FlashDatabase[0].ResetSeek();
                Debug.Log("LoopFlash");
            }
        }
        else
        {
            CurrentFrame += 1;
            Debug.Log("NextFrame");
        }
    }
}
