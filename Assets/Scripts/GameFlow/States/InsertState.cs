using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.InsertState
{
    public class InsertState : IGameStatement
    {
        public void StateStart()
        {
            Debug.Log("Start Medal Insert");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Medal Insert ");
        }

        public void StateEnd()
        {
            Debug.Log("End Medal Insert");
        }
    }
}