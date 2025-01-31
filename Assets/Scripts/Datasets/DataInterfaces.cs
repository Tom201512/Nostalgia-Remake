namespace ReelSpinGame_Interface
{
    // ゲーム状態のインターフェース
    public interface IGameStatement
    {
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