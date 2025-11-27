using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Spin;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels
{
    public class ReelObjectPresenter : MonoBehaviour
    {
        // リールオブジェクトプレゼンター

        // const
        // リール識別用ID
        public enum ReelID { ReelLeft, ReelMiddle, ReelRight };

        // var
        // リール情報
        [SerializeField] ReelDelayTableData reelDatabaseFile;
        // リール識別ID
        [SerializeField] private ReelID reelID;

        // リールが停止したかのイベント(個別ごとのリール)
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // リール演出用マネージャー
        public ReelEffect ReelEffectManager { get; private set; }

        // リール回転用のプレゼンター
        private ReelSpinPresenter reelSpinPresenter;

        // JAC中の点灯をするか
        public bool HasJacModeLight { get; set; }

        private void Awake()
        {
            ReelEffectManager = GetComponent<ReelEffect>();
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
        }

        private void Start()
        {
            reelSpinPresenter.ChangeBlurSetting(false);
            reelSpinPresenter.OnReelPositionChanged += OnReelPosChangedCallback;
            reelSpinPresenter.OnReelDegreeChanged += OnReelDegreeChangedCallback;
            reelSpinPresenter.HasReelStopped += OnReelHasStoppedCallback;
        }

        private void OnDestroy()
        {
            reelSpinPresenter.OnReelPositionChanged -= OnReelPosChangedCallback;
            reelSpinPresenter.OnReelDegreeChanged -= OnReelDegreeChangedCallback;
            reelSpinPresenter.HasReelStopped -= OnReelHasStoppedCallback;
        }

        // func

        // 数値を得る
        // リールのID
        public ReelID GetReelID() => reelID;

        // 現在のリール状態
        public ReelStatus GetCurrentReelStatus() => reelSpinPresenter.GetCurrentReelStatus();
        // 現在の下段位置
        public int GetCurrentLower() => reelSpinPresenter.GetCurrentLower();
        // 最後に止めた下段位置
        public int GetLastPushedLowerPos() => reelSpinPresenter.GetLastPushedPos();
        // 停止予定位置
        public int GetWillStopLowerPos() => reelSpinPresenter.GetWillStopLowerPos();
        // 停止したときの押し順を返す
        public int GetLastStoppedOrder() => reelSpinPresenter.GetLastStoppedOrder();
        // 最後に止めたときのディレイ数
        public int GetLastDelay() => reelSpinPresenter.GetLastStoppedDelay();

        // 指定した位置にあるリールの番号を返す
        public int GetReelPos(sbyte posID) => reelSpinPresenter.GetReelPos(reelSpinPresenter.GetCurrentLower(), posID);
        // 指定した位置にあるリールの図柄を返す
        public ReelSymbols GetReelSymbol(sbyte posID) => reelSpinPresenter.GetReelSymbol(reelSpinPresenter.GetCurrentLower(), posID);
        // 停止する位置から指定位置の図柄を返す
        public ReelSymbols GetSymbolFromWillStop(sbyte posID) => reelSpinPresenter.GetReelSymbol(reelSpinPresenter.GetWillStopLowerPos(), posID);

        // 指定した位置の図柄を得る
        public Sprite GetReelSymbolSprite(int reelPos) => reelSpinPresenter.GetReelSymbolSprite(reelPos);

        // 現在速度を返す
        public float GetCurrentSpeed() => reelSpinPresenter.GetCurrentSpeed();
        // 現在の角度を返す
        public float GetCurrentDegree() => reelSpinPresenter.GetCurrentDegree();

        // リール条件を渡す
        public ReelDelayTableData GetReelDatabase() => reelDatabaseFile;
        // リール配列を渡す
        public byte[] GetReelArrayData() => reelSpinPresenter.GetReelArray();

        // リールの初期化
        public void InitializeReel(int initialLowerPos)
        {
            reelSpinPresenter.SetCurrentLower(initialLowerPos);
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // 下段位置の変更
        public void ChangeCurrentLower(int lowerPos)
        {
            reelSpinPresenter.SetCurrentLower(lowerPos);
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // JAC中の明るさ計算の設定
        public void SetJacBrightnessCalculate(bool value) => ReelEffectManager.SetJacBrightnessCalculate(value);

        //　リール始動
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
            reelSpinPresenter.ChangeBlurSetting(true);
        }

        // リール停止
        public void StopReel(int pushedPos, int pushOrder, int delay)
        {
            reelSpinPresenter.StartStopReelSpin(pushedPos, pushOrder, delay);
        }

        // リール停止(高速版)
        public void StopReelFast(int pushedPos, int pushOrder, int delay)
        {
            reelSpinPresenter.StopReelImmediately(pushedPos, pushOrder,delay);
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // リール位置が変わったときのコールバック
        private void OnReelPosChangedCallback()
        {
            // JAC中であれば回転時の明るさ計算をリセット
            if(ReelEffectManager.HasJacBrightnessCalculate)
            {
                ReelEffectManager.ResetJacBrightnessCalculate(reelSpinPresenter.GetMaxSpeed());
            }
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.GetCurrentLower());
        }

        // リール角度が変わったときのコールバック
        private void OnReelDegreeChangedCallback()
        {
            if (ReelEffectManager.HasJacBrightnessCalculate)
            {
                Debug.Log("Calculating");
                ReelEffectManager.CalculateJacBrightness(reelSpinPresenter.GetMaxSpeed());
            }
        }

        // リールが停止したときのコールバック
        private void OnReelHasStoppedCallback()
        {
            //Debug.Log("StoppedEvent called");
            // JAC中ならライトも調整
            if (ReelEffectManager.HasJacBrightnessCalculate)
            {
                ReelEffectManager.FinishJacBrightnessCalculate();
            }
            HasReelStopped?.Invoke();
        }

        // リール位置をオーバーフローしない数値で返す
        public static int OffsetReelPos(int reelPos, int offset)
        {
            if (reelPos + offset < 0)
            {
                return MaxReelArray + reelPos + offset;
            }

            else if (reelPos + offset > MaxReelArray - 1)
            {
                return reelPos + offset - MaxReelArray;
            }
            // オーバーフローがないならそのまま返す
            return reelPos + offset;
        }
    }
}
