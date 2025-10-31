using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Array;
using ReelSpinGame_Reels.Spin;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

namespace ReelSpinGame_Reels
{
    public class ReelObjectPresenter : MonoBehaviour
    {
        // リールオブジェクトプレゼンター

        // var
        // 1分間の回転数 (Rotate Per Minute)
        [Range(0f, 80f), SerializeField] private float rotateRPM;

        // リール情報
        [SerializeField] ReelDatabase reelDatabaseFile;

        // リールが停止したかのイベント(個別ごとのリール)
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // リール演出用マネージャー
        public ReelEffect ReelEffectManager { get; private set; }

        // リールオブジェクトのモデル
        private ReelObjectModel reelObjectModel;
        // リール回転用のプレゼンター
        private ReelSpinPresenter reelSpinPresenter;
        // リール配列用のプレゼンター
        private ReelArrayPresenter reelArrayPresenter;

        // JAC中の点灯をするか
        public bool HasJacModeLight { get; set; }

        private void Awake()
        {
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
            ReelEffectManager = GetComponent<ReelEffect>();
            reelArrayPresenter = GetComponent<ReelArrayPresenter>();

            reelSpinPresenter.SetReelSpinPresenter(rotateRPM);
            reelArrayPresenter.SetReelArrayPresenter(reelDatabaseFile.Array);
        }

        private void Start()
        {
            reelSpinPresenter.ChangeBlurSetting(false);
            reelSpinPresenter.OnReelPositionChanged += OnReelPosChangedCallback;
            reelSpinPresenter.HasReelStopped += OnReelHasStoppedCallback;
            reelArrayPresenter.UpdateReelSymbols(reelObjectModel.CurrentLower);
        }

        private void OnDestroy()
        {
            reelSpinPresenter.OnReelPositionChanged -= OnReelPosChangedCallback;
            reelSpinPresenter.HasReelStopped -= OnReelHasStoppedCallback;
        }

        // func

        // 数値を得る
        // リールのID
        public int GetReelID() => reelObjectModel.ReelID;
        // 現在のリール状態
        public ReelStatus GetCurrentReelStatus() => reelSpinPresenter.GetCurrentReelStatus();
        // 最後に止めた下段位置
        public int GetLastPushedLowerPos() => reelObjectModel.LastPushedLowerPos;
        // 停止予定位置
        public int GetWillStopLowerPos() => reelObjectModel.WillStopLowerPos;
        // 最後に止めたときのディレイ数
        public int GetLastDelay() => reelSpinPresenter.GetLastStoppedDelay();
        // 指定した位置にあるリールの番号を返す
        public int GetReelPos(ReelPosID posID) => reelArrayPresenter.GetReelPos(reelObjectModel.CurrentLower, (sbyte)posID);

        // 現在速度を返す
        public float GetCurrentSpeed() => reelSpinPresenter.GetCurrentSpeed();
        // 現在の角度を返す
        public float GetCurrentDegree() => reelSpinPresenter.GetCurrentDegree();

        // リール条件を渡す
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // リールデータを渡す
        public void SetReelData(int reelID, int initialLowerPos) => reelObjectModel = new ReelObjectModel(reelID, initialLowerPos);

        // func

        // 下段の位置が停止予定位置になったかを返す
        public bool HasReachedStopPos() => reelObjectModel.CurrentLower == reelObjectModel.WillStopLowerPos;

        // 指定したリール位置にする
        public void SetReelPos(int lowerPos) => reelObjectModel.CurrentLower = lowerPos;
        // 最後に押した位置を設定する
        public void SetLastPushedLowerPos(int lastPushedLowerPos) => reelObjectModel.LastPushedLowerPos = lastPushedLowerPos;
        // 停止予定位置を設定する
        public void SetWillStopLowerPos(int willStoplowerPos) => reelObjectModel.WillStopLowerPos = willStoplowerPos;
        // 停止位置関連の数値をリセット
        public void ResetStopValues()
        {
            reelObjectModel.LastPushedLowerPos = 0;
            reelObjectModel.WillStopLowerPos = 0;
        }

