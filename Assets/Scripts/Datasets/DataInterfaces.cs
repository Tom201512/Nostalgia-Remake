namespace ReelSpinGame_Interface
{
    // ゲーム状態のインターフェース
    public interface IGameStatement
    {
        public MainGameFlow.GameStates State { get; }

        void StateStart();
        void StateUpdate();
        void StateEnd();
    }

    public interface ISave
    {
        void SaveFile();
        void LoadSave();
    }
}