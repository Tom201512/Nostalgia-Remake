using ReelSpinGame_Reels;
using System.Collections;
using UnityEngine;

public class FlashManager : MonoBehaviour
{
    // フラッシュ機能

    // const
    // リールフラッシュの間隔(秒間隔)
    const float ReelFlashTime = 0.1f;
    // デフォルトの明るさ(点灯時)
    const int TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    const int TurnOffValue = 80;

    // var
    // 現在のフレーム数(1フレーム0.1秒)
    public int CurrentFrame { get; private set; }

    // リールオブジェクト
    public ReelObject[] ReelObjects { get; private set; }

    // func
    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // フラッシュ再生
    public void StartFlash()
    {
        CurrentFrame = 0;
        StartCoroutine("FlashUpdate");
        Debug.Log("Flash started");
    }

    // フラッシュ停止
    public void StopFlash()
    {
        StopCoroutine("FlashUpdate");
        Debug.Log("Flash stopped");
        TurnOnAllReels();
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

    // フラッシュの更新処理
    public IEnumerator FlashUpdate()
    {
        Debug.Log("Coroutine called");
        while (true)
        {
            // テスト用
            if (CurrentFrame < 5)
            {
                Debug.Log("FlashA");
                TurnOffAllReels();
            }
            else
            {
                Debug.Log("FlashB");
                TurnOnAllReels();
            }

            CurrentFrame += 1;

            if (CurrentFrame == 9)
            {
                CurrentFrame = 0;
            }
            Debug.Log("Flash:" + CurrentFrame);

            yield return new WaitForSeconds(ReelFlashTime);
        }
    }
}
