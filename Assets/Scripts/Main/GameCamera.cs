using UnityEngine;

public class GameCamera : MonoBehaviour
{
    // �J�����̐���(��ɉ�ʕύX���̋���)

    // var
    private Camera cam;
    private Vector2 aspectVector;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if(!cam)
        {
            Debug.Log("Loaded");
        }
    }
}
