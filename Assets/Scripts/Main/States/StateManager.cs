using ReelSpinGame_Interface;

namespace ReelSpinGame_State
{
    // ステート管理マネージャー
    public class StateManager
    {
        // 現在のゲーム状態
        public IGameStatement CurrentState { get; private set; }

        public StateManager(IGameStatement startingState)
        {
            CurrentState = startingState;
        }

        // ステート開始
        public void StartState()
        {
            CurrentState.StateStart();
        }

        // 別のゲーム状態に移行
        public void ChangeState(IGameStatement nextState)
        {
            CurrentState.StateEnd();
            CurrentState = nextState;
            nextState.StateStart();
        }

        // ゲーム状態の更新
        public void StatementUpdate()
        {
            if (CurrentState != null) { CurrentState.StateUpdate(); }
        }
    }
}