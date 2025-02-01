using UnityEngine;

namespace ReelSpinGame_Lots.FlagCounter
{
    public class FlagCounter
    {
        // �����J�E���^

        // const

        //�����J�E���^�����l
        const int CounterDecrease = 256;

        //�����J�E���^�����l
        const int CounterIncreaseDefault = 100;

        // �ݒ�5��
        const int CounterIncrease5 = 104;

        // �ݒ�6��
        const int CounterIncrease6 = 108;


        // var

        // �����J�E���^
        public int Counter { get; private set; }


        // �R���X�g���N�^
        public FlagCounter(int Counter)
        {
            this.Counter = Counter;
        }


        // func

        // �����J�E���^�̑���
        public void IncreaseCounter(int payoutAmounts)
        {
            Counter += CounterDecrease * payoutAmounts;
            Debug.Log("Increased Counter Current:" + Counter);
        }

        // �����J�E���^�̌���
        public void DecreaseCounter(int settingNum, int betAmounts)
        {
            // �ݒ�5
            if(settingNum == 5)
            {
                Counter -= betAmounts * CounterIncrease5;
            }

            // �ݒ�6
            else if(settingNum == 6)
            {
                Counter -= betAmounts * CounterIncrease6;
            }

            // ����ȊO
            else
            {
                Counter -= betAmounts * CounterIncreaseDefault;
            }

            Debug.Log("Decreased Counter Current:" + Counter);
        }
    }
}
