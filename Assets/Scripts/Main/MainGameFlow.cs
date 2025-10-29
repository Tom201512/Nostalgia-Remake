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
    public enum GameStates { Init, Insert, FlagLots, FakeReel, Wait, Playing, Payout, Effect}
    // var
    // 現在のゲーム状態
    public StateManager stateManager { get; private set; }

    // ゲーム状態
    // 初期化状態
    public InitState InitState { get; private set; }
    // メダル投入
    public InsertState InsertState { get; private set; }
    // 抽選
    public LotsState LotsState { get; private set; }
    // 疑似遊技
    public FakeReelSpinState FakeReelSpinState { get; private set; }
    // ウェイトの状態
    public WaitState WaitState { get; private set; }
    // リール回転(プレイ中)
    public PlayingState PlayingState { get; private set; }
    // メダル払い出し
    public PayoutState PayoutState { get; private set; }
    // 演出
    public EffectState EffectState { get; private set; }

    // コンストラクタ
    public MainGameFlow(GameManager gameManager)
    {
        InitState = new InitState(gameManager);
        InsertState = new InsertState(gameManager);
        LotsState = new LotsState(gameManager);
        FakeReelSpinState = new FakeReelSpinState(gameManager);
        WaitState = new WaitState(gameManager);
        PlayingState = new PlayingState(gameManager);
        PayoutState = new PayoutState(gameManager);
        EffectState = new EffectState(gameManager);

        stateManager = new StateManager(InitState);
    }

    // func
    public void UpdateState() => stateManager.StatementUpdate();
}
