using System.Timers;

public class WaitManager
{
    // ウェイト管理

    // const
    // ウェイトに必要な時間(ミリ秒)
    public const int WaitTimerSetting = 4100;
    // var
    // 処理用タイマー
    private Timer updateTimer;
    // ウェイトが有効か
    public bool HasWait { get; private set; }
    // ウェイトを無効にしているか
    public bool HasWaitCut { get; private set; }

    // コンストラクタ
    public WaitManager(bool hasWaitCut)
    {
        // 処理用タイマー作成
        HasWaitCut = hasWaitCut;
        updateTimer = new Timer(WaitTimerSetting);
    }

    // func
    // タイマー処理の破棄
    public void DisposeWait()
    {
        // Timerのストップ
        updateTimer.Stop();
        updateTimer.Dispose();
    }

    // ウェイトカットの設定
    public void SetWaitCutSetting(bool hasWaitCut) => HasWaitCut = hasWaitCut;

    // ウェイトをかける
    public void SetWaitTimer()
    {
        // ウェイトカット、または実行中のウェイトがなければ実行
        if (!HasWaitCut && !HasWait)
        {
            HasWait = true;
            updateTimer.Elapsed += WaitProcess;
            updateTimer.AutoReset = false;
            updateTimer.Start();
        }
    }

    // コルーチン用

    // ウェイト管理
    private void WaitProcess(object sender, ElapsedEventArgs e)
    {
        HasWait = false;
        updateTimer.Elapsed -= WaitProcess;
    }
}
