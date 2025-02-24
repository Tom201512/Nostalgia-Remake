using ReelSpinGame_Reels;
using System;
using UnityEngine;

public class ReelObject : MonoBehaviour
{
    // リールオブジェクト

    // const

    // 図柄変更時の角度 (360度を21分割)
    const float ChangeAngle = 360.0f / 21.0f;

    // 回転速度 (Rotate Per Second)
    public const float RotateRPS = 79.7f / 60.0f;

    // リール重さ(kg)
    public const float ReelWeight = 25.5f;

    // リール半径(cm)
    public const float ReelRadius = 12.75f;

    // 最高ディレイ(スベリコマ数 4)
    public const int MaxDelay = 4;


    // var

    // 現在の回転速度
    private float rotateSpeed;

    // 最高速度
    private float maxSpeed;

    // 停止するのに必要なディレイ(スベリ)
    private int delayToStop;

    // 止まる予定か
    public bool IsStopping { get; private set; }

    // 停止したか
    public bool HasStopped { get; private set; }

    // リール情報を持つ
    public ReelData ReelData { get; private set; }
    // リール内の図柄
    private SymbolChange[] symbolsObj;


    // 初期化
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        delayToStop = 0;
        IsStopping = false;
        HasStopped = true;
        symbolsObj = GetComponentsInChildren<SymbolChange>();
        Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        UpdateSymbolsObjects();
        Debug.Log("StartDone");
    }

    // 実行中(60FPSでの更新)
    private void FixedUpdate()
    {
        if(maxSpeed != 0)
        {
            //止まっていないときは加速
            if (rotateSpeed <= maxSpeed) 
            { 
                SpeedUpReel(); 
            }
            RotateReel();
        }
    }


    // func

    // リールデータを渡す
    public void SetReelData(ReelData reelData) => ReelData = reelData;

    // 指定位置からリール位置を渡す
    public int GetReelPos(ReelData.ReelPosID posID) => ReelData.GetReelPos((sbyte)posID);

    // 指定位置からリール図柄を渡す
    public ReelData.ReelSymbols GetReelSymbol(ReelData.ReelPosID posID) => ReelData.GetReelSymbol((sbyte)posID);


    //　リール始動
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        HasStopped = false;
    }

    // リール停止
    public void StopReel(int delay)
    {
        if(delay < 0 || delay > MaxDelay)
        {
            throw new Exception("Invalid Delay. Must be within 0~4");
        }

        Debug.Log("Received Stop Delay:" + delay);
        delayToStop = delay;
        IsStopping = true;
    }

    // 速度加速
    private void SpeedUpReel()
    {
        Math.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), 
            -1 * maxSpeed, maxSpeed);
    }

    // リール回転
    private void RotateReel()
    {
        transform.Rotate((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed * Vector3.left);

        // 一定角度に達したら図柄の更新(17.174 or 342.826)
        if ((Math.Abs(transform.rotation.eulerAngles.x) <= 360.0f - ChangeAngle && Math.Sign(rotateSpeed) == -1 ||
            (Math.Abs(transform.rotation.eulerAngles.x) >= ChangeAngle && Math.Sign(rotateSpeed) == 1)))
        {
            // 図柄位置変更
            ReelData.ChangeReelPos(rotateSpeed);
            UpdateSymbolsObjects();

            // 変更角度分だけ回転を戻す。
            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

            // 停止する場合は
            if (IsStopping && delayToStop == 0)
            {
                // 再度リールの角度を調整して停止させる
                transform.Rotate(Vector3.left, Math.Abs(transform.rotation.eulerAngles.x));
                rotateSpeed = 0;
                maxSpeed = 0;
                IsStopping = false;
                HasStopped = true;
            }

            // 停止するがディレイ(スベリ)があれば数値を減らす(次の図柄更新で止める)
            else if (IsStopping)
            {
                delayToStop -= 1;
            }
        }
    }

    //図柄の更新
    private void UpdateSymbolsObjects()
    {
        // 現在のリール下段を基準として位置を更新する。
        foreach(SymbolChange symbol in symbolsObj)
        {
            symbol.ChangeSymbol(ReelData.GetReelSymbol((sbyte)symbol.GetPosID()));
        }
    }

    // 回転率計算
    private float ReturnAngularVelocity(float rpsValue)
    {
        // ラジアンを求める
        float radian = rpsValue * 2.0f * MathF.PI;

        // ラジアンから毎秒動かす角度を計算
        return radian * 180.0f / MathF.PI;
    }

    // 加速度を返す
    private float ReturnReelAccerateSpeed(float rpsValue)
    {
        return ReelRadius / 100f * ReturnAngularVelocity(rpsValue) / 1000.0f;
    }
}
