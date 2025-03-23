using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_Medal.Payout;
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
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get; private set; }
    public PayoutChecker Payout { get; private set; }

    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;

    // キー設定

    // MAXBET
    [SerializeField] private KeyCode maxBetKey;
    // 1BET
    [SerializeField] private KeyCode betOneKey;
    //2BET
    [SerializeField] private KeyCode betTwoKey;
    // リール始動(またはMAX BET)
    [SerializeField] private KeyCode startAndMaxBetKey;
    // リール停止
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    // 設定
    [SerializeField] private int setting;

    public KeyCode[] KeyCodes { get; private set; }

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }

    void Awake()
    {
        // メダル管理
        Medal = new MedalManager(0, MedalManager.MaxBet);
        Debug.Log("Medal is launched");

        // フラグ管理
        Lots = new FlagLots(setting);
        Debug.Log("Lots is launched");

        // ウェイト管理
        Wait = new WaitManager(false);
        Debug.Log("Wait is launched");

        // リール
        Reel = reelManagerObj.GetComponent<ReelManager>();
        Debug.Log("Reel is launched");

        // 払い出し処理
        Payout = new PayoutChecker(PayoutChecker.PayoutCheckMode.PayoutNormal);
        Debug.Log("Payout is launched");

        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // メインフロー作成
        MainFlow = new MainGameFlow(this);
    }

    void Start()
    {
        medalUI.SetMedalManager(Medal);
        lotsUI.SetFlagManager(Lots);
        waitUI.SetWaitManager(Wait);
        reelUI.SetReelManager(Reel);
    }

    void Update()
    {
        MainFlow.UpdateState();
    }

    // func
    // キー設定変更
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}
