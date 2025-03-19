using ReelSpinGame_Medal;
using ReelSpinGame_Lots.Flag;
using UnityEngine;
using System.IO;

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

    // 確率設定
    [SerializeField] private int setting;
    // 低確率時
    [SerializeField] private string flagTableAPath;
    // 高確率時
    [SerializeField] private string flagTableBPath;
    // BIG中テーブル
    [SerializeField] private string flagTableBIGPath;
    // JACはずれ確率
    [SerializeField] private int jacNoneProb;

    // 払い出し表のデータ
    [SerializeField] private string normalPayoutPath;
    [SerializeField] private string bigPayoutPath;
    [SerializeField] private string jacPayoutPath;
    [SerializeField] private string payoutLinePath;

    public KeyCode[] KeyCodes { get; private set; }

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }

    void Awake()
    {
        // メダル管理
        Medal = new MedalManager(0, MedalManager.MaxBet);
        Debug.Log("Medal is launched");

        // フラグ管理
        Lots = new FlagLots(setting, flagTableAPath, flagTableBPath, flagTableBIGPath, jacNoneProb);
        Debug.Log("Lots is launched");

        // ウェイト管理
        Wait = new WaitManager(false);

        // リール
        Reel = reelManagerObj.GetComponent<ReelManager>();

        // 払い出し処理
        Payout = new PayoutChecker(normalPayoutPath, bigPayoutPath, jacPayoutPath, payoutLinePath, PayoutChecker.PayoutCheckMode.PayoutNormal);

        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // メインフロー作成
        MainFlow = new MainGameFlow(this);
    }

    void Start()
    {
        medalUI.SetMedalManager(Medal);
    }

    void Update()
    {
        MainFlow.UpdateState();
    }

    // func

    // キー設定変更
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}
