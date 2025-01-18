using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class MedalTest : MonoBehaviour
{
    // メダル処理のテスト用

    private MedalManager medalManager;

    // Start is called before the first frame update
    void Awake()
    {
        medalManager = new MedalManager(0, MedalManager.MaxBet);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // MAX BET
        if(OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            medalManager.StartBet(3);
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

        if (OriginalInput.CheckOneKeyInput(KeyCode.P))
        {
            //medalManager.BetMedals(MedalManager.MAX_BET);
            medalManager?.StartPayout(15);
        }
    }
}
