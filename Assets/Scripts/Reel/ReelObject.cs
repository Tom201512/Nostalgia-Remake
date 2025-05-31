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
    // ��~�ő�L���͈�
    const float ChangeOffset = 0.8f;
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
    // �}���}�l�[�W���[
    private SymbolManager symbolManager;
    // ���[�����
    private ReelData reelData;
    // JAC���̓_�������邩
    public bool HasJacModeLight { get; set; }

    // ���[�����
    [SerializeField] ReelDatabase reelDatabaseFile;

    // ������
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        HasJacModeLight = false;

        reelBase = GetComponentInChildren<ReelBase>();
        symbolManager = GetComponentInChildren<SymbolManager>();

        foreach (byte value in reelDatabaseFile.Array)
        {
            ////Debug.Log(value + "Symbol:" + ReturnSymbol(value));
        }

        for (int i = 0; i < reelDatabaseFile.Array.Length; i++)
        {
            ////Debug.Log("No." + i + " Symbol:" + ReturnSymbol(reelDatabaseFile.Array[i]));

        }
        ////Debug.Log("ReelArray Generated");
        ////Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        symbolManager.SetReelData(reelData);
        symbolManager.UpdateSymbolsObjects();
        //Debug.Log("StartDone");
        Debug.Log("RPS:" + RotateRPS);
    }

    // ���s��(�펞�X�V)
    private void Update()
    {
        if (maxSpeed != 0)
        {
            //�~�܂��Ă��Ȃ��Ƃ��͉���
            if (rotateSpeed < maxSpeed)
            {
                SpeedUpReel();
            }

            if (rotateSpeed != 0)
            {
                RotateReel();
            }
        }
    }

    // func

    // ���l�𓾂�
    // ���[����ID
    public int GetReelID() => reelData.ReelID;
    // ���݂̃��[�����
    public ReelStatus GetCurrentReelStatus() => reelData.CurrentReelStatus;
    // �Ō�Ɏ~�߂����i�ʒu
    public int GetLastPushedPos() => reelData.LastPushedPos;
    // ��~�\��ʒu
    public int GetWillStopPos() => reelData.WillStopPos;
    // �Ō�Ɏ~�߂��Ƃ��̃f�B���C��
    public int GetLastDelay() => reelData.LastDelay;

    // �w�肵���ʒu�ɂ��郊�[���̔ԍ���Ԃ�
    public int GetReelPos(ReelPosID posID) => reelData.GetReelPos((sbyte)posID);
    // sbyte�œǂޏꍇ
    public int GetReelPos(sbyte posID) => reelData.GetReelPos(posID);
    // �w�肵���ʒu�̐}����Ԃ�
    public ReelSymbols GetReelSymbol(ReelPosID posID) => reelData.GetReelSymbol((sbyte)posID);
    // sbyte�œǂޏꍇ
    public ReelSymbols GetReelSymbol(sbyte posID) => reelData.GetReelSymbol(posID);
    // ��~�\��ʒu���烊�[���̐}����Ԃ�
    public ReelSymbols GetSymbolFromWillStop(ReelPosID posID) => reelData.GetSymbolFromWillStop((sbyte)posID);
    // sbyte�œǂޏꍇ
    public ReelSymbols GetSymbolFromWillStop(sbyte posID) => reelData.GetSymbolFromWillStop(posID);

    // ���[��������n��
    public ReelDatabase GetReelDatabase() => reelDatabaseFile;

    // ���[���f�[�^��n��
    public void SetReelData(int reelID, int initialLowerPos)
    {
        reelData = new ReelData(reelID, initialLowerPos, reelDatabaseFile);
    }

    // �ō����x���Ԃ�
    public bool IsMaximumSpeed() => rotateSpeed == maxSpeed;

    //�@���[���n��
    public void StartReel(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        reelData.BeginStartReel();
    }

    // ���[����~
    public void StopReel(int pushedPos, int delay)
    {
        reelData.BeginStopReel(pushedPos, delay);
    }

    // ���x����
    private void SpeedUpReel()
    {
        rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), -1 * maxSpeed, maxSpeed);
        Debug.Log("Speed:" + rotateSpeed);
    }

    // JAC���̖��邳�v�Z(
    private byte CalculateJACBrightness(bool isNegative)
    {
        float brightnessTest = 0;
        float distanceRotation = 360 - (360 - ChangeAngle);
        float currentDistance = 0;

        if (transform.rotation.eulerAngles.x > 0)
        {
           currentDistance = Math.Clamp((360 - transform.rotation.eulerAngles.x), 0, distanceRotation);
        }

        brightnessTest = currentDistance / distanceRotation;
        Debug.Log("Bright:" + brightnessTest);

        int distance = SymbolChange.TurnOnValue - SymbolChange.TurnOffValue;


        float CenterBright;

        if (isNegative)
        {
            CenterBright = Math.Clamp(SymbolChange.TurnOnValue - (distance * brightnessTest), 0, 255);
        }
        else
        {
            CenterBright = Math.Clamp(SymbolChange.TurnOffValue + (distance * brightnessTest), 0, 255);
        }

        Debug.Log("Center:" + CenterBright);
        return (byte)Math.Clamp(CenterBright, 0, 255);
    }

    // ���[����]
    private void RotateReel()
    {
        float rotation = ReturnRadPerSecond(RotateRPS) * rotateSpeed;
        Debug.Log("Rotation:" + rotation);

        transform.rotation = Quaternion.AngleAxis(rotation, Vector3.left) * transform.rotation;

        if(HasJacModeLight)
        {
            symbolManager.SymbolObj[(int)ReelPosID.Upper + 1].ChangeBrightness(CalculateJACBrightness(false));
            symbolManager.SymbolObj[(int)ReelPosID.Center + 1].ChangeBrightness(CalculateJACBrightness(true));
        }

        Debug.Log("Euler:" + transform.rotation.eulerAngles.x);

        // ���p�x�ɒB������}���̍X�V(17.14286�x)
        if (transform.rotation.eulerAngles.x > 0 &&
            (Math.Abs(transform.rotation.eulerAngles.x) < 360.0f - ChangeAngle + ChangeOffset && Math.Sign(maxSpeed) == 1))
        {
            // �}���ʒu�ύX

            Debug.Log("Changed");
            reelData.ChangeReelPos(rotateSpeed);
            symbolManager.UpdateSymbolsObjects();

            // �}���̏ꏊ�����ύX�p�x����]��߂�
            Debug.Log("Change Angle" + (360.0f - ChangeAngle));

            float dif = 360.0f - ChangeAngle + ChangeOffset - transform.rotation.eulerAngles.x;
            Debug.Log("Difference" + dif);

            Debug.Log("New Euler:" + transform.rotation.eulerAngles.x);

            if (dif != 0)
            {
                transform.rotation = Quaternion.AngleAxis(dif, Vector3.left);
            }

            Debug.Log("Fixed Euler:" + transform.rotation.eulerAngles.x);

            if (HasJacModeLight)
            {
                symbolManager.SymbolObj[(int)ReelPosID.Upper + 1].ChangeBrightness(SymbolChange.TurnOffValue);
                symbolManager.SymbolObj[(int)ReelPosID.Center + 1].ChangeBrightness(SymbolChange.TurnOnValue);
            }

            // ��~����ʒu�ɂȂ�����
            if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
            {
                // �ēx���[���̊p�x�𒲐����Ē�~������
                transform.rotation = Quaternion.identity;
                rotateSpeed = 0;
                maxSpeed = 0;
                reelData.FinishStopReel();

                //Debug.Log("Stopped");
            }
        }
    }

    // ���[���{�̂��̂��̖̂��邳��ύX
    public void SetReelBaseBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);
    // �w��ʒu���[���}���̖��邳��ύX
    public void SetSymbolBrightness(int posID, byte brightness) => symbolManager.SymbolObj[GetReelArrayIndex(posID)].ChangeBrightness(brightness);

    // �p���x(rad/s)�����߂�
    private float ReturnRadPerSecond(float rpsValue)
    {
        float radian = 2f * MathF.PI * rpsValue;
        return radian;
    }

    // �����x��Ԃ�
    private float ReturnReelAccerateSpeed(float rpsValue)
    {
        float speedAcceration = ReturnRadPerSecond(rpsValue) * Time.deltaTime;
        Debug.Log("Speed:" + speedAcceration);
        return Math.Clamp(speedAcceration, 0, maxSpeed);
    }
}
