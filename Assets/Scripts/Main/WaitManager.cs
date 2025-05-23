using UnityEngine;
using System.Threading.Tasks;

public class WaitManager
{
    // ウェイト管理

    // const
    // ウェイトに必要な時間(ミリ秒)
    public const int WaitTimerSetting = 4100;

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }

    // var
    // ウェイトが有効か
    public bool hasWait { get; private set; }
    // ウェイトを無効にしているか
    public bool hasWaitCut { get; private set; }

    // コンストラクタ
    public WaitManager(bool hasWaitCut)
    {
        // ウェイトカット設定
        this.hasWaitCut = hasWaitCut;
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
            Task.Run(ActivateWaitTimer);
        }
    }

    async Task ActivateWaitTimer()
    {
        try
        {
            Debug.Log("Wait start");
            hasWait = true;
            await Task.Delay(WaitTimerSetting);
            hasWait = false;
            Debug.Log("Wait disabled");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error:" + ex);
        }
    }
}
