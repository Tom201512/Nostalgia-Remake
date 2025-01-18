using System;
using UnityEngine;

public class SymbolSpin : MonoBehaviour
{
    // ƒŠ[ƒ‹‚Ì}•¿•”•ª‰ñ“]

    // const

    // }•¿•ÏX‚ÌŠp“x (360“x‚ğ21•ªŠ„)
    const float ChangeAngle = 360.0f / 21.0f;

    // ‰ñ“]‘¬“x (Rotate Per Second)
    public const float RotateRPS = 79.7f / 60.0f;

    // ƒŠ[ƒ‹d‚³(kg)
    public const float ReelWeight = 25.5f;

    // ƒŠ[ƒ‹”¼Œa(cm)
    public const float ReelRadius = 12.75f;


    // var
    private float rotateSpeed = 0.0f;
    private float maxSpeed = 0.0f;

    /*[SerializeField] REEL_COLUMN_ID reelID;
    private SymbolChange[] symbolsObj;

    //Get ReelData from presenter
    private ReelData reelData;
;*/

    bool isSpinning = false;

    void Awake()
    {
        //ReelPresenter rP = GameObject.Find("Presenter").GetComponent<ReelPresenter>();
        //reelData = rP.ReelDatas[(int)reelID];
        //if (reelData == null) { throw new Exception("No Presenter found!"); }
        //symbolsObj = GetComponentsInChildren<SymbolChange>();

        //Debug.Log("AwakeDone");
    }

    private void Start()
    {
        //UpdateSymbolsObjects(reelData);
        //Debug.Log("StartDone");

        //StartReel(1.0f);
    }

    void FixedUpdate()
    {
        if(isSpinning)
        {
            if (rotateSpeed <= maxSpeed) { AccerateReelSpeed(); }
            RotateReel();
        }
    }

    public void StartReel(float maxSpeed)
    {
        isSpinning = true;
        this.maxSpeed = maxSpeed;
    }

    void AccerateReelSpeed()
    {
        Math.Clamp(rotateSpeed += ReturnReelAccerateSpeed(RotateRPS) * Math.Sign(maxSpeed), 
            -1 * maxSpeed, maxSpeed);
    }

    void RotateReel()
    {
        //rotate the reel
        transform.Rotate((ReturnAngularVelocity(RotateRPS)) * Time.deltaTime * rotateSpeed * Vector3.left);
        //if LowerReelSymbol out bounds(if x angle is 17.174 or 342.826, depends on rotateSpeed is positive or not)
        if ((Math.Abs(transform.rotation.eulerAngles.x) <= 360.0f - ChangeAngle && rotateSpeed > 0) ||
            (Math.Abs(transform.rotation.eulerAngles.x) >= ChangeAngle && rotateSpeed < 0))
        {
            //ChangeSymbols
            //reelData.ChangeNextSymbol();
            //UpdateSymbolsObjects(reelData);

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

    /*private void UpdateSymbolsObjects(ReelData reel)
    {
        foreach(SymbolChange symbol in symbolsObj)
        {
            symbol.ChangeSymbol(reelData.ReturnReelSymbol((int)symbol.Position));
        }
    }*/


    private float ReturnAngularVelocity(float rpsValue)
    {
        //Radian
        float radian = rpsValue * 2.0f * MathF.PI;
        //ConvertRadian to angle per seconds
        return radian * 180.0f / MathF.PI;
    }

    private float ReturnReelAccerateSpeed(float rpsValue)
    {
        return ReelRadius / 100f * ReturnAngularVelocity(rpsValue) / 1000.0f;
    }
}
