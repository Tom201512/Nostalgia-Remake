using ReelSpinGame_Reel.Symbol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Reel.Effect
{
    // リールエフェクトマネージャー
    public class ReelEffectManager : MonoBehaviour
    {
        [SerializeField] List<ReelObject> reelObjects;        // リールオブジェクトプレゼンター

        public bool HasFakeSpin { get; private set; }        // 疑似遊技中か

        void Awake()
        {
            HasFakeSpin = false;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // 指定したリールのバックライト変更
        public void ChangeReelBackLight(ReelID reelID, int r, int g, int b) => reelObjects[(int)reelID].ReelEffectManager.ChangeReelBrightness(r,g,b);
        public void ChangeReelBackLight(ReelID reelID, byte brightness) => reelObjects[(int)reelID].ReelEffectManager.ChangeReelBrightness(brightness);

        // 指定したリールと図柄位置のライト変更
        public void ChangeReelSymbolLight(ReelID reelID, ReelPosID posID, int r, int g, int b) => reelObjects[(int)reelID].ReelEffectManager.ChangeSymbolBrightness(posID, r,g,b);

        public void ChangeReelSymbolLight(ReelID reelID, ReelPosID posID, byte brightness) => reelObjects[(int)reelID].ReelEffectManager.ChangeSymbolBrightness(posID, brightness);

        // 全リールの明るさ一括変更
        public void ChangeAllReelBrightness(byte brightness)
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.ReelEffectManager.ChangeReelBrightness(brightness);
                foreach (ReelPosID posID in Enum.GetValues(typeof(ReelPosID)))
                {
                    reel.ReelEffectManager.ChangeSymbolBrightness(posID, brightness);
                }
            }
        }

        // JAC GAME時のライト点灯
        public void EnableJacGameLight()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.ReelEffectManager.ChangeReelBrightness(ReelBase.TurnOffValue);

                // 真ん中以外点灯
                foreach(ReelPosID posID in Enum.GetValues(typeof(ReelPosID)))
                {
                    if (posID == ReelPosID.Center)
                    {
                        reel.ReelEffectManager.ChangeSymbolBrightness(posID, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reel.ReelEffectManager.ChangeSymbolBrightness(posID, SymbolLight.TurnOffValue);
                    }
                }
            }
        }

        // JAC中の明るさ計算の設定
        public void SetJacBrightnessCalculation(bool value)
        {
            foreach (ReelObject reel in reelObjects)
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
            reelObjects[(int)ReelID.ReelRight].StartReel(0.98f, true);

            Debug.Log("中リールを1秒後に回転させます。");
            yield return new WaitForSeconds(1.0f);
            reelObjects[(int)ReelID.ReelMiddle].StartReel(0.98f, true);

            Debug.Log("左リールを1秒後に回転させます。");
            yield return new WaitForSeconds(1.0f);
            reelObjects[(int)ReelID.ReelLeft].StartReel(0.98f, true);

            Debug.Log("3秒後に疑似遊技を終了します");
            yield return new WaitForSeconds(3.0f);
            Debug.Log("疑似遊技を終了しました");
            HasFakeSpin = false;
        }
    }
}

