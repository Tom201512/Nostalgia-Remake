using UnityEngine;

namespace ReelSpinGame_Lamps
{
    // メダルパネル部分
    public class MedalPanel : MonoBehaviour
    {
        [SerializeField] private LampComponent medal1;          // 1枚掛けランプ
        [SerializeField] private LampComponent medal2Up;        // 2枚掛けランプ上
        [SerializeField] private LampComponent medal2Down;      // 2枚掛けランプ下
        [SerializeField] private LampComponent medal3Up;        // 3枚掛けランプ上
        [SerializeField] private LampComponent medal3Down;      // 3枚掛けランプ下

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // ベット枚数からどのランプをつけるか判定する
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
                    medal2Up.TurnOn();
                    medal2Down.TurnOn();
                }
                else
                {
                    medal2Up.TurnOff();
                    medal2Down.TurnOff();
                }

                if (lastBetAmount >= 3)
                {
                    medal3Up.TurnOn();
                    medal3Down.TurnOn();
                }
                else
                {
                    medal3Up.TurnOff();
                    medal3Down.TurnOff();
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
                medal2Up.TurnOn();
                medal2Down.TurnOn();
            }
            else
            {
                medal2Up.TurnOff();
                medal2Down.TurnOff();
            }

            if (currentBet >= 3)
            {
                medal3Up.TurnOn();
                medal3Down.TurnOn();
            }
            else
            {
                medal3Up.TurnOff();
                medal3Down.TurnOff();
            }
        }

        // 全てのランプを消す
        public void TurnOffAllLamps()
        {
            medal1.TurnOff();
            medal2Up.TurnOff();
            medal2Down.TurnOff();
            medal3Up.TurnOff();
            medal3Down.TurnOff();
        }
    }
}
