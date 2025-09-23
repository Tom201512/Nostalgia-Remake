using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    // 画面UIメイン

    // const

    // 解像度
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
        // UIサイズ調整
        float screenAspect = Screen.width / (float)Screen.height;
        float targetAspect = AspectX / AspectY;

        float magRate = targetAspect / screenAspect;
    }

    // 最大公約数を求める
    private int GetGCD(int x, int y)
    {
        if(y == 0)
        {
            return x;
        }
        return GetGCD(y, x % y);
    }
}
