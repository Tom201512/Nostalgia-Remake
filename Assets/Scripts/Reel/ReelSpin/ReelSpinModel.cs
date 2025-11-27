using System;
using UnityEngine;

namespace ReelSpinGame_Reels.Spin
{
    public class ReelSpinModel
    {
        // const
        // リール配列数
        public const int MaxReelArray = 21;
        // 図柄
        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay }
        // リール位置識別用
        public enum ReelPosID { Lower2nd = -1, Lower, Center, Upper, Upper2nd }
        // 図柄変更時の角度 (360度を21分割)
        public const float ChangeAngle = 360.0f / 21.0f;
        // 図柄停止時の角度 (変更時角度から1度引いた角度)
        public const float StopAngle = ChangeAngle - 0.5f;
        // リール半径(cm)
        const float ReelRadius = 12.75f;
        // 最高速度までの経過時間(秒)
        const float MaxSpeedReelTime = 0.3f;

        // リールの状態(停止、回転、停止信号受理、停止中)
        public enum ReelStatus { Stopped, Spinning, RecieveStop, Stopping}

        // var
        // リール配列
        public byte[] ReelArray { get; set; }

        // 現在の回転速度
        public float RotateSpeed { get; set; }
        // 最高速度
        public float MaxSpeed { get; set; }
        // 1秒間のRPS
        public float RotateRPS { get; private set; }
        // JAC中の点灯をするか
        public bool HasJacModeLight { get; set; }
        // 現在のリール状態
        public ReelStatus CurrentReelStatus { get; set; }

        // 現在の位置(下段基準)
        public int CurrentLower { get; set; }
        // 押した位置(中段基準)
        public int LastPushedPos { get; set; }
        // 将来的に止まる位置(下段基準)
        public int WillStopLowerPos { get; set; }
        // 押し順
        public int LastStoppedOrder { get; set; }
        // 最後に止めた時のスベリコマ数
        public int LastStoppedDelay { get; set; }

        public ReelSpinModel(float rotateRPM)
        {
            RotateSpeed = 0.0f;
            MaxSpeed = 0.0f;
            RotateRPS = rotateRPM / 60.0f;
            CurrentLower = 0;
            LastStoppedOrder = 0;
            HasJacModeLight = false;
            CurrentReelStatus = ReelStatus.Stopped;
        }

        // RPSの変更
        public void ChangeRotateRPS(float rotateRPM) => RotateRPS = rotateRPM / 60.0f;

        // 1秒に何度回転させるか計算する
        public float ReturnDegreePerSecond()
        {
            // ラジアンを求める
            float radian = RotateRPS * 2.0f * MathF.PI;

            // ラジアンから毎秒動かす角度を計算

            float result = 180.0f / MathF.PI * radian;
            //Debug.Log("deg/s:" + result);
            return result;
        }

        // 速度加速
        public void AccelerateReelSpeed()
        {
            // 回転速度が最高速度より低ければ加速、高ければ減速させる。
            // 逆回転の場合
            if (Math.Sign(MaxSpeed) == -1)
            {
                if (RotateSpeed > MaxSpeed)
                {
                    RotateSpeed = Mathf.Clamp(RotateSpeed += ReturnReelAccerateSpeed() * Time.deltaTime * -1, MaxSpeed, 0);
                }
                else if (RotateSpeed < MaxSpeed)
                {
                    RotateSpeed = Mathf.Clamp(RotateSpeed -= ReturnReelAccerateSpeed() * Time.deltaTime * -1, MaxSpeed, 0);
                }
            }
            // 前回転の場合
            else
            {
                if (RotateSpeed < MaxSpeed)
                { 
                    RotateSpeed = Mathf.Clamp(RotateSpeed += ReturnReelAccerateSpeed() * Time.deltaTime * 1, 0, MaxSpeed);
                }
                else if (RotateSpeed > MaxSpeed)
                {
                    Debug.Log("Speed down");
                    RotateSpeed = Mathf.Clamp(RotateSpeed -= ReturnReelAccerateSpeed() * Time.deltaTime * 1, 0, MaxSpeed);
                }
            }
        }

        // リール回転を終了する
        public void FinishReelSpin()
        {
            Debug.Log("Finish ReelSpin");
            RotateSpeed = 0f;
            MaxSpeed = 0f;
            CurrentReelStatus = ReelStatus.Stopped;
        }

        // 加速度を返す
        private float ReturnReelAccerateSpeed()
        {
            // ラジアンを求める
            float radian = RotateRPS * 2.0f * MathF.PI;
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