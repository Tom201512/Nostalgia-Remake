using UnityEngine;

public class MedalPanel : MonoBehaviour
{
    // メダルパネル部分
    // var
    // メダル1枚ランプ
    [SerializeField] private LampComponent medal1;
    // メダル2枚ランプA(上)
    [SerializeField] private LampComponent medal2A;
    // メダル2枚ランプB(下)
    [SerializeField] private LampComponent medal2B;
    // メダル3枚ランプA(上)
    [SerializeField] private LampComponent medal3A;
    // メダル3枚ランプB(下)
    [SerializeField] private LampComponent medal3B;

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    // ベット枚数からどのランプをつけるか判定する
    public void UpdateLampByBet(int currentBet, int lastBetAmounts)
    {
        if (currentBet == 0)
        {
            if (lastBetAmounts >= 1)
            {
                medal1.TurnOn();
            }
            else
            {
                medal1.TurnOff();
            }

            if (lastBetAmounts >= 2)
            {
                medal2A.TurnOn();
                medal2B.TurnOn();
            }
            else
            {
                medal2A.TurnOff();
                medal2B.TurnOff();
            }

            if (lastBetAmounts >= 3)
            {
                medal3A.TurnOn();
                medal3B.TurnOn();
            }
            else
            {
                medal3A.TurnOff();
                medal3B.TurnOff();
            }
        }

        if (currentBet >= 1)
        {
            medal1.TurnOn();
        }
        else
        {
            medal1.TurnOff();
        }

        if (currentBet >= 2)
        {
            medal2A.TurnOn();
            medal2B.TurnOn();
    }
        else
        {
            medal2A.TurnOff();
            medal2B.TurnOff();
        }

        if (currentBet >= 3)
        {
            medal3A.TurnOn();
            medal3B.TurnOn();
        }
        else
        {
            medal3A.TurnOff();
            medal3B.TurnOff();
        }
    }

    public void TurnOffAllLamps()
    {
        medal1.TurnOff();
        medal2A.TurnOff();
        medal2B.TurnOff();
        medal3A.TurnOff();
        medal3B.TurnOff();
    }
}
