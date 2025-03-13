using ReelSpinGame_Util.OriginalInputs;
using System.IO;
using UnityEngine;

public class ReelTest : MonoBehaviour
{
    // リール処理のテスト用
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    // 払い出し表のデータ
    [SerializeField] private string normalPayoutData;
    [SerializeField] private string bigPayoutData;
    [SerializeField] private string jacPayoutData;
    [SerializeField] private string payoutLineData;

    [SerializeField] private ReelManager manager;
    private PayoutChecker payoutChecker;

    // これはメインループで使用する
    // 払い出し判定完了したか
    public bool hasFinishedCheck { get; private set; }

    // イベント用テスト
    // 入力があったか
    bool hasInput;

    void Awake()
    {
        hasFinishedCheck = true;
        hasInput = false;

        // 払い出しラインの読み込み
        StreamReader payoutLines = new StreamReader(payoutLineData) ?? throw new System.Exception("PayoutLine file is missing");

        // 払い出しデータの読み込み
        StreamReader normalPayout = new StreamReader(normalPayoutData) ?? throw new System.Exception("NormalPayoutData file is missing");
        StreamReader bigPayout = new StreamReader(bigPayoutData) ?? throw new System.Exception("BigPayoutData file is missing");
        StreamReader jacPayout = new StreamReader(jacPayoutData) ?? throw new System.Exception("JacPayoutData file is missing");

        payoutChecker = new PayoutChecker(normalPayout, bigPayout, jacPayout, payoutLines, PayoutChecker.PayoutCheckMode.PayoutNormal);
    }

    void Update()
    {
        // 将来的にゲームマネージャーがリール制御のために
        // ベット枚数やフラグなどの条件をリールマネージャーへ渡してさらに各リールを止める

        // 何も入力が入っていなければ実行
        if(!hasInput)
        {
            // リール回転
            if (OriginalInput.CheckOneKeyInput(KeyCode.UpArrow) && 
                !manager.IsWorking && hasFinishedCheck)
            {
                manager.StartReels();
                hasFinishedCheck = false;
            }
            // 左停止
            if (OriginalInput.CheckOneKeyInput(keyToStopLeft) && manager.IsWorking)
            {
                manager.StopSelectedReel(ReelManager.ReelID.ReelLeft);
            }
            // 中停止
            if (OriginalInput.CheckOneKeyInput(keyToStopMiddle) && manager.IsWorking)
            {
                manager.StopSelectedReel(ReelManager.ReelID.ReelMiddle);
            }
            // 右停止
            if (OriginalInput.CheckOneKeyInput(keyToStopRight) && manager.IsWorking)
            {
                manager.StopSelectedReel(ReelManager.ReelID.ReelRight);
            }

            // 入力がないかチェック
            if (Input.anyKey)
            {
                hasInput = true;
                //Debug.Log("Input true");
            }
            // 入力がなくすべてのリールが止まっていたら払い出し処理をする
            else if(manager.IsFinished && !hasFinishedCheck)
            {
                Debug.Log("Start Payout Check");

                StartCheckPayout(3);
                Debug.Log("Payouts result" + payoutChecker.LastPayoutResult.Payouts);
                Debug.Log("Bonus:" + payoutChecker.LastPayoutResult.BonusID + "ReplayOrJac" + payoutChecker.LastPayoutResult.IsReplayOrJAC);
            }
        }

        // 入力がある場合は離れたときの制御を行う
        else if (hasInput)
        {
            //Debug.Log("Input still");
            if(!Input.anyKey)
            {
                //Debug.Log("input end");
                hasInput = false;
            }
        }
    }

    private void StartCheckPayout(int betAmounts)
    {
        if (!manager.IsWorking)
        {
            payoutChecker.CheckPayoutLines(betAmounts, manager.LastSymbols);
            hasFinishedCheck = true;
        }
        else
        {
            Debug.Log("Failed to check payout because reels are spinning");
        }
    }
} 