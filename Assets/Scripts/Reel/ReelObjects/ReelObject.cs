using ReelSpinGame_Reel.Spin;
using ReelSpinGame_Reel.Table;
using System;
using UnityEngine;

namespace ReelSpinGame_Reel
{
    // リールオブジェクトプレゼンター
    public class ReelObject : MonoBehaviour
    {
        [SerializeField] private ReelSpinPresenter reelSpinPresenter;       // リール回転
        [SerializeField] private ReelEffect reelEffect;                     // リール演出部
        [SerializeField] private ReelDelayTableFile reelDelayTableFile;     // リール情報
        [SerializeField] private ReelID reelID;                             // リール識別ID

        // プロパティ
        public ReelID ReelID { get => reelID; }                                         // リールのID
        public ReelStatus ReelStatus { get => reelSpinPresenter.ReelStatus; }           // 現在のリール状態

        public int[] ReelArray { get => reelSpinPresenter.ReelArray; }                  // リール配列を渡す
        public float RotateSpeed { get => reelSpinPresenter.RotateSpeed; }              // 現在速度を返す
        public float CurrentDegree { get => reelSpinPresenter.CurrentDegree; }          // 現在の角度を返す
        public int CurrentLower { get => reelSpinPresenter.CurrentLower; }              // 現在の下段位置
        public int LastPushedPos { get => reelSpinPresenter.LastPushedPos; }            // 最後に止めた下段位置
        public int WillStopLowerPos => reelSpinPresenter.WillStopLowerPos;              // 停止予定位置
        public int LastStoppedOrder { get => reelSpinPresenter.LastStoppedOrder; }      // 停止したときの押し順
        public int LastDelay { get => reelSpinPresenter.LastStoppedDelay; }             // スベリコマ数

        // ブラーの設定
        public bool HasBlur
        {
            get => reelSpinPresenter.HasBlur;
            set => reelSpinPresenter.HasBlur = value;
        }

        public ReelDelayTableFile ReelDelayTableFile { get => reelDelayTableFile; }     // スベリコマ制御ファイル
        public ReelEffect ReelEffectManager { get => reelEffect; }                      // リール演出部
        public bool HasJacModeLight { get; set; }                                       // JAC中の点灯をするか

        public event Action<ReelID> ReelStopped;        // リールが停止したときのイベント
        public event Action<ReelID> ReelPosChanged;     // リール位置が変更された時のイベント

        private void Start()
        {
            reelSpinPresenter.ReelPositionChanged += OnReelPosChanged;
            reelSpinPresenter.ReelDegreeChanged += OnReelDegreeChanged;
            reelSpinPresenter.HasReelStopped += OnReelStopped;
            HasBlur = false;
        }

        private void OnDestroy()
        {
            reelSpinPresenter.ReelPositionChanged -= OnReelPosChanged;
            reelSpinPresenter.ReelDegreeChanged -= OnReelDegreeChanged;
            reelSpinPresenter.HasReelStopped -= OnReelStopped;
        }

        // 指定した位置にあるリールの番号を返す
        public int GetReelPos(ReelPosID posID) => reelSpinPresenter.GetReelPos(reelSpinPresenter.CurrentLower, posID);
        // 指定した位置にあるリールの図柄を返す
        public ReelSymbols GetReelSymbol(ReelPosID posID) => reelSpinPresenter.GetReelSymbol(reelSpinPresenter.CurrentLower, posID);
        // 停止する位置から指定位置の図柄を返す
        public ReelSymbols GetSymbolFromWillStop(ReelPosID posID) => reelSpinPresenter.GetReelSymbol(reelSpinPresenter.WillStopLowerPos, posID);
        // 指定した位置の図柄を得る
        public Sprite GetReelSymbolSprite(int reelPos) => reelSpinPresenter.GetReelSymbolSprite(reelPos);

        // 下段位置の変更
        public void ChangeCurrentLower(int lowerPos) => reelSpinPresenter.ChangeLowerPos(lowerPos);
        // マーカー位置指定
        public void SetMarker(int markerPos) => reelSpinPresenter.ChangeMarkerPos(markerPos);
        // JAC中の明るさ計算の設定
        public void SetJacBrightnessCalculate(bool value) => reelEffect.SetJacBrightnessCalculate(value);

        // リール始動
        public void StartReel(float maxSpeed, bool cutAccelerate) => reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
        // リール停止
        public void StopReel(int pushedPos, int pushOrder, int delay) => reelSpinPresenter.StartStopReelSpin(pushedPos, pushOrder, delay);
        // リール停止(高速版)
        public void StopReelFast(int pushedPos, int pushOrder, int delay) => reelSpinPresenter.StopReelImmediately(pushedPos, pushOrder, delay);

        // リール位置が変わったときの処理
        void OnReelPosChanged(int reelPos)
        {
            // JAC中であれば回転時の明るさ計算をリセット
            if (reelEffect.HasJacBrightnessCalculate)
            {
                reelEffect.ResetJacBrightnessCalculate(reelSpinPresenter.MaxSpeed);
            }
        }

        // リール角度が変わったときの処理
        void OnReelDegreeChanged()
        {
            if (reelEffect.HasJacBrightnessCalculate)
            {
                reelEffect.CalculateJacBrightness(reelSpinPresenter.MaxSpeed);
            }
        }

        // リールが停止したときの処理
        void OnReelStopped()
        {
            // JAC中ならライトも調整
            if (reelEffect.HasJacBrightnessCalculate)
            {
                reelEffect.FinishJacBrightnessCalculate();
            }
            ReelStopped?.Invoke(ReelID);
        }
    }
}