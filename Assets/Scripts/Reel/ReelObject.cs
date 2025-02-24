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

    // �~�܂�\�肩
    public bool IsStopping { get; private set; }

    // ��~������
    public bool HasStopped { get; private set; }

    // ���[����������
    public ReelData ReelData { get; private set; }
    // ���[�����̐}��
    private SymbolChange[] symbolsObj;


    // ������
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        delayToStop = 0;
        IsStopping = false;
        HasStopped = true;
        symbolsObj = GetComponentsInChildren<SymbolChange>();
        Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        UpdateSymbolsObjects();
        Debug.Log("StartDone");
    }

    // ���s��(60FPS�ł̍X�V)
    private void FixedUpdate()
    {
        if(maxSpeed != 0)
        {
            //�~�܂��Ă��Ȃ��Ƃ��͉���
            if (rotateSpeed <= maxSpeed) 
            { 
                SpeedUpReel(); 
            }
            RotateReel();
        }
    }


    // func

    // ���[���f�[�^��n��
    public void SetReelData(ReelData reelData) => ReelData = reelData;

    // �w��ʒu���烊�[���ʒu��n��
    public int GetReelPos(ReelData.ReelPosID posID) => ReelData.GetReelPos((sbyte)posID);

    // �w��ʒu���烊�[���}����n��
    public ReelData.ReelSymbols GetReelSymbol(ReelData.ReelPosID posID) => ReelData.GetReelSymbol((sbyte)posID);


    //�@���[���n��
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        HasStopped = false;
    }

    // ���[����~
    public void StopReel(int delay)
    {
        if(delay < 0 || delay > MaxDelay)
        {
            throw new Exception("Invalid Delay. Must be within 0~4");
        }

        Debug.Log("Received Stop Delay:" + delay);
        delayToStop = delay;
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

        // ���p�x�ɒB������}���̍X�V(17.174 or 342.826)
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
            }

            // ��~���邪�f�B���C(�X�x��)������ΐ��l�����炷(���̐}���X�V�Ŏ~�߂�)
            else if (IsStopping)
            {
                delayToStop -= 1;
            }
        }
    }

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
