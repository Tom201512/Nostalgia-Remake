using System;

namespace ReelSpinGame_Input
{
    // 入力用インターフェース
    public interface IInputProvider
    {
        public event Action<InputManager.ControlKeys> InputTriggered;      // 何かしらの入力を受けたときのイベント
    }
}