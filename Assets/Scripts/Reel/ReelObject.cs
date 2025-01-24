using ReelSpinGame_Reels;
using ReelSpinGame_Reels.ReelArray;
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


    // var

    // ���݂̉�]���x
    private float rotateSpeed = 0.0f;

    // �ō����x
    private float maxSpeed = 0.0f;

    //[SerializeField] REEL_COLUMN_ID reelID;
    private SymbolChange[] symbolsObj;

    // ���[����������
    public ReelData ReelData { get; private set; }

    bool isSpinning = false;

    void Awake()
    {
        symbolsObj = GetComponentsInChildren<SymbolChange>();
        Debug.Log("ReelSpin AwakeDone");
    }

    private void Start()
    {
        UpdateSymbolsObjects();
        Debug.Log("StartDone");

        StartReel(1.0f);
    }

    void FixedUpdate()
    {
        if(isSpinning)
        {
            if (rotateSpeed <= maxSpeed) { AccerateReelSpeed(); }
            RotateReel();
        }
    }

    // ���[���f�[�^��n��
    public void SetReelData(ReelData reelData) => ReelData = reelData;

    //�@���[���n��
    public void StartReel(float maxSpeed)
    {
        isSpinning = true;
        this.maxSpeed = maxSpeed;
    }

    // ���x����
    void AccerateReelSpeed()
    {
        Math.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), 
            -1 * maxSpeed, maxSpeed);
    }

    // ���[����]
    void RotateReel()
    {
        transform.Rotate((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed * Vector3.left);

        // ���p�x�ɒB������}���̍X�V(17.174 or 342.826)
        if ((Math.Abs(transform.rotation.eulerAngles.x) <= 360.0f - ChangeAngle && rotateSpeed > 0) ||
            (Math.Abs(transform.rotation.eulerAngles.x) >= ChangeAngle && rotateSpeed < 0))
        {
            ReelData.ChangeReelPos(rotateSpeed);
            UpdateSymbolsObjects();

            Debug.Log("Changed Symbol");

            transform.Rotate(Vector3.right, ChangeAngle * Math.Sign(rotateSpeed));

            /*if (reelData.WillStop && reelData.CurrentDelay == 0)
            {
                reelData.ChangeSpinState(false);
                //Recalculate the rotation
                transform.Rotate(Vector3.left, Math.Abs(transform.rotation.eulerAngles.x));
                rotateSpeed = 0;
            }
            else if (reelData.WillStop){reelData.DecrementDelay(); }*/
        }
    }

    //�}���̍X�V
    private void UpdateSymbolsObjects()
    {
        // ���݂̃��[�����i����Ƃ��Ĉʒu���X�V����B
        foreach(SymbolChange symbol in symbolsObj)
        {
            symbol.ChangeSymbol(ReelData.Array[ReelData.GetReelPos(symbol.GetPosID())]);
            Debug.Log("Changed Symbol:" + ReelData.Array[ReelData.GetReelPos(symbol.GetPosID())]);
        }
    }

    // ��]���v�Z
    private float ReturnAngularVelocity(float rpsValue)
    {
        //Radian
        float radian = rpsValue * 2.0f * MathF.PI;
        //ConvertRadian to angle per seconds
        return radian * 180.0f / MathF.PI;
    }

    // �����x��Ԃ�
    private float ReturnReelAccerateSpeed(float rpsValue)
    {
        return ReelRadius / 100f * ReturnAngularVelocity(rpsValue) / 1000.0f;
    }
}
