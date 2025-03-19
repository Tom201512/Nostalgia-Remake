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
    public enum GameStates { None, Insert, FlagLots, Wait, Playing, Payout}
    // var
    // �������~�߂�p�̃^�C�}�[
    private Timer flowStopTimer;

    // ���݂̃Q�[�����
    public StateManager stateManager { get; private set; }

    // �Q�[�����
    // ���_������
    public InsertState InsertState { get; private set; }
    // ���I
    public LotsState LotsState { get; private set; }
    // �E�F�C�g�̏��
    public WaitState WaitState { get; private set; }
    // ���[����](�v���C��)
    public PlayingState PlayingState { get; private set; }
    // ���_�������o��
    public PayoutState PayoutState { get; private set; }

    // �R���X�g���N�^
    public MainGameFlow(GameManager gameManager)
    {
        // �����p�^�C�}�[�쐬
        flowStopTimer = new Timer();

        InsertState = new InsertState(gameManager);
        LotsState = new LotsState(gameManager);
        WaitState = new WaitState(gameManager);
        PlayingState = new PlayingState(gameManager);
        PayoutState = new PayoutState(gameManager);

        stateManager = new StateManager(InsertState);
    }

    // �f�X�g���N�^
    ~MainGameFlow()
    {
        // Timer�̃X�g�b�v
        flowStopTimer.Stop();
        flowStopTimer.Dispose();
    }

    // func
    public void UpdateState() => stateManager.StatementUpdate();
}
