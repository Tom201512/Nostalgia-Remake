using UnityEngine;

public class WaitManager : MonoBehaviour
{
    // ウェイト管理

    // const
    // ウェイトに必要な時間(ミリ秒)
    public const float WaitTime = 4.1f;
    // var
    // ウェイトが有効か
    public bool HasWait { get; private set; }
    // ウェイトを無効にしているか
    public bool HasWaitCut { get; private set; }

    void Awake()
    {
        HasWait = false;
        HasWaitCut = false;
    }

    // func
    // ウェイトカットの設定
    public void SetWaitCutSetting(bool hasWaitCut) => HasWaitCut = hasWaitCut;

    // ウェイトをかける
    public void SetWaitTimer()
    {
        // ウェイトカット、または実行中のウェイトがなければ実行
        if (!HasWaitCut && !HasWait)
        {
            HasWait = true;
            Invoke("DisableWaitTimer", WaitTime);
        }
    }

    // ウェイト状態を強制解除する(オート用)
    public void DisableWaitTimer()
    {
        if(HasWait)
        {
            HasWait = false;
        }
    }
}
