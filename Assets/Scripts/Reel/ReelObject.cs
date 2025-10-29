using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Symbol;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_Reels
{
    public class ReelObject : MonoBehaviour
    {
        // リールオブジェクト

        // const
        // 図柄変更時の角度 (360度を21分割)
        const float ChangeAngle = 360.0f / 21.0f;
        // リール半径(cm)
        const float ReelRadius = 12.75f;
        // JAC時の光度調整数値
        const float JacLightOffset = 0.4f;
        // 最高速度までの経過時間(秒)
        const float MaxSpeedReelTime = 0.3f;

        // var
        // 1分間の回転数 (Rotate Per Minute)
        [Range(0f, 80f), SerializeField] private float rotateRPM;

        // 現在の回転速度
        private float rotateSpeed;
        // 最高速度
        private float maxSpeed;
        // 図柄マネージャー
        private SymbolManager symbolManager;

        // リール情報
        private ReelData reelData;
        // JAC中の点灯をするか
        public bool HasJacModeLight { get; set; }
        // モーションブラー
        private PostProcessVolume postVolume;
        // ブラー部分のプロファイル
        private MotionBlur motionBlur;

        // 1秒間の回転数 (Rotate Per Second)
        private float rotateRPS;

        // リールが停止したかのイベント(個別ごとのリール)
        public delegate void ReelStoppedEvent();
        public event ReelStoppedEvent HasReelStopped;

        // リール演出用マネージャー
        private ReelEffect reelEffectManager;

        // リール情報
        [SerializeField] ReelDatabase reelDatabaseFile;

        // 初期化
        private void Awake()
        {
            rotateSpeed = 0.0f;
            maxSpeed = 0.0f;
            rotateRPS = rotateRPM / 60.0f;
            HasJacModeLight = false;

            symbolManager = GetComponentInChildren<SymbolManager>();
            reelEffectManager = GetComponentInChildren<ReelEffect>();

            // ブラーの取得
            postVolume = GetComponent<PostProcessVolume>();
            postVolume.profile.TryGetSettings(out motionBlur);
        }

        private void Start()
        {
            symbolManager.UpdateSymbolsObjects(reelData);
            ChangeBlurSetting(false);
        }

        private void Update()
        {
            if (maxSpeed != 0)
            {
                ChangeReelSpeed();
                RotateReel();
            }
        }

        // func

        // 数値を得る
        // 現在のスピード
        public float GetCurrentSpeed() => rotateSpeed;
        // 現在の角度
        public float GetCurrentDegree() => transform.rotation.eulerAngles.x;
        // リールのID
        public int GetReelID() => reelData.ReelID;
        // 現在のリール状態
        public ReelStatus GetCurrentReelStatus() => reelData.CurrentReelStatus;
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

        // 指定した位置の図柄を返す
        public ReelSymbols GetReelSymbol(ReelPosID posID) => reelData.GetReelSymbol((sbyte)posID);
        // sbyteで読む場合
        public ReelSymbols GetReelSymbol(sbyte posID) => reelData.GetReelSymbol(posID);
        // 停止予定位置からリールの図柄を返す
        public ReelSymbols GetSymbolFromWillStop(ReelPosID posID) => reelData.GetSymbolFromWillStop((sbyte)posID);
        // sbyteで読む場合
        public ReelSymbols GetSymbolFromWillStop(sbyte posID) => reelData.GetSymbolFromWillStop(posID);
        // リール条件を渡す
        public ReelDatabase GetReelDatabase() => reelDatabaseFile;

        // 指定番号の図柄画像を返す
        public Sprite GetSymbolImageAtPos(int pos) => symbolManager.GetSymbolImage(reelDatabaseFile.Array[pos]);

        // リールデータを渡す
        public void SetReelData(int reelID, int initialLowerPos)
        {
            reelData = new ReelData(reelID, initialLowerPos, reelDatabaseFile);
        }

        // リール位置変更(セーブデータ読み込み用)
        public void SetReelPos(int lowerPos) => reelData.SetReelPos(lowerPos);

        // 最高速度状態か返す
        public bool IsMaximumSpeed() => rotateSpeed == maxSpeed;

        //　リール始動
        public void StartReel(float maxSpeed, bool cutAccelerate)
        {
            this.maxSpeed = maxSpeed;

            // 加速をカットする場合はすぐに速度を上げる
            if (cutAccelerate)
            {
                rotateSpeed = maxSpeed;
            }
            reelData.BeginStartReel();
            ChangeBlurSetting(true);
        }

        // 速度変更
        public void AdjustMaxSpeed(float maxSpeed)
        {
            Debug.Log("Max speed changed:" + maxSpeed);
            if (Math.Sign(this.maxSpeed) != Math.Sign(maxSpeed))
            {
                rotateSpeed = 0f;
            }
            this.maxSpeed = maxSpeed;
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
            // 図柄位置変更
            symbolManager.UpdateSymbolsObjects(reelData);

            // JAC中ならライトも調整
            if (HasJacModeLight)
            {
                reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center,SymbolLight.TurnOnValue);
                reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOffValue);
                reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
            }

            StopReelSpeed();
        }

        // 速度変更
        private void ChangeReelSpeed()
        {
            // 回転速度が最高速度より低ければ加速、高ければ減速させる。
            // 逆回転の場合
            if(Math.Sign(maxSpeed) == -1)
            {
                if (rotateSpeed > maxSpeed)
                {
                    Debug.Log("Speed Up Reverse");
                    rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * -1, maxSpeed, 0);
                }
                else if (rotateSpeed < maxSpeed)
                {
                    Debug.Log("Speed Down Reverse");
                    rotateSpeed = Mathf.Clamp(rotateSpeed -= ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * -1, maxSpeed, 0);
                }
            }
            // 前回転の場合
            else
            {
                if (rotateSpeed < maxSpeed)
                {
                    rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * 1, 0, maxSpeed);
                }
                else if (rotateSpeed > maxSpeed)
                {
                    Debug.Log("Speed down");
                    rotateSpeed = Mathf.Clamp(rotateSpeed -= ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * 1, 0, maxSpeed);
                }
            }

            Debug.Log("RotateSpeed:" + rotateSpeed);
        }

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
        }

        // リール回転
        private void RotateReel()
        {
            float rotationAngle;
            rotationAngle = Math.Clamp((ReturnDegreePerSecond(rotateRPS)) * Time.deltaTime * rotateSpeed, -360, 360);
            Debug.Log("RotationAngle:" + rotationAngle);

            Debug.Log(rotationAngle * Vector3.left);
            transform.Rotate(rotationAngle * Vector3.left);

            // JAC中であればライトの調整をする
            if (HasJacModeLight)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(false));
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, CalculateJACBrightness(true));
                }
                else
                {
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, CalculateJACBrightness(false));
                    reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, CalculateJACBrightness(true));
                }
            }

            Debug.Log("Euler:" + transform.rotation.eulerAngles.x);
            //Debug.Log("ChangeAngle:" + (360f - ChangeAngle));

            // 一定角度に達したら図柄の更新(17.14286度)

            // 回転速度に合わせて変更
            // 逆回転の場合
            if(Math.Sign(rotateSpeed) == -1 && transform.rotation.eulerAngles.x < 180 && transform.rotation.eulerAngles.x > ChangeAngle)
            {
                Debug.Log("Symbol changed");
                // 図柄位置変更
                reelData.ChangeReelPos(rotateSpeed);
                symbolManager.UpdateSymbolsObjects(reelData);

                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ChangeAngle * -1);

                if (HasJacModeLight)
                {
                    if (Math.Sign(maxSpeed) == -1)
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
                    }
                }

                // 停止する位置になったら
                if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }
            }
            // 前回転の場合
            else if(Math.Sign(rotateSpeed) == 1 && transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360f - ChangeAngle)
            {
                Debug.Log("Symbol changed");
                // 図柄位置変更
                reelData.ChangeReelPos(rotateSpeed);
                symbolManager.UpdateSymbolsObjects(reelData);

                // 角度をもとに戻す
                transform.Rotate(Vector3.right, ChangeAngle);


                if (HasJacModeLight)
                {
                    if (Math.Sign(maxSpeed) == -1)
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Lower, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Upper, SymbolLight.TurnOffValue);
                        reelEffectManager.ChangeSymbolBrightness((int)ReelPosID.Center, SymbolLight.TurnOnValue);
                    }
                }

                // 停止する位置になったら
                if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
                {
                    StopReelSpeed();
                }
            }
        }

        // リールの回転を停止させる
        private void StopReelSpeed()
        {
            // 再度リールの角度を調整して停止させる
            transform.rotation = Quaternion.identity;
            rotateSpeed = 0;
            maxSpeed = 0;
            reelData.FinishStopReel();
            HasReelStopped.Invoke();
        }

        // 1秒に何度回転させるか計算
        private float ReturnDegreePerSecond(float rpsValue)
        {
            // ラジアンを求める
            float radian = rpsValue * 2.0f * MathF.PI;

            // ラジアンから毎秒動かす角度を計算

            float result = 180.0f / MathF.PI * radian;
            //Debug.Log("deg/s:" + result);
            return result;
        }

        // 加速度を返す
        private float ReturnReelAccerateSpeed(float rpsValue)
        {
            // ラジアンを求める
            float radian = rpsValue * 2.0f * MathF.PI;
            // 接線速度(m/s)を求める
            float tangentalVelocity = ReelRadius * radian / 100f;
            //Debug.Log("TangentalVelocity:" + tangentalVelocity);

            // 必要経過時間から速度を出す
            float speed = tangentalVelocity / MaxSpeedReelTime;
            //Debug.Log("Speed:" + speed);
            return speed;
        }
    }
}
