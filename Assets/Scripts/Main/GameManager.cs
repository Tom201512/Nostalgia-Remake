using ReelSpinGame_Bonus;
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
    public BonusManager Bonus { get; private set; }

    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;
    [SerializeField] BonusTestUI bonusUI;

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
    [Range(0,6), SerializeField] private int setting;

    public KeyCode[] KeyCodes { get; private set; }

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }
    public int Setting { get { return setting; } }

    void Awake()
    {
        // 画面
        Debug.Log("Screen:" + Screen.width + "," + Screen.height);
        Screen.SetResolution(960, 540, false);

        // メダル管理
        Medal = new MedalManager(0, MedalManager.MaxBet, 0, false);
        Debug.Log("Medal is launched");

        // フラグ管理
        Lots = GetComponent<FlagLots>();
        Debug.Log("Lots is launched");

        // ウェイト管理
        Wait = new WaitManager(false);
        Debug.Log("Wait is launched");

        // リール
        Reel = reelManagerObj.GetComponent<ReelManager>();
        Debug.Log("Reel is launched");

        // 払い出し処理
        Payout = GetComponent<PayoutChecker>();
        Debug.Log("Payout is launched");

        // ボーナス
        Bonus = new BonusManager();

        // キーボードのコード設定
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // メインフロー作成
        MainFlow = new MainGameFlow(this);

        // 例外処理
        if (setting < 0 && setting > 6) { throw new System.Exception("Invalid Setting, must be within 0~6"); }
        // 0ならランダムを選ぶ
        else if (setting == 0)
        {
            setting = Random.Range(1, 6);
        }

        Debug.Log("Setting:" + setting);
    }

    void Start()
    {
        medalUI.SetMedalManager(Medal);
        lotsUI.SetFlagManager(Lots);
        waitUI.SetWaitManager(Wait);
        reelUI.SetReelManager(Reel);
        bonusUI.SetBonusManager(Bonus);
    }

    void Update()
    {
        // 画面サイズ調整

        // 終了処理
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Game Closed");
            Application.Quit();
        }

        MainFlow.UpdateState();
    }

    // func
    // キー設定変更
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}
