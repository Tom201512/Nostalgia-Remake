using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �Q�[���̊Ǘ�

    // const

    // �e�푀��̃V���A���C�Y
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var

    // �e��@�\

    public MedalManager Medal { get; private set; }
    //public FlagLots Lots { get; private set; }
    //public ReelManager

    [SerializeField] MedalTestUI medalUI;

    // �L�[�ݒ�

    // MAXBET
    [SerializeField] private KeyCode maxBetKey;
    // 1BET
    [SerializeField] private KeyCode betOneKey;
    //2BET
    [SerializeField] private KeyCode betTwoKey;
    // ���[���n��(�܂���MAX BET)
    [SerializeField] private KeyCode startAndMaxBetKey;

    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    public KeyCode[] KeyCodes { get; private set; }

    // �Q�[���X�e�[�g�p
    public MainGameFlow MainFlow { get; private set; }

    void Awake()
    {
        MainFlow = new MainGameFlow(this);
        Medal = new MedalManager(0, MedalManager.MaxBet);
        Debug.Log("Medal is launched");
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        medalUI.SetMedalManager(Medal);
    }

    void Start()
    {
        
    }

    void Update()
    {
        MainFlow.UpdateState();
    }

    // func

    // �L�[�ݒ�ύX
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}
