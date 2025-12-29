using UnityEngine;

// カメラ機能
public class SlotCamera : MonoBehaviour
{
    Camera slotCam;

    private void Awake()
    {
        slotCam = GetComponent<Camera>();
    }

    // カメラの表示方式の変更
    public void ChangeCameraMode() => slotCam.orthographic = !slotCam.orthographic;
}
