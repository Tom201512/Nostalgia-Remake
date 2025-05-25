using UnityEngine;
using UnityEngine.UI;

public class MedalPanel : MonoBehaviour
{
    // const
    // デフォルトの明るさ(点灯時)
    const byte TurnOnValue = 255;
    // デフォルトの暗さ(消灯時)
    const byte TurnOffValue = 120;

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
    private void ChangeMedal1Lamp(byte brightness) => medal1.color = new Color32(brightness, brightness, brightness, 255);

    private void ChangeMedal2Lamp(byte brightness)
    {
        medal2A.color = new Color32(brightness, brightness, brightness, 255);
        medal2B.color = new Color32(brightness, brightness, brightness, 255);
    }

    private void ChangeMedal3Lamp(byte brightness)
    {
        medal3A.color = new Color32(brightness, brightness, brightness, 255);
        medal3B.color = new Color32(brightness, brightness, brightness, 255);
    }

    public void UpdateMedalPanel(int currentBet, int lastBetAmounts)
    {
        if (currentBet == 0)
        {
            if (lastBetAmounts >= 1)
            {
                ChangeMedal1Lamp(TurnOnValue);
            }
            else
            {
                ChangeMedal1Lamp(TurnOffValue);
            }

            if (lastBetAmounts >= 2)
            {
                ChangeMedal2Lamp(TurnOnValue);
            }
            else
            {
                ChangeMedal2Lamp(TurnOffValue);
            }

            if (lastBetAmounts >= 3)
            {
                ChangeMedal3Lamp(TurnOnValue);
            }
            else
            {
                ChangeMedal3Lamp(TurnOffValue);
            }
        }
        else
        {
            if (currentBet >= 1)
            {
                ChangeMedal1Lamp(TurnOnValue);
            }
            else
            {
                ChangeMedal1Lamp(TurnOffValue);
            }

            if (currentBet >= 2)
            {
                ChangeMedal2Lamp(TurnOnValue);
            }
            else
            {
                ChangeMedal2Lamp(TurnOffValue);
            }

            if (currentBet >= 3)
            {
                ChangeMedal3Lamp(TurnOnValue);
            }
            else
            {
                ChangeMedal3Lamp(TurnOffValue);
            }
        }
    }
}
