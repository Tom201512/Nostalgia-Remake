using UnityEngine;

public class SlotCamera : MonoBehaviour
{
    // �J�����@�\

    // var
    // �]����2D�`��ɂ��邩
    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // func
    
    // �J�����̕\�������̕ύX
    public void ChangeCameraMode() => camera.orthographic = !camera.orthographic;
}
