using UnityEngine;

public class GameCamera : MonoBehaviour
{
    // カメラの制御(主に画面変更時の挙動)

    // var
    private Camera cam;
    private Vector2 aspectVector;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if(!cam)
        {
            //Debug.Log("Loaded");
        }
    }
}
