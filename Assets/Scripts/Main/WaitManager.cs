using System;
using System.Timers;
using UnityEngine;

public class WaitManager
{
    // �E�F�C�g�Ǘ�

    // const
    // �E�F�C�g�ɕK�v�Ȏ���(�~���b)
    public const int WaitTimerSetting = 4100;

    // �Q�[���X�e�[�g�p
    public MainGameFlow MainFlow { get; private set; }

    // var
    // �����p�^�C�}�[
    private Timer updateTimer;
    // �E�F�C�g���L����
    public bool hasWait { get; private set; }
    // �E�F�C�g�𖳌��ɂ��Ă��邩
    public bool hasWaitCut { get; private set; }

    // �R���X�g���N�^
    public WaitManager(bool hasWaitCut)
    {
        // �����p�^�C�}�[�쐬
        this.hasWaitCut = hasWaitCut;
        updateTimer = new Timer(WaitTimerSetting);
    }

    // �f�X�g���N�^
    ~WaitManager()
    {
        // Timer�̃X�g�b�v
        updateTimer.Stop();
        updateTimer.Dispose();
    }

    // func

    // �E�F�C�g�J�b�g�̐ݒ�
    public void SetWaitCutSetting(bool hasWaitCut) => this.hasWaitCut = hasWaitCut;

    // �E�F�C�g��������
    public void SetWaitTimer()
    {
        if (hasWaitCut)
        {
            Debug.Log("WaitCut is enabled");
        }

        else if(hasWait)
        {
            Debug.Log("Wait is enabled already");
        }

        // �E�F�C�g�J�b�g�A�܂��͎��s���̃E�F�C�g���Ȃ���Ύ��s
        else
        {
            hasWait = true;
            updateTimer.Elapsed += WaitProcess;
            updateTimer.AutoReset = false;
            updateTimer.Start();

            Debug.Log("Wait start");
        }
    }

    // �R���[�`���p

    // �E�F�C�g�Ǘ�
    private void WaitProcess(object sender, ElapsedEventArgs e)
    {
        hasWait = false;
        updateTimer.Elapsed -= WaitProcess;
        Debug.Log("Wait disabled");
    }
}
