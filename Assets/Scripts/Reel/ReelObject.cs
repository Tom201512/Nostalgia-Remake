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
    public class ReelObject : MonoBehaviour
    {
        // リールオブジェクト

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

        // リール回転用のプレゼンター
        private ReelSpinPresenter reelSpinPresenter;
        // リール配列用のプレゼンター
        private ReelArrayPresenter reelArrayPresenter;

        // リール情報
        private ReelData reelData;
        // JAC中の点灯をするか
        public bool HasJacModeLight { get; set; }
        // モーションブラー
        private PostProcessVolume postVolume;
        // ブラー部分のプロファイル
        private MotionBlur motionBlur;

        private void Awake()
        {
            reelSpinPresenter = GetComponent<ReelSpinPresenter>();
            ReelEffectManager = GetComponent<ReelEffect>();

            // ブラーの取得
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);

            reelSpinPresenter.SetReelSpinPresenter(rotateRPM);
        }

        private void Start()
        {
            ChangeBlurSetting(false);
        }

        // func

        // 数値を得る
        // リールのID
        public int GetReelID() => reelData.ReelID;
        // 現在のリール状態
        public ReelStatus GetCurrentReelStatus() => reelSpinPresenter.GetCurrentReelStatus();
        // 最後に止めた下段位置
        public int GetLastPushedPos() => reelData.LastPushedPos;
        // 停止予定位置
        public int GetWillStopPos() => reelData.WillStopPos;
        // 最後に止めたときのディレイ数
        public int GetLastDelay() => reelData.LastDelay;
        // 指定した位置にあるリールの番号を返す
        public int GetReelPos(ReelPosID posID) => reelData.GetReelPos((sbyte)posID);
        // sbyteで読む場合
        public int GetReelPos(sbyte posID) => reelData.GetReelPos(posID);

        // 速度
        // 現在速度を返す
        public float GetCurrentSpeed() => reelSpinPresenter.GetCurrentSpeed();
        // 現在の角度を返す
        public float GetCurrentDegree() => reelSpinPresenter.GetCurrentDegree();
        // リール条件を渡す
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // リールデータを渡す
        public void SetReelData(int reelID, int initialLowerPos)
        {
            reelArrayPresenter = GetComponent<ReelArrayPresenter>();
            reelArrayPresenter.SetReelArrayPresenter(initialLowerPos, reelDatabaseFile.Array);
        }

        // リール位置変更(セーブデータ読み込み用)
        public void SetReelPos(int lowerPos) => reelArrayPresenter.SetCurrentLower(lowerPos);

        //　リール始動
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            reelSpinPresenter.StartReelSpin(maxSpeed, cutAccelerate);
        }

        // ブラー設定
        public void ChangeBlurSetting(bool value) => motionBlur.enabled.value = value;

        // リール停止
        public void StopReel(int pushedPos, int delay) => reelData.BeginStopReel(pushedPos, delay, false);

        // リール停止(高速版)
        public void StopReelFast(int pushedPos, int delay)
        {
            // 強制停止
            reelData.BeginStopReel(pushedPos, delay, true);

            // JAC中ならライトも調整
            if (HasJacModeLight)
            {
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center,SymbolLight.TurnOnValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
                ReelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
            }

            //ReelSpinPresenter.StopReelSpeed();
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
    }
}
