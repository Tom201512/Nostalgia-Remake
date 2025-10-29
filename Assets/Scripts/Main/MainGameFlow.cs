using ReelSpinGame_State;
using ReelSpinGame_State.InsertState;
using ReelSpinGame_State.LotsState;
using ReelSpinGame_State.PayoutState;
using ReelSpinGame_State.PlayingState;
using System.Timers;

public class MainGameFlow
{
    // �Q�[����Ԃ̏���

    // const

    //�Q�[����ԃV���A���C�Y
    public enum GameStates { Init, Insert, FlagLots, FakeReel, Wait, Playing, Payout, Effect}
    // var
    // ���݂̃Q�[�����
    public StateManager stateManager { get; private set; }

    // �Q�[�����
    // ���������
    public InitState InitState { get; private set; }
    // ���_������
    public InsertState InsertState { get; private set; }
    // ���I
    public LotsState LotsState { get; private set; }
    // �^���V�Z
    public FakeReelSpinState FakeReelSpinState { get; private set; }
    // �E�F�C�g�̏��
    public WaitState WaitState { get; private set; }
    // ���[����](�v���C��)
    public PlayingState PlayingState { get; private set; }
    // ���_�������o��
    public PayoutState PayoutState { get; private set; }
    // ���o
    public EffectState EffectState { get; private set; }

    // �R���X�g���N�^
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
