using ReelSpinGame_Bonus;
using ReelSpinGame_Util.OriginalInputs;
using System;
using UnityEngine;

public class ReelTest : MonoBehaviour
{
    // リール処理のテスト用
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    [SerializeField] private ReelManager manager;

    // イベント用テスト
    // 入力があったか
    bool hasInput;

    void Awake()
    {
        hasInput = false;
    }

    void Update()
    {
        // 何も入力が入っていなければ実行
        if(!hasInput)
        {
            // リール回転
            if (OriginalInput.CheckOneKeyInput(KeyCode.UpArrow) && 
                !manager.IsWorking && manager.HasFinishedCheck)
            {
                manager.StartReels();
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
            else if(manager.IsFinished && !manager.HasFinishedCheck)
            {
                Debug.Log("Start Payout Check");
                manager.StartCheckPayout(3);
                Debug.Log("Payouts result" + manager.LastPayoutResult.Payouts);
                Debug.Log("Bonus:" + manager.LastPayoutResult.BonusID + "ReplayOrJac" + manager.LastPayoutResult.IsReplayOrJAC);
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
} 