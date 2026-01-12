using ReelSpinGame_State;
using ReelSpinGame_State.InsertState;
using ReelSpinGame_State.LotsState;
using ReelSpinGame_State.PayoutState;
using ReelSpinGame_State.PlayingState;

// ゲーム状態の処理
public class MainGameFlow
{
    public StateManager StateManager { get; private set; }    // 現在のゲーム状態

    // ゲーム状態
    public InitState InitState { get; private set; }                    // 初期化状態
    public InsertState InsertState { get; private set; }                // メダル投入
    public LotsState LotsState { get; private set; }                    // 抽選
    public FakeReelSpinState FakeReelSpinState { get; private set; }    // 疑似遊技
    public WaitState WaitState { get; private set; }                    // ウェイトの状態
    public PlayingState PlayingState { get; private set; }              // リール回転(プレイ中)
    public PayoutState PayoutState { get; private set; }                // メダル払い出し
    public EffectState EffectState { get; private set; }                // 演出
    public ErrorState ErrorState { get; private set; }                  // エラー時
    public ReachedLimitState LimitReachedState {  get; private set; }   // 打ち止め時

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
        ErrorState = new ErrorState(gameManager);
        LimitReachedState = new ReachedLimitState(gameManager);
        StateManager = new StateManager(InitState);
    }

    // ステート更新
    public void UpdateState() => StateManager.StatementUpdate();
}
