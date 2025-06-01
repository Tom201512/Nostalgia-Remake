using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.ReelData;

public class ReelObject : MonoBehaviour
{
    // リールオブジェクト

    // const
    // 回転時図柄変更の角度 (360度を21分割) (回転は下方向)
    //const float ChangeAngle = 360.0f - 360.0f / 21.0f;
    // 逆回転時図柄変更の角度
    //const float ReversedChangeAngle = 360.0f / 21.0f;
    // 図柄変更時の角度 (360度を21分割)
    const float ChangeAngle = 360.0f / 21.0f;
    // 停止最大有効範囲
    const float ChangeOffset = 0f;
    // 回転速度 (Rotate Per Second)
    const float RotateRPS = 79.8f / 60.0f;
    // リール半径(cm)
    const float ReelRadius = 12.75f;
    // JAC時の光度調整数値
    const float JacLightOffset = 0.4f;

    // var
    // 現在の回転速度
    private float rotateSpeed;
    // 最高速度
    private float maxSpeed;
    // リール本体
    private ReelBase reelBase;
    // 図柄マネージャー
    private SymbolManager symbolManager;
    // リール情報
    private ReelData reelData;
    // JAC中の点灯をするか
    public bool HasJacModeLight { get; set; }
    // モーションブラー
    private PostProcessVolume postVolume;

    // リール情報
    [SerializeField] ReelDatabase reelDatabaseFile;

    // 初期化
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        HasJacModeLight = false;

        reelBase = GetComponentInChildren<ReelBase>();
        symbolManager = GetComponentInChildren<SymbolManager>();
        postVolume = GetComponent<PostProcessVolume>();
        postVolume.enabled = false;

        foreach (byte value in reelDatabaseFile.Array)
        {
            ////Debug.Log(value + "Symbol:" + ReturnSymbol(value));
        }

        for (int i = 0; i < reelDatabaseFile.Array.Length; i++)
        {
            ////Debug.Log("No." + i + " Symbol:" + ReturnSymbol(reelDatabaseFile.Array[i]));

        }
        ////Debug.Log("ReelArray Generated");
        ////Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        symbolManager.SetReelData(reelData);
        symbolManager.UpdateSymbolsObjects();
        //Debug.Log("StartDone");
        //Debug.Log("RPS:" + RotateRPS);
    }

    private void FixedUpdate()
    {
        //止まっていないときは加速
        if (rotateSpeed < maxSpeed && Math.Sign(maxSpeed) == 1 ||
        rotateSpeed > maxSpeed && Math.Sign(maxSpeed) == -1)
        {
            SpeedUpReel();
        }
    }

    // 実行中(常時更新)
    private void Update()
    {
        if (maxSpeed != 0)
        {
            RotateReel();
        }
    }

    // func

    // 数値を得る
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
    public void StopReel(int pushedPos, int delay) => reelData.BeginStopReel(pushedPos, delay);

    // 速度加速
    private void SpeedUpReel() =>
        rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), -1 * maxSpeed, maxSpeed);

    // JAC時の明るさ計算(
    private byte CalculateJACBrightness(bool isNegative)
    {
        float brightnessTest = 0;
        float currentDistance = 0;

        // 少し早めに光らせるため距離は短くする
        float distanceRotation = ChangeAngle * JacLightOffset;

        // 符号に合わせて距離を計算
        Debug.Log("Current Euler:" + transform.rotation.eulerAngles.x);
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
        Debug.Log("Brightness:" + brightnessTest);

        int distance = SymbolChange.TurnOnValue - SymbolChange.TurnOffValue;

        float CenterBright = 0;

        if (isNegative)
        {
            CenterBright = Math.Clamp(SymbolChange.TurnOnValue - (distance * brightnessTest), 0, 255);
        }
        else
        {
            CenterBright = Math.Clamp(SymbolChange.TurnOffValue + (distance * brightnessTest), 0, 255);
        }
        return (byte)Math.Clamp(CenterBright, 0, 255);
    }

    // リール回転
    private void RotateReel()
    {
        float rotationAngle = Math.Clamp((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed, 0, 360);
        transform.Rotate(rotationAngle * Vector3.left);

        if (HasJacModeLight)
        {
            if (Math.Sign(maxSpeed) == -1)
            {
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(CalculateJACBrightness(false));
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Lower)].ChangeBrightness(CalculateJACBrightness(true));
            }
            else
            {
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Upper)].ChangeBrightness(CalculateJACBrightness(false));
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(CalculateJACBrightness(true));
            }
        }

        Debug.Log("Euler:" + transform.rotation.eulerAngles.x);
        Debug.Log("ChangeAngle:" + ChangeAngle);

        // 一定角度に達したら図柄の更新(17.14286度)
        if (transform.rotation.eulerAngles.x > 0 &&
            (transform.rotation.eulerAngles.x < 360f - ChangeAngle && Math.Sign(rotateSpeed) == 1))
        {
            Debug.Log("Symbol changed");
            // 図柄位置変更
            reelData.ChangeReelPos(rotateSpeed);
            symbolManager.UpdateSymbolsObjects();

            // 図柄の場所だけ変更角度分回転を戻す
            float dif = 0f;

            if (Math.Sign(maxSpeed) == -1)
            {
                //dif = ReversedChangeAngle - transform.rotation.eulerAngles.x;
                dif = ChangeAngle - transform.rotation.eulerAngles.x;
            }
            else
            {
                dif = ChangeAngle - transform.rotation.eulerAngles.x;
            }

            if (dif != 0)
            {
                //transform.rotation = Quaternion.identity;
                //transform.Rotate(ReversedChangeAngle * Vector3.right);
            }

            // 図柄の場所だけ変更角度分回転を戻す
            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

            if (HasJacModeLight)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(SymbolChange.TurnOffValue);
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Lower)].ChangeBrightness(SymbolChange.TurnOnValue);
                }
                else
                {
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Upper)].ChangeBrightness(SymbolChange.TurnOffValue);
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(SymbolChange.TurnOnValue);
                }
            }

            // 停止する位置になったら
            if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
            {
                // 再度リールの角度を調整して停止させる
                transform.rotation = Quaternion.identity;
                rotateSpeed = 0;
                maxSpeed = 0;
                reelData.FinishStopReel();

                //Debug.Log("Stopped");
            }

            // 変更後はポストエフェクトを切る
            postVolume.enabled = false;
        }
        // 回転動作中はポストエフェクトを有効にする
        else
        {
            postVolume.enabled = true;
        }
    }

    // リール本体そのものの明るさを変更
    public void SetReelBaseBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);
    // 指定位置リール図柄の明るさを変更
    public void SetSymbolBrightness(int posID, byte brightness) => symbolManager.SymbolObj[GetReelArrayIndex(posID)].ChangeBrightness(brightness);

    // 回転角計算
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
