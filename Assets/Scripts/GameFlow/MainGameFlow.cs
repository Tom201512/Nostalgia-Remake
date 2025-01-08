using UnityEngine;
using ReelSpinGame_State;
using ReelSpinGame_State.InsertState;
using ReelSpinGame_State.LotsState;
using ReelSpinGame_State.PlayingState;
using ReelSpinGame_State.PayoutState;

public class MainGameFlow : MonoBehaviour
{
    delegate void EventHandler();
    // var

    // 現在のゲーム状態
    private StateManager stateManager;


    // ゲーム状態

    // メダル投入
    private InsertState insertState;

    // 抽選
    private LotsState lotsState;

    // リール回転(プレイ中)
    private PlayingState playingState;

    // メダル払い出し
    private PayoutState payoutState;


    // func
    void Awake()
    {
        insertState = new InsertState();
        lotsState = new LotsState();
        playingState = new PlayingState();
        payoutState = new PayoutState();

        stateManager = new StateManager(insertState);
    }

    void Start()
    {
        Invoke("ChangeStateB", 5.0f);
    }

    void Update()
    {
        stateManager.StatementUpdate();
    }

    void ChangeStateA()
    {
        stateManager.ChangeState(insertState);
        Invoke("ChangeStateB", 5.0f);
    }

    void ChangeStateB()
    {
        stateManager.ChangeState(lotsState);
        Invoke("ChangeStateA", 5.0f);
    }
}
