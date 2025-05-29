using ReelSpinGame_Bonus;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_Sound;
using ReelSpinGame_System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ゲームの管理
    // const
    // 各種操作のシリアライズ
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var
    // 各種機能
    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] private SoundManager soundManagerObj;
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;
    [SerializeField] BonusTestUI bonusUI;
    [SerializeField] PlayerUI playerUI;

    public MedalManager Medal { get; private set; }
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get { return reelManagerObj; } }
    public BonusManager Bonus { get; private set; }
    public SoundManager Sound { get { return soundManagerObj; } }

    [SerializeField] StatusPanel statusPanel;
    public StatusPanel Status { get; private set; }

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
    // 入力用キーコード
    public KeyCode[] KeyCodes { get; private set; }

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }
    public int Setting { get { return setting; } }

    // プレイヤーデータ用
    public PlayingDatabase PlayerData { get; private set; }

    void Awake()
    {
        // プレイヤーデータ作成
        PlayerData = new PlayingDatabase();

        // 画面
        //Debug.Log("Screen:" + Screen.width + "," + Screen.height);
        Screen.SetResolution(960, 540, false);

        // メダル管理
        Medal = GetComponent<MedalManager>();
        ////Debug.Log("Medal is launched");

        // フラグ管理
        Lots = GetComponent<FlagLots>();
        ////Debug.Log("Lots is launched");

        // ウェイト管理
        Wait = new WaitManager(false);
        ////Debug.Log("Wait is launched");

        // ボーナス
        Bonus = GetComponent<BonusManager>();
        ////Debug.Log("Bonus is launched");

        // メインフロー作成
        MainFlow = new MainGameFlow(this);

        // ステータスパネル
        Status = statusPanel;
        ////Debug.Log("StatusPanel is launched");

        // キーボードのコード設定
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // 例外処理
        if (setting < 0 && setting > 6) { throw new System.Exception("Invalid Setting, must be within 0~6"); }
        // 0ならランダムを選ぶ
        else if (setting == 0)
        {
            setting = Random.Range(1, 6);
        }
        ////Debug.Log("Setting:" + setting);

        // 画面サイズ初期化
        Screen.SetResolution(1600, 900, false);

        // FPS固定
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        // メダル設定
        Medal.SetMedalData(0, 3, 0, false);

        // UI 設定
        waitUI.SetWaitManager(Wait);
        playerUI.SetPlayerData(PlayerData);

        // ステート開始
        MainFlow.stateManager.StartState();
    }

    void Update()
    {
        // 画面サイズ調整

        // 終了処理
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Game Closed");
            Application.Quit();
        }

        MainFlow.UpdateState();
    }

    // タイマーを持つ機能の廃棄
    private void OnDestroy()
    {
        Wait.DisposeWait();
    }

    // func
    // キー設定変更
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}
