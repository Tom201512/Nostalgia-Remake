using ReelSpinGame_Reels.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_Reels.Effect
{
    // リールエフェクトマネージャー
    public class ReelEffectManager : MonoBehaviour
    {
        // var
        // リールオブジェクトプレゼンター
        public List<ReelObjectPresenter> ReelObjects { get; private set; }

        // 疑似遊技中か
        public bool HasFakeSpin { get; private set; }

        private void Awake()
        {
            HasFakeSpin = false;
        }

        // リールオブジェクトをセットする
        public void SetReels(List<ReelObjectPresenter> reels)
        {
            ReelObjects = reels;
        }

        // 指定したリールのバックライト変更
        public void ChangeReelBackLight(int reelID, byte brightness) => ReelObjects[reelID].ReelEffectManager.ChangeReelBrightness(brightness);

        // 指定したリールと図柄位置のライト変更
        public void ChangeReelSymbolLight(int reelID, int posID, byte brightness) => ReelObjects[reelID].ReelEffectManager.ChangeSymbolBrightness(posID, brightness);

        // 全リールの明るさ一括変更
        public void ChangeAllReelBrightness(byte brightness)
        {
            foreach (ReelObjectPresenter reel in ReelObjects)
            {
                reel.ReelEffectManager.ChangeReelBrightness(brightness);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.ReelEffectManager.ChangeSymbolBrightness(i, brightness);
                }
            }
        }

        // JAC GAME時のライト点灯
        public void EnableJacGameLight()
        {
            foreach (ReelObjectPresenter reel in ReelObjects)
            {
                reel.ReelEffectManager.ChangeReelBrightness(ReelBase.TurnOffValue);

                // 真ん中以外点灯
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    if (i == (int)ReelPosID.Center)
                    {
                        reel.ReelEffectManager.ChangeSymbolBrightness(i, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reel.ReelEffectManager.ChangeSymbolBrightness(i, SymbolLight.TurnOffValue);
                    }
                }
            }
        }

        // JAC中の明るさ計算の設定
        public void SetJacBrightnessCalculation(bool value)
        {
            foreach (ReelObjectPresenter reel in ReelObjects)
            {
                reel.SetJacBrightnessCalculate(value);
            }
        }

        // 疑似遊技
        public void StartFakeSpin()
        {
            HasFakeSpin = true;
            StartCoroutine(nameof(FakeSpinTestA));
        }

        // 疑似遊技テスト
        private IEnumerator FakeSpinTestA()
        {
            Debug.Log("疑似遊技を開始しました。");
            
            Debug.Log("右リールを1秒後に回転させます。");
            yield return new WaitForSeconds(1.0f);
            ReelObjects[(int)ReelID.ReelRight].StartReel(0.98f, true);

            Debug.Log("中リールを1秒後に回転させます。");
            yield return new WaitForSeconds(1.0f);
            ReelObjects[(int)ReelID.ReelMiddle].StartReel(0.98f, true);

            Debug.Log("左リールを1秒後に回転させます。");
            yield return new WaitForSeconds(1.0f);
            ReelObjects[(int)ReelID.ReelLeft].StartReel(0.98f, true);

            Debug.Log("3秒後に疑似遊技を終了します");
            yield return new WaitForSeconds(3.0f);
            Debug.Log("疑似遊技を終了しました");
            HasFakeSpin = false;
        }
    }
}

