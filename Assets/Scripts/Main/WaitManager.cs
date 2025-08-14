using UnityEngine;

public class WaitManager : MonoBehaviour
{
    // �E�F�C�g�Ǘ�

    // const
    // �E�F�C�g�ɕK�v�Ȏ���(�~���b)
    public const float WaitTime = 4.1f;
    // var
    // �E�F�C�g���L����
    public bool HasWait { get; private set; }
    // �E�F�C�g�𖳌��ɂ��Ă��邩
    public bool HasWaitCut { get; private set; }

    void Awake()
    {
        HasWait = false;
        HasWaitCut = false;
    }

    // func
    // �E�F�C�g�J�b�g�̐ݒ�
    public void SetWaitCutSetting(bool hasWaitCut) => HasWaitCut = hasWaitCut;

    // �E�F�C�g��������
    public void SetWaitTimer()
    {
        // �E�F�C�g�J�b�g�A�܂��͎��s���̃E�F�C�g���Ȃ���Ύ��s
        if (!HasWaitCut && !HasWait)
        {
            HasWait = true;
            Invoke("DisableWaitTimer", WaitTime);
        }
    }

    // �E�F�C�g��Ԃ�������������(�I�[�g�p)
    public void DisableWaitTimer()
    {
        if(HasWait)
        {
            HasWait = false;
        }
    }
}
