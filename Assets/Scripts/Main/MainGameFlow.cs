using ReelSpinGame_State;
using ReelSpinGame_State.InsertState;
using ReelSpinGame_State.LotsState;
using ReelSpinGame_State.PayoutState;
using ReelSpinGame_State.PlayingState;
using System.Timers;

public class MainGameFlow
{
    // ゲーム状態の処理

    // const

    //ゲーム状態シリアライズ
    public enum GameStates { None, Insert, FlagLots, Wait, Playing, Payout}
    // var
    // 処理を止める用のタイマー
    private Timer flowStopTimer;

    // 現在のゲーム状態
    public StateManager stateManager { get; private set; }

    // ゲーム状態
    // メダル投入
    public InsertState insertState { get; private set; }
    // 抽選
    public LotsState lotsState { get; private set; }
    // ウェイトの状態
    public WaitState waitState { get; private set; }
    // リール回転(プレイ中)
    public PlayingState playingState { get; private set; }
    // メダル払い出し
    public PayoutState payoutState { get; private set; }

    // コンストラクタ
    public MainGameFlow(GameManager gameManager)
    {
        // 処理用タイマー作成
        flowStopTimer = new Timer();

        insertState = new InsertState(gameManager);
        lotsState = new LotsState(gameManager);
        waitState = new WaitState(gameManager);
        playingState = new PlayingState(gameManager);
        payoutState = new PayoutState(gameManager);

        stateManager = new StateManager(insertState);
    }

    // デストラクタ
    ~MainGameFlow()
    {
        // Timerのストップ
        flowStopTimer.Stop();
        flowStopTimer.Dispose();
    }

    // func
    public void UpdateState() => stateManager.StatementUpdate();
}
