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

    // リール本体
    private ReelBase reelBase;
    // リール内の図柄
    private SymbolChange[] symbolsObj;

    // 止まる予定か
    public bool IsStopping { get; private set; }
    // 停止したか
    public bool HasStopped { get; private set; }
    // 最後に止めた位置(下段基準)
    public int lastPressedPos { get; private set; }
    // 最後に止めたときのディレイ数
    public int lastDelay { get; private set; }
    // リール情報
    public ReelData ReelData { get; private set; }

    // 初期化
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        delayToStop = 0;
        lastPressedPos = 0;
        lastDelay = 0;
        IsStopping = false;
        HasStopped = true;

        symbolsObj = GetComponentsInChildren<SymbolChange>();
        reelBase = GetComponentInChildren<ReelBase>();
        Debug.Log("ReelSpin AwakeDone");

        Debug.Log(ChangeAngle);
    }

    private void Start()
    {
        UpdateSymbolsObjects();
        Debug.Log("StartDone");
    }

    // 実行中(60FPSでの更新)
    private void FixedUpdate()
    {
        if (maxSpeed != 0)
        {
            //止まっていないときは加速
            if (rotateSpeed <= maxSpeed)
            {
                SpeedUpReel();
            }
        }
    }

    // 実行中(常時更新)
    private void Update()
    {
        if(maxSpeed != 0)
        {
            RotateReel();
        }
    }

    // func
    // リールデータを渡す
    public void SetReelData(ReelData reelData) => ReelData = reelData;
    // 指定位置からリール位置を渡す
    public int GetReelPos(int posID) => ReelData.GetReelPos((sbyte)posID);
    // 指定位置からリール図柄を渡す
    public ReelData.ReelSymbols GetReelSymbol(int posID) => ReelData.GetReelSymbol((sbyte)posID);
    // 押した位置を返す(テーブル制御判定用)
    public int GetPressedPos() => ReelData.GetReelPos((int)ReelData.ReelPosID.Center);
    // 最後に止めた停止位置を返す
    public int GetLastPressedPos() => lastPressedPos;
    // 直近のディレイ数を返す
    public int GetLastDelay() => lastDelay;

    //　リール始動
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        HasStopped = false;
    }

    // リール停止
    public void StopReel(int delay)
    {
        // 停止位置を記録
        lastPressedPos = ReelData.GetReelPos((int)ReelData.ReelPosID.Center);

        if (delay < 0 || delay > MaxDelay)
        {
            throw new Exception("Invalid Delay. Must be within 0~4");
        }
        Debug.Log("Received Stop Delay:" + delay);

        // テーブルから得たディレイを記録し、その分リールの停止を遅らせる。
        delayToStop = delay;
        lastDelay = delay;
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

        // 一定角度に達したら図柄の更新(17.14286度)
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

                Debug.Log("Stopped");
            }

            // 停止するがディレイ(スベリ)があれば数値を減らす(次の図柄更新で止める)
            else if (IsStopping)
            {
                delayToStop -= 1;
            }
        }
    }

    // リール本体そのものの明るさを変更
    public void SetReelBaseBrightness(byte brightness) => reelBase.SetBrightness(brightness);

    // 指定位置リール図柄の明るさを変更
    public void SetSymbolBrightness(int posArrayID, byte brightness) => symbolsObj[posArrayID].ChangeBrightness(brightness);

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
