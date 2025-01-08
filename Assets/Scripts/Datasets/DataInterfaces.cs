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

    // イベント情報を持つ
    public interface ISubject
    {
        // イベント
        public event Action ThingHappened;

        // 何かをする
        public void DoThing();
    }
}