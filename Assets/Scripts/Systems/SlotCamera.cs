using UnityEngine;

public class SlotCamera : MonoBehaviour
{
    // カメラ機能

    // var
    // 従来の2D描画にするか
    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // func
    
    // カメラの表示方式の変更
    public void ChangeCameraMode() => camera.orthographic = !camera.orthographic;
}
