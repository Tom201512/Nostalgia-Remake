using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;
using System;
using UnityEngine;

public class MedalTest : MonoBehaviour
{
    // ���_�������̃e�X�g�p

    private MedalManager medalManager;


    // �C�x���g
    public event Action<int> betMedal;
    public event Action betMax;

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
            //medalManager.BetMedals(MedalManager.MAX_BET);
            betMax?.Invoke();
        }
    }
}
