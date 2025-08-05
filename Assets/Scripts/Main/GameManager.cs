using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_System;
using ReelSpinGame_Save.Database;
using ReelSpinGame_UI.Player;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ゲームの管理
    // const
    // 最高設定値
    public const int MaxSlotSetting = 6;

    // 各種操作のシリアライズ
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight }

    // var
    // メインカメラ
    [SerializeField] private SlotCamera slotCam;

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
    [SerializeField] AutoTestUI autoUI;

    // 各種機能
    public MedalManager Medal { get; private set; }
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get { return reelManagerObj; } }
    public BonusManager Bonus { get; private set; }
    public EffectManager Effect { get { return effectManagerObj; } }

    // プレイヤー情報
    public PlayerDatabase Player;

    // プレイヤーUI
    public PlayerUI PlayerUI { get { return playerUI; } }

    // オートプレイ機能
    public AutoPlayFunction Auto { get; private set; }

    // セーブ機能
    private SaveManager saveManager;

    // セーブデータベース
    public SaveDatabase Save { get { return saveManager.CurrentSave; } }

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

    // <デバッグ用> オートの押し順設定
    [SerializeField] private KeyCode keyToAutoOrderChange;

    // <デバッグ用> オートスピード変更
    [SerializeField] private KeyCode keyToAutoSpeedChange;

    // <デバッグ用> ウェイトカットの有無設定
    [SerializeField] private KeyCode keyToWaitCutToggle;

    // <デバッグ用>セーブを消去するか
    [SerializeField] private bool deleteSave;

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

    void Awake()
    {
        // 画面サイズ初期化
        Screen.SetResolution(1600, 900, false);

        // FPS固定
        Application.targetFrameRate = 60;

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
        // プレイヤー情報
        Player = new PlayerDatabase();
        // オート機能
        Auto = new AutoPlayFunction();
        // セーブ機能
        saveManager = new SaveManager();

        // キーボードのコード設定
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey, betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight };

        // デバッグUIの表示
        hasDebugUI = false;
    }

    void Start()
    {
        // セーブ読み込み。セーブがない場合は新規作成
        // セーブフォルダの作成
        saveManager.GenerateSaveFolder();

        if (deleteSave)
        {
            // セーブ削除
            saveManager.DeleteSaveFile();

            // 設定値の作成
            Save.RecordSlotSetting(debugSetting);
            deleteSave = false;
        }

        // 古いファイルがあるなら読み込んで削除
        else if (!saveManager.LoadOldSaveFile())
        {
            if (!saveManager.LoadSaveFileWithDecryption())
            {
                // 設定値の作成
                Save.RecordSlotSetting(debugSetting);
                Debug.Log("Save is newly generated");
            }
        }

        // UI 設定
        waitUI.SetWaitManager(Wait);
        autoUI.SetAutoFunction(Auto);

        // デバッグをすべて非表示
        ToggleDebugUI(false);

        // ステート開始
        MainFlow.stateManager.StartState();
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
            Auto.ChangeAutoMode(AutoEndConditionID.AnyBonus, 0, true, true);
        }

        // オート押し順変更
        if (Input.GetKeyDown(keyToAutoOrderChange))
        {
            Auto.ChangeAutoOrder();
        }

        // オートスピード変更
        if(Input.GetKeyDown(keyToAutoSpeedChange))
        {
            Auto.ChangeAutoSpeed();
        }

        // ウェイトカット
        if (Input.GetKeyDown(keyToWaitCutToggle))
        {
            Wait.SetWaitCutSetting(!Wait.HasWaitCut);
        }

        // カメラ表示方法変更
        if (Input.GetKeyDown(keyCameraModeChange))
        {
            slotCam.ChangeCameraMode();
        }

        // デバッグ表示
        if (Input.GetKeyDown(keyToDebugToggle))
        {
            DebugButtonBehavior();
        }

        MainFlow.UpdateState();
    }

    private void OnApplicationQuit()
    {
        // ウェイトタイマーの放棄
        Wait.DisposeWaitTimer();
        // セーブ開始
        saveManager.GenerateSaveFolder();
        saveManager.GenerateSaveFileWithEncrypt();
    }

    // func
    // 設定値変更
    public void ChangeSetting(int setting)
    {
        // 例外処理
        if (setting < 0 && setting > 6) 
        { 
            throw new System.Exception("Invalid Setting, must be within 0~6");
        }
        // 0ならランダムを選ぶ
        else if (setting == 0)
        {
            Setting = Random.Range(1, MaxSlotSetting + 1);
        }
        else
        {
            Setting = setting;
        }
    }

    // デバッグをつける機能(デバッグ用)
    private void DebugButtonBehavior()
    {
        hasDebugUI = !hasDebugUI;
        ToggleDebugUI(hasDebugUI);
    }

    // デバッグUIの表示非表示(デバッグ用)
    private void ToggleDebugUI(bool value)
    {
        autoUI.ToggleUI(value);
        medalUI.ToggleUI(value);
        lotsUI.ToggleUI(value);
        waitUI.ToggleUI(value);
        reelUI.ToggleUI(value);
        bonusUI.ToggleUI(value);
    }
}
