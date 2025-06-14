using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.ReelData;

public class ReelObject : MonoBehaviour
{
    // ���[���I�u�W�F�N�g

    // const
    // �}���ύX���̊p�x (360�x��21����)
    const float ChangeAngle = 360.0f / 21.0f;
    // ���[�����a(cm)
    const float ReelRadius = 12.75f;
    // JAC���̌��x�������l
    const float JacLightOffset = 0.4f;
    // �ō����x�܂ł̌o�ߎ���(�b)
    const float MaxSpeedReelTime = 0.3f;

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
    // ���[�V�����u���[
    private PostProcessVolume postVolume;
    // �u���[�����̃v���t�@�C��
    private MotionBlur motionBlur;

    // ��]���x (Rotate Per Minute)
    [Range(0f, 80f), SerializeField] private float rotateRPM;
    // ��]���x (Rotate Per Second)
    private float rotateRPS;

    // ���[�����
    [SerializeField] ReelDatabase reelDatabaseFile;

    // ������
    private void Awake()
    {
        rotateSpeed = 0.0f;
        maxSpeed = 0.0f;
        rotateRPS = rotateRPM / 60.0f;
        HasJacModeLight = false;

        reelBase = GetComponentInChildren<ReelBase>();
        symbolManager = GetComponentInChildren<SymbolManager>();

        // �u���[�̎擾
        postVolume = GetComponent<PostProcessVolume>();
        postVolume.profile.TryGetSettings(out motionBlur);

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
        //Debug.Log("RPS:" + rotateRPS);
        ChangeBlurSetting(false);
    }

    private void Update()
    {
        //�~�܂��Ă��Ȃ��Ƃ��͉���
        if (rotateSpeed < maxSpeed && Math.Sign(maxSpeed) == 1 ||
        rotateSpeed > maxSpeed && Math.Sign(maxSpeed) == -1)
        {
            SpeedUpReel();
        }
        if (maxSpeed != 0)
        {
            RotateReel();
        }
    }

    // func

