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
    public InsertState insertState { get; private set; }
    // ���I
    public LotsState lotsState { get; private set; }
    // �E�F�C�g�̏��
    public WaitState waitState { get; private set; }
    // ���[����](�v���C��)
    public PlayingState playingState { get; private set; }
    // ���_�������o��
    public PayoutState payoutState { get; private set; }

    // �R���X�g���N�^
    public MainGameFlow(GameManager gameManager)
    {
        // �����p�^�C�}�[�쐬
        flowStopTimer = new Timer();

        insertState = new InsertState(gameManager);
        lotsState = new LotsState(gameManager);
        waitState = new WaitState(gameManager);
        playingState = new PlayingState(gameManager);
        payoutState = new PayoutState(gameManager);

        stateManager = new StateManager(insertState);
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
