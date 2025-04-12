using UnityEngine;

public class Camera : MonoBehaviour
{
    // ƒJƒƒ‰‚Ì§Œä(å‚É‰æ–Ê•ÏX‚Ì‹““®)

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
