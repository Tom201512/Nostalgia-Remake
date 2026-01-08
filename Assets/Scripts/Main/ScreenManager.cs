using ReelSpinGame_Save.Database.Option;
using UnityEngine;

// 画面設定
public class ScreenManager : MonoBehaviour
{
    [SerializeField] Camera mainGameCamera;    // メインカメラ

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);
        Application.targetFrameRate = 60;
        mainGameCamera.orthographic = false;
    }

    // カメラモードの設定
    public void SetCameraMode(bool useOrthographic) => mainGameCamera.orthographic = useOrthographic;

    //　設定に合わせた画面サイズにする
    public void SetScreenSize(ResolutionOptionID resolutionID)
    {
        switch(resolutionID)
        {
            case ResolutionOptionID.W960H540:
                Screen.SetResolution(960, 540, false);
                break;
            case ResolutionOptionID.W1024H576:
                Screen.SetResolution(1024, 576, false);
                break;
            case ResolutionOptionID.W1280H720:
                Screen.SetResolution(1280, 720, false);
                break;
            case ResolutionOptionID.W1366H768:
                Screen.SetResolution(1366, 768, false);
                break;
            case ResolutionOptionID.W1600H900:
                Screen.SetResolution(1600, 900, false);
                break;
            case ResolutionOptionID.W1920H1080:
                Screen.SetResolution(1920, 1080, false);
                break;
        }
    }
}
