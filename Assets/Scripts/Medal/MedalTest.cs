using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class MedalTest : MonoBehaviour
{
    // メダル処理のテスト用

    private MedalManager medalManager;

    void Awake()
    {
        medalManager = new MedalManager(0, MedalManager.MaxBet);
    }

    void Start()
    {
        
    }

    void Update()
    {

        // MAX BET
        if(OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            medalManager.StartMAXBet();
        }

        // BET2
        if (OriginalInput.CheckOneKeyInput(KeyCode.Alpha2))
        {
            medalManager.StartBet(2);
        }

        // BET1
        if (OriginalInput.CheckOneKeyInput(KeyCode.Alpha1))
        {
            medalManager?.StartBet(1);
        }

        // PayoutTEST
        if (OriginalInput.CheckOneKeyInput(KeyCode.P))
        {
            //medalManager?.StartPayout(15);
            medalManager?.ChangeMaxBet(1);
        }
    }
}
