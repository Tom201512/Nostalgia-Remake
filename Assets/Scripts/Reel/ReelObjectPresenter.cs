using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Spin;
using UnityEngine;

namespace ReelSpinGame_Reels
{
    public class ReelObjectPresenter : MonoBehaviour
    {
        // リールオブジェクトプレゼンター

        [SerializeField] ReelDelayTableData reelDatabaseFile;       // リール情報
        [SerializeField] private ReelID reelID;                     // リール識別ID

        // プロパティ
        public ReelID ReelID { get => reelID; }                                         // リールのID
        public ReelStatus ReelStatus { get => reelSpinPresenter.ReelStatus; }           // 現在のリール状態

        public byte[] ReelArray { get => reelSpinPresenter.ReelArray; }                 // リール配列を渡す
        public float RotateSpeed { get => reelSpinPresenter.RotateSpeed; }              // 現在速度を返す
        public float CurrentDegree { get => reelSpinPresenter.CurrentDegree; }          // 現在の角度を返す
        public int CurrentLower { get => reelSpinPresenter.CurrentLower; }              // 現在の下段位置
        public int LastPushedPos { get => reelSpinPresenter.LastPushedPos; }            // 最後に止めた下段位置
        public int WillStopLowerPos => reelSpinPresenter.WillStopLowerPos;              // 停止予定位置
        public int LastStoppedOrder { get => reelSpinPresenter.LastStoppedOrder; }      // 停止したときの押し順
        public int LastDelay { get => reelSpinPresenter.LastStoppedDelay; }             // スベリコマ数

        public bool HasJacModeLight { get; set; }        // JAC中の点灯をするか

        // リール演出用マネージャー
        public ReelEffect ReelEffectManager { get; private set; }

        // リールが停止したかのイベント(個別ごとのリール)
        public delegate void ReelStoppedEvent(ReelID reelID);
        public event ReelStoppedEvent HasReelStopped;

        // リール回転用のプレゼンター
        private ReelSpinPresenter reelSpinPresenter;

        void Awake()
        {
            ReelEffectManager = GetComponent<ReelEffect>();
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
        }

        void Start()
        {
            reelSpinPresenter.ChangeBlurSetting(false);
            reelSpinPresenter.OnReelPositionChanged += OnReelPosChangedCallback;
            reelSpinPresenter.OnReelDegreeChanged += OnReelDegreeChangedCallback;
            reelSpinPresenter.HasReelStopped += OnReelHasStoppedCallback;
        }

        void OnDestroy()
        {
            reelSpinPresenter.OnReelPositionChanged -= OnReelPosChangedCallback;
            reelSpinPresenter.OnReelDegreeChanged -= OnReelDegreeChangedCallback;
            reelSpinPresenter.HasReelStopped -= OnReelHasStoppedCallback;
        }

        // 指定した位置にあるリールの番号を返す
        public int GetReelPos(sbyte posID) => reelSpinPresenter.GetReelPos(reelSpinPresenter.CurrentLower, posID);
        // 指定した位置にあるリールの図柄を返す
        public ReelSymbols GetReelSymbol(sbyte posID) => reelSpinPresenter.GetReelSymbol(reelSpinPresenter.CurrentLower, posID);
        // 停止する位置から指定位置の図柄を返す
        public ReelSymbols GetSymbolFromWillStop(sbyte posID) => reelSpinPresenter.GetReelSymbol(reelSpinPresenter.WillStopLowerPos, posID);
        // 指定した位置の図柄を得る
        public Sprite GetReelSymbolSprite(int reelPos) => reelSpinPresenter.GetReelSymbolSprite(reelPos);
        // リール条件を渡す
        public ReelDelayTableData GetReelDatabase() => reelDatabaseFile;

        // リールの初期化
        public void InitializeReel(int initialLowerPos)
        {
            reelSpinPresenter.CurrentLower = initialLowerPos;
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.CurrentLower);
        }

        // 下段位置の変更
        public void ChangeCurrentLower(int lowerPos)
        {
            reelSpinPresenter.CurrentLower = lowerPos;
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.CurrentLower);
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
            reelSpinPresenter.StopReelImmediately(pushedPos, pushOrder, delay);
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.CurrentLower);
        }

        // リール位置が変わったときのコールバック
        private void OnReelPosChangedCallback()
        {
            // JAC中であれば回転時の明るさ計算をリセット
            if (ReelEffectManager.HasJacBrightnessCalculate)
            {
                ReelEffectManager.ResetJacBrightnessCalculate(reelSpinPresenter.MaxSpeed);
            }
            reelSpinPresenter.UpdateReelSymbols(reelSpinPresenter.CurrentLower);
        }

        // リール角度が変わったときのコールバック
        private void OnReelDegreeChangedCallback()
        {
            if (ReelEffectManager.HasJacBrightnessCalculate)
            {
                ReelEffectManager.CalculateJacBrightness(reelSpinPresenter.MaxSpeed);
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
            HasReelStopped?.Invoke(ReelID);
        }
    }
}