        //　リール始動
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
            reelSpinPresenter.ChangeBlurSetting(true);
        }

        // リール停止
        public void StopReel(int pushedMiddlePos, int delay)
        {
            Debug.Log("Pushed at: " + pushedMiddlePos);
            Debug.Log("Delay:" + delay);
            reelObjectModel.LastPushedLowerPos = pushedMiddlePos;
            reelObjectModel.WillStopLowerPos = OffsetReelPos(pushedMiddlePos, delay);
            reelObjectModel.LastStoppedDelay = delay;
            reelSpinPresenter.StartStopReelSpin(delay);
        }

        // リール停止(高速版)
        public void StopReelFast(int pushedMiddlePos, int delay)
        {
            // 強制停止
            reelObjectModel.LastPushedLowerPos = pushedMiddlePos;
            reelObjectModel.WillStopLowerPos = OffsetReelPos(pushedMiddlePos, delay);
            reelObjectModel.LastStoppedDelay = delay;
            reelObjectModel.CurrentLower = reelObjectModel.WillStopLowerPos;

            reelSpinPresenter.FinishReelSpin();
            reelArrayPresenter.UpdateReelSymbols(reelObjectModel.CurrentLower);

            // JAC中ならライトも調整
            if (HasJacModeLight)
            {
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center,SymbolLight.TurnOnValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
            }

            //ReelSpinPresenter.StopReelSpeed();
        }

        // リール位置変更 (回転速度の符号に合わせて変更)
        public void ChangeReelPos(float rotateSpeed)
        {
            // 逆回転の場合
            if (Math.Sign(rotateSpeed) == -1)
            {
                if (reelObjectModel.CurrentLower - 1 < 0)
                {
                    reelObjectModel.CurrentLower = MaxReelArray - 1;
                }
                else
                {
                    reelObjectModel.CurrentLower -= 1;
                }
            }
            // 前回転の場合
            else if (Math.Sign(rotateSpeed) == 1)
            {
                if (reelObjectModel.CurrentLower + 1 > MaxReelArray)
                {
                    reelObjectModel.CurrentLower = 0;
                }
                else
                {
                    reelObjectModel.CurrentLower += 1;
                }
            }
        }

        /*
        // JAC時の明るさ計算(
        private byte CalculateJACBrightness(bool isNegative)
        {
            float brightnessTest = 0;
            float currentDistance = 0;

            // 少し早めに光らせるため距離は短くする
            float distanceRotation = ChangeAngle * JacLightOffset;

            // 符号に合わせて距離を計算
            //Debug.Log("Current Euler:" + transform.rotation.eulerAngles.x);
            if (transform.rotation.eulerAngles.x > 0f)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    currentDistance = transform.rotation.eulerAngles.x;
                }
                else
                {
                    currentDistance = 360.0f - transform.rotation.eulerAngles.x;
                }
            }
            brightnessTest = Math.Clamp(currentDistance / distanceRotation, 0, 1);

            int distance = SymbolLight.TurnOnValue - SymbolLight.TurnOffValue;

            float CenterBright = 0;

            if (isNegative)
            {
                CenterBright = Math.Clamp(SymbolLight.TurnOnValue - (distance * brightnessTest), 0, 255);
            }
            else
            {
                CenterBright = Math.Clamp(SymbolLight.TurnOffValue + (distance * brightnessTest), 0, 255);
            }
            return (byte)Math.Clamp(CenterBright, 0, 255);
        }*/

        // リール位置が変わったときのコールバック
        private void OnReelPosChangedCallback()
        {
            //Debug.Log("Reel pos changed");
            ChangeReelPos(reelSpinPresenter.GetCurrentSpeed());
            reelArrayPresenter.UpdateReelSymbols(reelObjectModel.CurrentLower);
        }

        // リールが停止したときのコールバック
        private void OnReelHasStoppedCallback()
        {
            Debug.Log("StoppedEvent called");
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
