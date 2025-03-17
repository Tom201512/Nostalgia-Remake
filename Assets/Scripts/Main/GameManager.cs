using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ゲームの管理

    // const

    // 各種操作のシリアライズ
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var

    // 各種機能

    public MedalManager Medal { get; private set; }
    //public FlagLots Lots { get; private set; }
    //public ReelManager

    [SerializeField] MedalTestUI medalUI;

    // キー設定

    // MAXBET
    [SerializeField] private KeyCode maxBetKey;
    // 1BET
    [SerializeField] private KeyCode betOneKey;
    //2BET
    [SerializeField] private KeyCode betTwoKey;
    // リール始動(またはMAX BET)
    [SerializeField] private KeyCode startAndMaxBetKey;

    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    public KeyCode[] KeyCodes { get; private set; }

    // ゲームステート用
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

    // キー設定変更
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}
