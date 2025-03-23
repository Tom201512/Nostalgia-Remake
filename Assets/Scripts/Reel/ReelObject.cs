using ReelSpinGame_Reels;
using System;
using UnityEngine;

public class ReelObject : MonoBehaviour
{
    // ���[���I�u�W�F�N�g

    // const

    // �}���ύX���̊p�x (360�x��21����)
    const float ChangeAngle = 360.0f / 21.0f;
    // ��]���x (Rotate Per Second)
    public const float RotateRPS = 79.7f / 60.0f;
    // ���[���d��(kg)
    public const float ReelWeight = 25.5f;
    // ���[�����a(cm)
    public const float ReelRadius = 12.75f;
    // �ō��f�B���C(�X�x���R�}�� 4)
    public const int MaxDelay = 4;

    // var
    // ���݂̉�]���x
    private float rotateSpeed;
    // �ō����x
    private float maxSpeed;
    // ��~����̂ɕK�v�ȃf�B���C(�X�x��)
    private int delayToStop;

    // ���[���{��
    private ReelBase reelBase;
    // ���[�����̐}��
    private SymbolChange[] symbolsObj;

    // �~�܂�\�肩
    public bool IsStopping { get; private set; }
    // ��~������
    public bool HasStopped { get; private set; }
    // �Ō�Ɏ~�߂��ʒu(���i�)
    public int lastPressedPos { get; private set; }
    // �Ō�Ɏ~�߂��Ƃ��̃f�B���C��
    public int lastDelay { get; private set; }
    // ���[�����
    public ReelData ReelData { get; private set; }

    // ������
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        delayToStop = 0;
        lastPressedPos = 0;
        lastDelay = 0;
        IsStopping = false;
        HasStopped = true;

        symbolsObj = GetComponentsInChildren<SymbolChange>();
        reelBase = GetComponentInChildren<ReelBase>();
        Debug.Log("ReelSpin AwakeDone");

        Debug.Log(ChangeAngle);
    }

    private void Start()
    {
        UpdateSymbolsObjects();
        Debug.Log("StartDone");
    }

    // ���s��(60FPS�ł̍X�V)
    private void FixedUpdate()
    {
        if (maxSpeed != 0)
        {
            //�~�܂��Ă��Ȃ��Ƃ��͉���
            if (rotateSpeed <= maxSpeed)
            {
                SpeedUpReel();
            }
        }
    }

    // ���s��(�펞�X�V)
    private void Update()
    {
        if(maxSpeed != 0)
        {
            RotateReel();
        }
    }

    // func
    // ���[���f�[�^��n��
    public void SetReelData(ReelData reelData) => ReelData = reelData;
    // �w��ʒu���烊�[���ʒu��n��
    public int GetReelPos(int posID) => ReelData.GetReelPos((sbyte)posID);
    // �w��ʒu���烊�[���}����n��
    public ReelData.ReelSymbols GetReelSymbol(int posID) => ReelData.GetReelSymbol((sbyte)posID);
    // �������ʒu��Ԃ�(�e�[�u�����䔻��p)
    public int GetPressedPos() => ReelData.GetReelPos((int)ReelData.ReelPosID.Center);
    // �Ō�Ɏ~�߂���~�ʒu��Ԃ�
    public int GetLastPressedPos() => lastPressedPos;
    // ���߂̃f�B���C����Ԃ�
    public int GetLastDelay() => lastDelay;

    //�@���[���n��
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        HasStopped = false;
    }

    // ���[����~
    public void StopReel(int delay)
    {
        // ��~�ʒu���L�^
        lastPressedPos = ReelData.GetReelPos((int)ReelData.ReelPosID.Center);

        if (delay < 0 || delay > MaxDelay)
        {
            throw new Exception("Invalid Delay. Must be within 0~4");
        }
        Debug.Log("Received Stop Delay:" + delay);

        // �e�[�u�����瓾���f�B���C���L�^���A���̕����[���̒�~��x�点��B
        delayToStop = delay;
        lastDelay = delay;
        IsStopping = true;
    }

    // ���x����
    private void SpeedUpReel()
    {
        Math.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), 
            -1 * maxSpeed, maxSpeed);
    }

    // ���[����]
    private void RotateReel()
    {
        transform.Rotate((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed * Vector3.left);

        // ���p�x�ɒB������}���̍X�V(17.14286�x)
        if ((Math.Abs(transform.rotation.eulerAngles.x) <= 360.0f - ChangeAngle && Math.Sign(rotateSpeed) == -1 ||
            (Math.Abs(transform.rotation.eulerAngles.x) >= ChangeAngle && Math.Sign(rotateSpeed) == 1)))
        {
            // �}���ʒu�ύX
            ReelData.ChangeReelPos(rotateSpeed);
            UpdateSymbolsObjects();

            // �ύX�p�x��������]��߂��B
            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

            // ��~����ꍇ��
            if (IsStopping && delayToStop == 0)
            {
                // �ēx���[���̊p�x�𒲐����Ē�~������
                transform.Rotate(Vector3.left, Math.Abs(transform.rotation.eulerAngles.x));
                rotateSpeed = 0;
                maxSpeed = 0;
                IsStopping = false;
                HasStopped = true;

                Debug.Log("Stopped");
            }

            // ��~���邪�f�B���C(�X�x��)������ΐ��l�����炷(���̐}���X�V�Ŏ~�߂�)
            else if (IsStopping)
            {
                delayToStop -= 1;
            }
        }
    }

    // ���[���{�̂��̂��̖̂��邳��ύX
    public void SetReelBaseBrightness(byte brightness) => reelBase.SetBrightness(brightness);

    // �w��ʒu���[���}���̖��邳��ύX
    public void SetSymbolBrightness(int posArrayID, byte brightness) => symbolsObj[posArrayID].ChangeBrightness(brightness);

    //�}���̍X�V
    private void UpdateSymbolsObjects()
    {
        // ���݂̃��[�����i����Ƃ��Ĉʒu���X�V����B
        foreach(SymbolChange symbol in symbolsObj)
        {
            symbol.ChangeSymbol(ReelData.GetReelSymbol((sbyte)symbol.GetPosID()));
        }
    }

    // ��]���v�Z
    private float ReturnAngularVelocity(float rpsValue)
    {
        // ���W�A�������߂�
        float radian = rpsValue * 2.0f * MathF.PI;

        // ���W�A�����疈�b�������p�x���v�Z
        return radian * 180.0f / MathF.PI;
    }

    // �����x��Ԃ�
    private float ReturnReelAccerateSpeed(float rpsValue)
    {
        return ReelRadius / 100f * ReturnAngularVelocity(rpsValue) / 1000.0f;
    }
}
