using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;
using System;
using UnityEngine;

public class MedalTest : MonoBehaviour
{
    // メダル処理のテスト用

    private MedalManager medalManager;

    // イベント
    public event Action<int> BetMedal;
    public event Action<int> StartPayout;

    // Start is called before the first frame update
    void Awake()
    {
        medalManager = new MedalManager(0, MedalManager.MaxBet, this);
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
            BetMedal?.Invoke(3);
        }

        // BET2
        if (OriginalInput.CheckOneKeyInput(KeyCode.Alpha2))
        {
            BetMedal?.Invoke(2);
        }


        // BET1
        if (OriginalInput.CheckOneKeyInput(KeyCode.Alpha1))
        {
            BetMedal?.Invoke(1);
        }

        if (OriginalInput.CheckOneKeyInput(KeyCode.P))
        {
            //medalManager.BetMedals(MedalManager.MAX_BET);
            StartPayout?.Invoke(15);
        }
    }
}
