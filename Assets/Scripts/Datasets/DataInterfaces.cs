using System;

namespace ReelSpinGame_Interface
{
    // ゲーム状態のインターフェース
    public interface IGameStatement
    {
        void StateStart();
        void StateUpdate();
        void StateEnd();
    }

    // ファイル読み込み機能を持つインターフェース
    public interface IFileRead
    {
        void ReadFile(string path);
    }

    // ファイル読み込み機能を持つインターフェース
    public interface IFileWrite
    {
        void WriteFile();
    }
}