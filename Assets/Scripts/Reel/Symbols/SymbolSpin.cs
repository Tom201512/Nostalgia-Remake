using UnityEngine;

public class SymbolSpin : MonoBehaviour
{
    /*const float ROTATE_MAX = 360.0f / 21.0f;
    [SerializeField] DataRules.REEL_COLUMN_ID reelID;
    private SymbolChange[] symbolsObj;

    //Get ReelData from presenter
    private ReelData reelData;
    private float currentRotateSpeed = 0.0f;

    void Awake()
    {
        ReelPresenter rP = GameObject.Find("Presenter").GetComponent<ReelPresenter>();
        reelData = rP.ReelDatas[(int)reelID];
        if (reelData == null) { throw new Exception("No Presenter found!"); }
        symbolsObj = GetComponentsInChildren<SymbolChange>();

        Debug.Log("AwakeDone");
    }

    private void Start()
    {
        UpdateSymbolsObjects(reelData);
        Debug.Log("StartDone");
    }

    void FixedUpdate()
    {
        if(reelData.IsSpinning)
        {
            if (currentRotateSpeed <= reelData.RotateSpeed) { AccerateReelSpeed(); }
            RotateReel();
        }
    }

    void AccerateReelSpeed()
    {
        Math.Clamp(currentRotateSpeed += OriginalMathmatics.ReturnReelAccerateSpeed(DataRules.ROTATE_RPM / 60.0f) * Math.Sign(reelData.RotateSpeed), 
            -1 * reelData.RotateSpeed, reelData.RotateSpeed);
    }

    void RotateReel()
    {
        //rotate the reel
        transform.Rotate((OriginalMathmatics.ReturnAngularVelocity(DataRules.ROTATE_RPM / 60.0f)) * Time.deltaTime * currentRotateSpeed * Vector3.left);
        //if LowerReelSymbol out bounds(if x angle is 17.174 or 342.826, depends on rotateSpeed is positive or not)
        if ((Math.Abs(transform.rotation.eulerAngles.x) <= 360.0f - ROTATE_MAX && currentRotateSpeed > 0) ||
            (Math.Abs(transform.rotation.eulerAngles.x) >= ROTATE_MAX && currentRotateSpeed < 0))
        {
            //ChangeSymbols
            reelData.ChangeNextSymbol();
            UpdateSymbolsObjects(reelData);
            transform.Rotate(Vector3.right, ROTATE_MAX * Math.Sign(currentRotateSpeed));

            if (reelData.WillStop && reelData.CurrentDelay == 0)
            {
                reelData.ChangeSpinState(false);
                //Recalculate the rotation
                transform.Rotate(Vector3.left, Math.Abs(transform.rotation.eulerAngles.x));
                currentRotateSpeed = 0;
            }
            else if (reelData.WillStop){reelData.DecrementDelay(); }
        }
    }

    void UpdateSymbolsObjects(ReelData reel)
    {
        foreach(SymbolChange symbol in symbolsObj)
        {
            symbol.ChangeSymbol(reelData.ReturnReelSymbol((int)symbol.Position));
        }
    }*/
}
