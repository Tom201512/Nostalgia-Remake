using UnityEngine;

public class MedalPanel : MonoBehaviour
{
    // ���_���p�l������
    // var
    // ���_��1�������v
    [SerializeField] private LampComponent medal1;
    // ���_��2�������vA(��)
    [SerializeField] private LampComponent medal2A;
    // ���_��2�������vB(��)
    [SerializeField] private LampComponent medal2B;
    // ���_��3�������vA(��)
    [SerializeField] private LampComponent medal3A;
    // ���_��3�������vB(��)
    [SerializeField] private LampComponent medal3B;

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    // �x�b�g��������ǂ̃����v�����邩���肷��
    public void UpdateLampByBet(int currentBet, int lastBetAmount)
    {
        if (currentBet == 0)
        {
            if (lastBetAmount >= 1)
            {
                medal1.TurnOn();
            }
            else
            {
                medal1.TurnOff();
            }

            if (lastBetAmount >= 2)
            {
                medal2A.TurnOn();
                medal2B.TurnOn();
            }
            else
            {
                medal2A.TurnOff();
                medal2B.TurnOff();
            }

            if (lastBetAmount >= 3)
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
