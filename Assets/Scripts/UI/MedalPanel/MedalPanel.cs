using UnityEngine;
using UnityEngine.UI;

public class MedalPanel : MonoBehaviour
{
    // メダルパネル部分

    // var
    // メダル1枚ランプ
    [SerializeField] private Image medal1;
    // メダル2枚ランプA(上)
    [SerializeField] private Image medal2A;
    // メダル2枚ランプB(下)
    [SerializeField] private Image medal2B;
    // メダル3枚ランプA(上)
    [SerializeField] private Image medal3A;
    // メダル3枚ランプB(下)
    [SerializeField] private Image medal3B;

    // func
    public void ChangeMedal1Lamp(byte brightness) => medal1.color = new Color32(brightness, brightness, brightness, 255);

    public void ChangeMedal2Lamp(byte brightness)
    {
        Debug.Log(new Color(brightness, brightness, brightness));
        medal2A.color = new Color32(brightness, brightness, brightness, 255);
        medal2B.color = new Color32(brightness, brightness, brightness, 255);
    }

    public void ChangeMedal3Lamp(byte brightness)
    {
        medal3A.color = new Color32(brightness, brightness, brightness, 255);
        medal3B.color = new Color32(brightness, brightness, brightness, 255);
    }
}
