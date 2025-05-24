using UnityEngine;
using UnityEngine.UI;

public class MedalPanel : MonoBehaviour
{
    // ���_���p�l������

    // var
    // ���_��1�������v
    [SerializeField] private Image medal1;
    // ���_��2�������vA(��)
    [SerializeField] private Image medal2A;
    // ���_��2�������vB(��)
    [SerializeField] private Image medal2B;
    // ���_��3�������vA(��)
    [SerializeField] private Image medal3A;
    // ���_��3�������vB(��)
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
