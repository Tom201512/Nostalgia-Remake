using UnityEngine;

public class Camera : MonoBehaviour
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