    // ���l�𓾂�
    // ���݂̃X�s�[�h
    public float GetCurrentSpeed() => rotateSpeed;
    // ���݂̊p�x
    public float GetCurrentDegree() => transform.rotation.eulerAngles.x;

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
        ChangeBlurSetting(true);
    }

    // ���[����~
    public void StopReel(int pushedPos, int delay) => reelData.BeginStopReel(pushedPos, delay, false);

    // ���[����~(������)
    public void StopReelFast(int pushedPos, int delay)
    {
        // ������~
        reelData.BeginStopReel(pushedPos, delay, true);
        // �}���ʒu�ύX
        symbolManager.UpdateSymbolsObjects();

        // JAC���Ȃ烉�C�g������
        if(HasJacModeLight)
        {
            symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(SymbolChange.TurnOnValue);
            symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Lower)].ChangeBrightness(SymbolChange.TurnOffValue);
            symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Upper)].ChangeBrightness(SymbolChange.TurnOffValue);
        }

        StopReelSpeed();
    }

    // ���x����
    private void SpeedUpReel() =>
        rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(rotateRPS) * Time.deltaTime * Math.Sign(maxSpeed), -1 * maxSpeed, maxSpeed);

    // �u���[��؂邩
    private void ChangeBlurSetting(bool value) => motionBlur.enabled.value = value;

    // JAC���̖��邳�v�Z(
    private byte CalculateJACBrightness(bool isNegative)
    {
        float brightnessTest = 0;
        float currentDistance = 0;

        // �������߂Ɍ��点�邽�ߋ����͒Z������
        float distanceRotation = ChangeAngle * JacLightOffset;

        // �����ɍ��킹�ċ������v�Z
        //Debug.Log("Current Euler:" + transform.rotation.eulerAngles.x);
        if (transform.rotation.eulerAngles.x > 0f)
        {
            if (Math.Sign(maxSpeed) == -1)
            {
                currentDistance = transform.rotation.eulerAngles.x;
            }
            else
            {
                currentDistance = 360.0f - transform.rotation.eulerAngles.x;
            }
        }
        brightnessTest = Math.Clamp(currentDistance / distanceRotation, 0, 1);
        //ebug.Log("Brightness:" + brightnessTest);

        int distance = SymbolChange.TurnOnValue - SymbolChange.TurnOffValue;

        float CenterBright = 0;

        if (isNegative)
        {
            CenterBright = Math.Clamp(SymbolChange.TurnOnValue - (distance * brightnessTest), 0, 255);
        }
        else
        {
            CenterBright = Math.Clamp(SymbolChange.TurnOffValue + (distance * brightnessTest), 0, 255);
        }
        return (byte)Math.Clamp(CenterBright, 0, 255);
    }

    // ���[����]
    private void RotateReel()
    {
        float rotationAngle = Math.Clamp((ReturnDegreePerSecond(rotateRPS)) * Time.deltaTime * rotateSpeed, 0, 360);
        transform.Rotate(rotationAngle * Vector3.left);

        // JAC���ł���΃��C�g�̒���������
        if (HasJacModeLight)
        {
            if (Math.Sign(maxSpeed) == -1)
            {
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(CalculateJACBrightness(false));
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Lower)].ChangeBrightness(CalculateJACBrightness(true));
            }
            else
            {
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Upper)].ChangeBrightness(CalculateJACBrightness(false));
                symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(CalculateJACBrightness(true));
            }
        }

        //Debug.Log("Euler:" + transform.rotation.eulerAngles.x);
        //Debug.Log("ChangeAngle:" + (360f - ChangeAngle));

        // ���p�x�ɒB������}���̍X�V(17.14286�x)
        if (transform.rotation.eulerAngles.x > 0 &&
            (transform.rotation.eulerAngles.x < 360f - ChangeAngle && Math.Sign(rotateSpeed) == 1) ||
            (transform.rotation.eulerAngles.x > ChangeAngle && Math.Sign(rotateSpeed) == -1))
        {
            //Debug.Log("Symbol changed");
            // �}���ʒu�ύX
            reelData.ChangeReelPos(rotateSpeed);
            symbolManager.UpdateSymbolsObjects();

            // �p�x�����Ƃɖ߂�
            transform.Rotate(Vector3.right, (ChangeAngle) * Math.Sign(rotateSpeed));

            if (HasJacModeLight)
            {
                if (Math.Sign(maxSpeed) == -1)
                {
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(SymbolChange.TurnOffValue);
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Lower)].ChangeBrightness(SymbolChange.TurnOnValue);
                }
                else
                {
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Upper)].ChangeBrightness(SymbolChange.TurnOffValue);
                    symbolManager.SymbolObj[GetReelArrayIndex((int)ReelPosID.Center)].ChangeBrightness(SymbolChange.TurnOnValue);
                }
            }

            // ��~����ʒu�ɂȂ�����
            if (reelData.CurrentReelStatus == ReelStatus.Stopping && reelData.CheckReachedStop())
            {
                StopReelSpeed();
            }
        }
    }

    // ���[���̉�]���~������
    private void StopReelSpeed()
    {
        // �ēx���[���̊p�x�𒲐����Ē�~������
        transform.rotation = Quaternion.identity;
        rotateSpeed = 0;
        maxSpeed = 0;
        reelData.FinishStopReel();
    }

    // ���[���{�̂��̂��̖̂��邳��ύX
    public void SetReelBaseBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);
    // �w��ʒu���[���}���̖��邳��ύX
    public void SetSymbolBrightness(int posID, byte brightness) => symbolManager.SymbolObj[GetReelArrayIndex(posID)].ChangeBrightness(brightness);

    // 1�b�ɉ��x��]�����邩�v�Z
    private float ReturnDegreePerSecond(float rpsValue)
    {
        // ���W�A�������߂�
        float radian = rpsValue * 2.0f * MathF.PI;

        // ���W�A�����疈�b�������p�x���v�Z

        float result = 180.0f / MathF.PI * radian;
        //Debug.Log("deg/s:" + result);
        return result;
    }

    // �����x��Ԃ�
    private float ReturnReelAccerateSpeed(float rpsValue)
    {
        // ���W�A�������߂�
        float radian = rpsValue * 2.0f * MathF.PI;
        // �ڐ����x(m/s)�����߂�
        float tangentalVelocity = ReelRadius * radian / 100f;
        //Debug.Log("TangentalVelocity:" + tangentalVelocity);

        // �K�v�o�ߎ��Ԃ��瑬�x���o��
        float speed = tangentalVelocity / MaxSpeedReelTime;
        //Debug.Log("Speed:" + speed);
        return speed;
    }
}
