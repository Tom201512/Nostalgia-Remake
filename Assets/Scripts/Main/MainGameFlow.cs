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
    public FirstLaunchState FirstLaunchState { get; private set; }      // 初回起動
    public ErrorState ErrorState { get; private set; }                  // エラー時
    public InsertState InsertState { get; private set; }                // メダル投入
    public LotsState LotsState { get; private set; }                    // 抽選
    public FakeReelSpinState FakeReelSpinState { get; private set; }    // 疑似遊技
    public WaitState WaitState { get; private set; }                    // ウェイトの状態
    public PlayingState PlayingState { get; private set; }              // リール回転(プレイ中)
    public PayoutState PayoutState { get; private set; }                // メダル払い出し
    public EffectState EffectState { get; private set; }                // 演出
    public ReachedLimitState LimitReachedState { get; private set; }   // 打ち止め時

    public MainGameFlow(GameManager gameManager)
    {
        InitState = new InitState(gameManager, gameManager.MedalManager);
        FirstLaunchState = new FirstLaunchState(gameManager);
        InsertState = new InsertState(gameManager, gameManager.InputManager, gameManager.MedalManager);
        LotsState = new LotsState(gameManager, gameManager.MedalManager);
        FakeReelSpinState = new FakeReelSpinState(gameManager);
        WaitState = new WaitState(gameManager);
        PlayingState = new PlayingState(gameManager, gameManager.InputManager, gameManager.MedalManager);
        PayoutState = new PayoutState(gameManager, gameManager.MedalManager);
        EffectState = new EffectState(gameManager, gameManager.MedalManager);
        ErrorState = new ErrorState(gameManager);
        LimitReachedState = new ReachedLimitState(gameManager, gameManager.MedalManager);
        StateManager = new StateManager(InitState);
    }

    // ステート更新
    public void UpdateState() => StateManager.StatementUpdate();
}
