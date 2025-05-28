using System.Timers;
using UnityEngine;

public class WaitManager
{
    // �E�F�C�g�Ǘ�

    // const
    // �E�F�C�g�ɕK�v�Ȏ���(�~���b)
    public const int WaitTimerSetting = 4100;
    // var
    // �����p�^�C�}�[
    private Timer updateTimer;
    // �E�F�C�g���L����
    public bool HasWait { get; private set; }
    // �E�F�C�g�𖳌��ɂ��Ă��邩
    public bool HasWaitCut { get; private set; }

    // �R���X�g���N�^
    public WaitManager(bool hasWaitCut)
    {
        // �����p�^�C�}�[�쐬
        HasWaitCut = hasWaitCut;
        updateTimer = new Timer(WaitTimerSetting);
    }

    // func
    // �^�C�}�[�����̔j��
    public void DisposeWait()
    {
        // Timer�̃X�g�b�v
        updateTimer.Stop();
        updateTimer.Dispose();
    }

    // �E�F�C�g�J�b�g�̐ݒ�
    public void SetWaitCutSetting(bool hasWaitCut) => HasWaitCut = hasWaitCut;

    // �E�F�C�g��������
    public void SetWaitTimer()
    {
        if (HasWaitCut)
        {
            //Debug.Log("WaitCut is enabled");
        }

        else if (HasWait)
        {
            //Debug.Log("Wait is enabled already");
        }

        // �E�F�C�g�J�b�g�A�܂��͎��s���̃E�F�C�g���Ȃ���Ύ��s
        else
        {
            HasWait = true;
            updateTimer.Elapsed += WaitProcess;
            updateTimer.AutoReset = false;
            updateTimer.Start();

            //Debug.Log("Wait start");
        }
    }

    // �R���[�`���p

    // �E�F�C�g�Ǘ�
    private void WaitProcess(object sender, ElapsedEventArgs e)
    {
        HasWait = false;
        updateTimer.Elapsed -= WaitProcess;
        //Debug.Log("Wait disabled");
    }
}
