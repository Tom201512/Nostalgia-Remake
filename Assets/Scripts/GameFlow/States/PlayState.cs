using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.PlayingState
{
    public class PlayingState : IGameStatement
    {
        public void StateStart()
        {
            Debug.Log("Start Playing State");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Playing State");
        }

        public void StateEnd()
        {
            Debug.Log("End Playing State");
        }
    }
}