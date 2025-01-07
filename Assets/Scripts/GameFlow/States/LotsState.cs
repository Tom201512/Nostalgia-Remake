using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.LotsState
{
    public class LotsState : IGameStatement
    {
        public void StateStart()
        {
            Debug.Log("Start Lots State");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Lots State");
        }

        public void StateEnd()
        {
            Debug.Log("End Lots State");
        }
    }
}