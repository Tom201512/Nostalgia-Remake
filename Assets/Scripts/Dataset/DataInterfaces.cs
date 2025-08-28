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


    // セーブできるデータ
    public interface ISavable
    {
        List<int> SaveData();
        bool LoadData(BinaryReader br);
    }

    // セーブ機能を持つ
    public interface IHasSave
    {
        ISavable MakeSaveData();
        void LoadSaveData(ISavable save);
    }
}