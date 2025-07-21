using System.Collections.Generic;
using System.IO;

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

    public interface ISavable
    {
        List<int> SaveData();
        bool LoadData(BinaryReader bStream);
    }
}