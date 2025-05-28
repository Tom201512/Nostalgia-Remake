using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class ReelObject : MonoBehaviour
{
    // リールオブジェクト

    // const
    // 図柄変更時の角度 (360度を21分割)
    const float ChangeAngle = 360.0f / 21.0f;
    // 回転速度 (Rotate Per Second)
    public const float RotateRPS = 79.8f / 60.0f;
    // リール重さ(kg)
    public const float ReelWeight = 25.5f;
    // リール半径(cm)
    public const float ReelRadius = 12.75f;

    // var
    // 現在の回転速度
    private float rotateSpeed;
    // 最高速度
    private float maxSpeed;

    // リール本体
    private ReelBase reelBase;
    // リール内の図柄
    //private SymbolChange[] symbolsObj;
    // 図柄マネージャー
    private SymbolManager symbolManager;

    // リール情報
    private ReelData reelData;

    // リール情報
    [SerializeField] ReelDatabase reelDatabaseFile;

    // 初期化
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;

        reelBase = GetComponentInChildren<ReelBase>();
        symbolManager = GetComponentInChildren<SymbolManager>();

        foreach (byte value in reelDatabaseFile.Array)
        {
            Debug.Log(value + "Symbol:" + ReturnSymbol(value));
        }

        for (int i = 0; i < reelDatabaseFile.Array.Length; i++)
        {
            Debug.Log("No." + i + " Symbol:" + ReturnSymbol(reelDatabaseFile.Array[i]));

        }
        Debug.Log("ReelArray Generated");
        Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        symbolManager.SetReelData(reelData);
        symbolManager.UpdateSymbolsObjects();
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

    // 数値を得る
    // リールのID
    public int GetReelID() => reelData.ReelID;
    // 停止可能か
    public bool GetCanStop() => reelData.CanStop;
    // 停止処理中か
    public bool GetIsStopping() => reelData.IsStopping;
    // 停止したか
    public bool GetHasStopped() => reelData.HasStopped;
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

    // リールデータを渡す
    public void SetReelData(int reelID, int initialLowerPos)
    {
        reelData = new ReelData(reelID, initialLowerPos, reelDatabaseFile);
    }

    // 最高速度が返す
    public bool IsMaximumSpeed() => rotateSpeed == maxSpeed;

    //　リール始動
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        reelData.BeginStartReel();
    }

    // リール停止
    public void StopReel(int pushedPos, int delay)
    {
        reelData.BeginStopReel(pushedPos, delay);
    }

    // 速度加速
    private void SpeedUpReel()=> 
        rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), -1 * maxSpeed, maxSpeed);

    // リール回転
    private void RotateReel()
    {
        transform.Rotate((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed * Vector3.left);

        // 一定角度に達したら図柄の更新(17.14286度)
        if ((Math.Abs(transform.rotation.eulerAngles.x) <= 360.0f - ChangeAngle && Math.Sign(rotateSpeed) == -1 ||
            (Math.Abs(transform.rotation.eulerAngles.x) >= ChangeAngle && Math.Sign(rotateSpeed) == 1)))
        {
            // 図柄位置変更
            reelData.ChangeReelPos(rotateSpeed);
            symbolManager.UpdateSymbolsObjects();

            // 図柄の場所だけ変更角度分回転を戻す
            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

            // 停止する位置になったら
            if (reelData.IsStopping && reelData.CheckReachedStop())
            {
                // 再度リールの角度を調整して停止させる
                transform.Rotate(Vector3.left, Math.Abs(transform.rotation.eulerAngles.x));
                rotateSpeed = 0;
                maxSpeed = 0;
                reelData.FinishStopReel();

                Debug.Log("Stopped");
            }
        }
    }

    // リール本体そのものの明るさを変更
    public void SetReelBaseBrightness(byte brightness) => reelBase.SetBrightness(brightness);
    // 指定位置リール図柄の明るさを変更
    public void SetSymbolBrightness(int posID, byte r, byte g, byte b) => symbolManager.SymbolObj[ReelData.GetReelArrayIndex(posID)].ChangeBrightness(r,g,b);
    // 指定位置リール図柄の光度を変更
    public void SetSymbolEmission(int posID, byte r, byte g, byte b) => symbolManager.SymbolObj[ReelData.GetReelArrayIndex(posID)].ChangeEmmision(r,g,b);

    // 回転率計算
    private float ReturnAngularVelocity(float rpsValue)
    {
        // ラジアンを求める
        float radian = rpsValue * 2.0f * MathF.PI;

        // ラジアンから毎秒動かす角度を計算
        return radian * 180.0f / MathF.PI;
    }

    // 加速度を返す
    private float ReturnReelAccerateSpeed(float rpsValue) => ReelRadius / 80f * ReturnAngularVelocity(rpsValue) / 1000.0f;
}
