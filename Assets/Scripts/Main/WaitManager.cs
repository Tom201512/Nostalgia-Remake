using System;
using System.Timers;
using UnityEngine;

public class WaitManager
{
    // ウェイト管理

    // const
    // ウェイトに必要な時間(ミリ秒)
    public const int WaitTimerSetting = 4100;

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }

    // var
    // 処理用タイマー
    private Timer updateTimer;
    // ウェイトが有効か
    public bool hasWait { get; private set; }
    // ウェイトを無効にしているか
    public bool hasWaitCut { get; private set; }

    // コンストラクタ
    public WaitManager(bool hasWaitCut)
    {
        // 処理用タイマー作成
        this.hasWaitCut = hasWaitCut;
        updateTimer = new Timer(WaitTimerSetting);
    }

    // デストラクタ
    ~WaitManager()
    {
        // Timerのストップ
        updateTimer.Stop();
        updateTimer.Dispose();
    }

    // func

    // ウェイトカットの設定
    public void SetWaitCutSetting(bool hasWaitCut) => this.hasWaitCut = hasWaitCut;

    // ウェイトをかける
    public void SetWaitTimer()
    {
        if (hasWaitCut)
        {
            Debug.Log("WaitCut is enabled");
        }

        else if(hasWait)
        {
            Debug.Log("Wait is enabled already");
        }

        // ウェイトカット、または実行中のウェイトがなければ実行
        else
        {
            hasWait = true;
            updateTimer.Elapsed += WaitProcess;
            updateTimer.AutoReset = false;
            updateTimer.Start();

            Debug.Log("Wait start");
        }
    }

    // コルーチン用

    // ウェイト管理
    private void WaitProcess(object sender, ElapsedEventArgs e)
    {
        hasWait = false;
        updateTimer.Elapsed -= WaitProcess;
        Debug.Log("Wait disabled");
    }
}
