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
    public event Action BetMax;
    public event Action<int> StartPayout;

    // Start is called before the first frame update
    void Awake()
    {
        medalManager = new MedalManager(0, MedalManager.MAX_BET, this);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            BetMax?.Invoke();
        }

        if (OriginalInput.CheckOneKeyInput(KeyCode.P))
        {
            //medalManager.BetMedals(MedalManager.MAX_BET);
            StartPayout?.Invoke(15);
        }
    }
}
