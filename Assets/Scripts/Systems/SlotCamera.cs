using UnityEngine;

public class SlotCamera : MonoBehaviour
{
    // �J�����@�\

    // var
    private Camera slotCam;

    private void Awake()
    {
        slotCam = GetComponent<Camera>();
    }

    // func
    
    // �J�����̕\�������̕ύX
    public void ChangeCameraMode() => slotCam.orthographic = !slotCam.orthographic;
}
