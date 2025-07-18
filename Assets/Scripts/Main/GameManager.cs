using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_System;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;

public class GameManager : MonoBehaviour
{
    // ゲームの管理
    // const
    // 各種操作のシリアライズ
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var

    // メインカメラ
    [SerializeField] private SlotCamera camera;

    // 各種機能
    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] private EffectManager effectManagerObj;

    [SerializeField] PlayerUI playerUI;

    // デバッグ用
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;
    [SerializeField] BonusTestUI bonusUI;
    [SerializeField] AutoTestUI AutoUI;


    public MedalManager Medal { get; private set; }
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get { return reelManagerObj; } }
    public BonusManager Bonus { get; private set; }
    public EffectManager Effect { get { return effectManagerObj; } }

    // オートプレイ機能
    public AutoPlayFunction Auto { get; private set; }

    // セーブ機能
    private SaveManager save;

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

    // オート開始/停止ボタン
    [SerializeField] private KeyCode keyToAutoToggle;

    // <デバッグ用> デバッグUI表示用
    [SerializeField] private KeyCode keyToDebugToggle;

    // カメラの視点変更
    [SerializeField] private KeyCode keyCameraModeChange;

    // <デバッグ用>セーブの読み込み、書き込みをするか
    [SerializeField] private bool useSaveFunciton;

    // <デバッグ用> デバッグUI表示するか
    private bool hasDebugUI;

    // 設定値
    public int Setting { get; private set; }
    // デバッグ用設定値
    [Range(0, 6), SerializeField] private int debugSetting;

    // 入力用キーコード
    public KeyCode[] KeyCodes { get; private set; }

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }


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
        // フラグ管理
        Lots = GetComponent<FlagLots>();
        // ウェイト管理
        Wait = new WaitManager(false);
        // ボーナス
        Bonus = GetComponent<BonusManager>();
        // メインフロー作成
        MainFlow = new MainGameFlow(this);
        // ステータスパネル
        Status = statusPanel;

        // オート機能
        Auto = new AutoPlayFunction(AutoPlaySpeed.Normal, AutoStopOrderOptions.LMR);
        // セーブ機能
        save = new SaveManager();

        // キーボードのコード設定
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // 例外処理
        if (Setting < 0 && Setting > 6) { throw new System.Exception("Invalid Setting, must be within 0~6"); }
        // 0ならランダムを選ぶ
        else if (Setting == 0)
        {
            Setting = Random.Range(1, 6);
        }

        // 画面サイズ初期化
        Screen.SetResolution(1600, 900, false);

        // FPS固定
        Application.targetFrameRate = 60;

        // デバッグUIの表示
        hasDebugUI = false;
    }

    void Start()
    {
        if(useSaveFunciton)
        {
            // セーブ読み込み。セーブがない場合は新規作成
            if (!save.LoadSaveFile())
            {
                // セーブフォルダの作成
                save.GenerateSaveFolder();
            }
        }

        // メダル設定
        Medal.SetMedalData(0, 3, 0, false);

        // UI 設定
        waitUI.SetWaitManager(Wait);
        playerUI.SetPlayerData(PlayerData);
        playerUI.SetMedalManager(Medal);
        AutoUI.SetAutoFunction(Auto);

        // ステート開始
        MainFlow.stateManager.StartState();

        // デバッグをすべて非表示
        ToggleDebugUI(false);

        // 設定反映

        if(useSaveFunciton)
        {
            save.CurrentSave.SetSetting(debugSetting);
            Setting = save.CurrentSave.Setting;
            Debug.Log("Setting:" + save.CurrentSave.Setting);
        }
        else
        {
            Setting = debugSetting;
            Debug.Log("Setting:" + debugSetting);
        }
    }

    void Update()
    {
        // 画面サイズ調整

        // 終了処理
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // オートプレイ機能ボタン
        if (Input.GetKeyDown(keyToAutoToggle))
        {
            Auto.ChangeAutoMode();
        }

        // カメラ表示方法変更
        if (Input.GetKeyDown(keyCameraModeChange))
        {
            camera.ChangeCameraMode();
        }

        // デバッグ表示
        if (Input.GetKeyDown(keyToDebugToggle))
        {
            DebugButtonBehavior();
        }

        MainFlow.UpdateState();
    }

    // 終了時の処理
    private void OnDestroy()
    {
        Wait.DisposeWait();

        if (useSaveFunciton)
        {
            // セーブ開始
            save.GenerateSaveFolder();
            // ファイルセーブを行う
            save.GenerateSaveFile();
        }
    }

    // func
    // キー設定変更
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;

    // デバッグをつける機能
    private void DebugButtonBehavior()
    {
        hasDebugUI = !hasDebugUI;
        ToggleDebugUI(hasDebugUI);
    }

    // デバッグUIの表示非表示
    private void ToggleDebugUI(bool value)
    {
        medalUI.ToggleUI(value);
        lotsUI.ToggleUI(value);
        waitUI.ToggleUI(value);
        reelUI.ToggleUI(value);
        bonusUI.ToggleUI(value);
    }
}
