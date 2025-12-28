using UnityEngine;

public class SlotCamera : MonoBehaviour
{
    // カメラ機能

    // var
    private Camera slotCam;

    private void Awake()
    {
        slotCam = GetComponent<Camera>();
    }

    // func

    // カメラの表示方式の変更
    public void ChangeCameraMode() => slotCam.orthographic = !slotCam.orthographic;
}
