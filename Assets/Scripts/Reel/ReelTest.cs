using ReelSpinGame_Medal.Payout;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class ReelTest : MonoBehaviour
{
    // リール処理のテスト用
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

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
        payoutChecker = new PayoutChecker();
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
                manager.StopSelectedReel(ReelManager.ReelID.ReelLeft, 3, 
                    ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagNone, ReelSpinGame_Bonus.BonusManager.BonusType.BonusNone);
            }
            // 中停止
            if (OriginalInput.CheckOneKeyInput(keyToStopMiddle) && manager.IsWorking)
            {
                manager.StopSelectedReel(ReelManager.ReelID.ReelMiddle, 3, 
                    ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagNone, ReelSpinGame_Bonus.BonusManager.BonusType.BonusNone);
            }
            // 右停止
            if (OriginalInput.CheckOneKeyInput(keyToStopRight) && manager.IsWorking)
            {
                manager.StopSelectedReel(ReelManager.ReelID.ReelRight, 3, 
                    ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagNone, ReelSpinGame_Bonus.BonusManager.BonusType.BonusNone);
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
                Debug.Log("Bonus:" + payoutChecker.LastPayoutResult.BonusID + "ReplayOrJac" + payoutChecker.LastPayoutResult.IsReplayOrJacIn);
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

    // 払い出しの確認
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