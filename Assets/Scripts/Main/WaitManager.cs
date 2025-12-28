using UnityEngine;

// ウェイト管理
public class WaitManager : MonoBehaviour
{
    public const float WaitTime = 4.1f;     // ウェイトに必要な時間(ミリ秒)

    public bool HasWait { get; private set; }       // ウェイトが有効か
    public bool HasWaitCut { get; private set; }    // ウェイトを無効にしているか

    private void Awake()
    {
        HasWait = false;
        HasWaitCut = false;
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
            Invoke("DisableWaitTimer", WaitTime);
        }
    }

    // ウェイト状態を強制解除する(オート用)
    public void DisableWaitTimer()
    {
        if (HasWait)
        {
            HasWait = false;
        }
    }
}
