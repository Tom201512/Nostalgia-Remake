using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class ReelObject : MonoBehaviour
{
    // ���[���I�u�W�F�N�g

    // const
    // �}���ύX���̊p�x (360�x��21����)
    const float ChangeAngle = 360.0f / 21.0f;
    // ��]���x (Rotate Per Second)
    public const float RotateRPS = 79.8f / 60.0f;
    // ���[���d��(kg)
    public const float ReelWeight = 25.5f;
    // ���[�����a(cm)
    public const float ReelRadius = 12.75f;

    // var
    // ���݂̉�]���x
    private float rotateSpeed;
    // �ō����x
    private float maxSpeed;

    // ���[���{��
    private ReelBase reelBase;
    // ���[�����̐}��
    //private SymbolChange[] symbolsObj;
    // �}���}�l�[�W���[
    private SymbolManager symbolManager;

    // ���[�����
    public ReelData ReelData { get; private set; }

    // ���[�����
    [SerializeField] ReelDatabase reelDatabaseFile;

    // ������
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;

        reelBase = GetComponentInChildren<ReelBase>();
        symbolManager = GetComponentInChildren<SymbolManager>();

        foreach (byte value in reelDatabaseFile.Array)
        {
            Debug.Log(value + "Symbol:" + ReturnSymbol(value));
        }

        for (int i = 0; i < reelDatabaseFile.Array.Length; i++)
        {
            Debug.Log("No." + i + " Symbol:" + ReturnSymbol(reelDatabaseFile.Array[i]));

        }
        Debug.Log("ReelArray Generated");
        Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        symbolManager.SetReelData(ReelData);
        symbolManager.UpdateSymbolsObjects();
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
    public void SetReelData(int reelID, int initialLowerPos)
    {
        ReelData = new ReelData(reelID, initialLowerPos, reelDatabaseFile);
    }

    // �ō����x���Ԃ�
    public bool IsMaximumSpeed() => rotateSpeed == maxSpeed;

    //�@���[���n��
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        ReelData.BeginStartReel();
    }

    // ���[����~
    public void StopReel(int pushedPos, int delay)
    {
        ReelData.BeginStopReel(pushedPos, delay);
    }

    // ���x����
    private void SpeedUpReel()=> 
        rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), -1 * maxSpeed, maxSpeed);

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
            symbolManager.UpdateSymbolsObjects();

            // �}���̏ꏊ�����ύX�p�x����]��߂�
            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

            // ��~����ʒu�ɂȂ�����
            if (ReelData.IsStopping && ReelData.CheckReachedStop())
            {
                // �ēx���[���̊p�x�𒲐����Ē�~������
                transform.Rotate(Vector3.left, Math.Abs(transform.rotation.eulerAngles.x));
                rotateSpeed = 0;
                maxSpeed = 0;
                ReelData.FinishStopReel();

                Debug.Log("Stopped");
            }
        }
    }

    // ���[���{�̂��̂��̖̂��邳��ύX
    public void SetReelBaseBrightness(byte brightness) => reelBase.SetBrightness(brightness);
    // �w��ʒu���[���}���̖��邳��ύX
    public void SetSymbolBrightness(int posID, byte r, byte g, byte b) => symbolManager.SymbolObj[ReelData.GetReelArrayIndex(posID)].ChangeBrightness(r,g,b);
    // �w��ʒu���[���}���̌��x��ύX
    public void SetSymbolEmission(int posID, byte r, byte g, byte b) => symbolManager.SymbolObj[ReelData.GetReelArrayIndex(posID)].ChangeEmmision(r,g,b);

    // ��]���v�Z
    private float ReturnAngularVelocity(float rpsValue)
    {
        // ���W�A�������߂�
        float radian = rpsValue * 2.0f * MathF.PI;

        // ���W�A�����疈�b�������p�x���v�Z
        return radian * 180.0f / MathF.PI;
    }

    // �����x��Ԃ�
    private float ReturnReelAccerateSpeed(float rpsValue) => ReelRadius / 80f * ReturnAngularVelocity(rpsValue) / 1000.0f;
}
