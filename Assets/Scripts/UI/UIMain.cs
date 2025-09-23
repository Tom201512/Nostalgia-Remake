using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    // ���UI���C��

    // const

    // �𑜓x
    public const float AspectX = 16.0f;
    public const float AspectY = 9.0f;

    // var
    private CanvasScaler canvas;

    // func

    private void Awake()
    {
        canvas = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        // UI�T�C�Y����
        float screenAspect = Screen.width / (float)Screen.height;
        float targetAspect = AspectX / AspectY;

        float magRate = targetAspect / screenAspect;
    }

    // �ő���񐔂����߂�
    private int GetGCD(int x, int y)
    {
        if(y == 0)
        {
            return x;
        }
        return GetGCD(y, x % y);
    }
}
