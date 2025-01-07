using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
        public void StateStart()
        {
            Debug.Log("Start Payout State");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Payout State");
        }

        public void StateEnd()
        {
            Debug.Log("End Payout State");
        }
    }
}