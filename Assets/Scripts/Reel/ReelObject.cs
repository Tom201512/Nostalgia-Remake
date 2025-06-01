using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.PostProcessing;
using static ReelSpinGame_Reels.ReelData;

public class ReelObject : MonoBehaviour
{
    // ���[���I�u�W�F�N�g

    // const
    // ��]���}���ύX�̊p�x (360�x��21����) (��]�͉�����)
    //const float ChangeAngle = 360.0f - 360.0f / 21.0f;
    // �t��]���}���ύX�̊p�x
    //const float ReversedChangeAngle = 360.0f / 21.0f;
    // �}���ύX���̊p�x (360�x��21����)
    const float ChangeAngle = 360.0f / 21.0f;
    // ��~�ő�L���͈�
    const float ChangeOffset = 0f;
    // ��]���x (Rotate Per Second)
    const float RotateRPS = 79.8f / 60.0f;
    // ���[�����a(cm)
    const float ReelRadius = 12.75f;
    // JAC���̌��x�������l
    const float JacLightOffset = 0.4f;

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
        postVolume = GetComponent<PostProcessVolume>();
        postVolume.enabled = false;

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
        //Debug.Log("RPS:" + RotateRPS);
    }

    private void FixedUpdate()
    {
        //�~�܂��Ă��Ȃ��Ƃ��͉���
        if (rotateSpeed < maxSpeed && Math.Sign(maxSpeed) == 1 ||
        rotateSpeed > maxSpeed && Math.Sign(maxSpeed) == -1)
        {
            SpeedUpReel();
        }
    }

    // ���s��(�펞�X�V)
    private void Update()
    {
        if (maxSpeed != 0)
        {
            RotateReel();
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
    public void StopReel(int pushedPos, int delay) => reelData.BeginStopReel(pushedPos, delay);

    // ���x����
    private void SpeedUpReel() =>
        rotateSpeed = Mathf.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), -1 * maxSpeed, maxSpeed);

    // JAC���̖��邳�v�Z(
    private byte CalculateJACBrightness(bool isNegative)
    {
        float brightnessTest = 0;
        float currentDistance = 0;

        // �������߂Ɍ��点�邽�ߋ����͒Z������
        float distanceRotation = ChangeAngle * JacLightOffset;

        // �����ɍ��킹�ċ������v�Z
        Debug.Log("Current Euler:" + transform.rotation.eulerAngles.x);
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
        Debug.Log("Brightness:" + brightnessTest);

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
        float rotationAngle = Math.Clamp((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed, 0, 360);
        transform.Rotate(rotationAngle * Vector3.left);

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

        Debug.Log("Euler:" + transform.rotation.eulerAngles.x);
        Debug.Log("ChangeAngle:" + ChangeAngle);

        // ���p�x�ɒB������}���̍X�V(17.14286�x)
        if (transform.rotation.eulerAngles.x > 0 &&
            (transform.rotation.eulerAngles.x < 360f - ChangeAngle && Math.Sign(rotateSpeed) == 1))
        {
            Debug.Log("Symbol changed");
            // �}���ʒu�ύX
            reelData.ChangeReelPos(rotateSpeed);
            symbolManager.UpdateSymbolsObjects();

            // �}���̏ꏊ�����ύX�p�x����]��߂�
            float dif = 0f;

            if (Math.Sign(maxSpeed) == -1)
            {
                //dif = ReversedChangeAngle - transform.rotation.eulerAngles.x;
                dif = ChangeAngle - transform.rotation.eulerAngles.x;
            }
            else
            {
                dif = ChangeAngle - transform.rotation.eulerAngles.x;
            }

            if (dif != 0)
            {
                //transform.rotation = Quaternion.identity;
                //transform.Rotate(ReversedChangeAngle * Vector3.right);
            }

            // �}���̏ꏊ�����ύX�p�x����]��߂�
            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

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
                // �ēx���[���̊p�x�𒲐����Ē�~������
                transform.rotation = Quaternion.identity;
                rotateSpeed = 0;
                maxSpeed = 0;
                reelData.FinishStopReel();

                //Debug.Log("Stopped");
            }

            // �ύX��̓|�X�g�G�t�F�N�g��؂�
            postVolume.enabled = false;
        }
        // ��]���쒆�̓|�X�g�G�t�F�N�g��L���ɂ���
        else
        {
            postVolume.enabled = true;
        }
    }

    // ���[���{�̂��̂��̖̂��邳��ύX
    public void SetReelBaseBrightness(byte brightness) => reelBase.ChangeBrightness(brightness);
    // �w��ʒu���[���}���̖��邳��ύX
    public void SetSymbolBrightness(int posID, byte brightness) => symbolManager.SymbolObj[GetReelArrayIndex(posID)].ChangeBrightness(brightness);

    // ��]�p�v�Z
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
