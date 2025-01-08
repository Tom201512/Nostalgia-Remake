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

    // ���݂̃Q�[�����
    private StateManager stateManager;


    // �Q�[�����

    // ���_������
    private InsertState insertState;

    // ���I
    private LotsState lotsState;

    // ���[����](�v���C��)
    private PlayingState playingState;

    // ���_�������o��
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
