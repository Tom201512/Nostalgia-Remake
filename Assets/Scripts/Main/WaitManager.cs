using System.Threading;
using System.Threading.Tasks;
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
    // �E�F�C�g���L����
    public bool hasWait { get; private set; }
    // �E�F�C�g�𖳌��ɂ��Ă��邩
    public bool hasWaitCut { get; private set; }
    // �L�����Z���p
    private CancellationTokenSource cancel;

    // �R���X�g���N�^
    public WaitManager(bool hasWaitCut)
    {
        // �E�F�C�g�J�b�g�ݒ�
        cancel = new CancellationTokenSource();
        this.hasWaitCut = hasWaitCut;
    }

    // func

    // �E�F�C�g�J�b�g�̐ݒ�
    public void SetWaitCutSetting(bool hasWaitCut) => this.hasWaitCut = hasWaitCut;
    
    public void DisposeWait()
    {
        Debug.Log("Wait is disposed");
        cancel.Cancel();
    }

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
            Task.Run(ActivateWaitTimer);
        }
    }

    async Task ActivateWaitTimer()
    {
        try
        {
            Debug.Log("Wait start");
            hasWait = true;
            await Task.Delay(WaitTimerSetting);
            if (cancel.IsCancellationRequested)
            {
                Debug.Log("Cancelled");
                return;
            }
            hasWait = false;
            Debug.Log("Wait disabled");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error:" + ex);
        }
    }
}
