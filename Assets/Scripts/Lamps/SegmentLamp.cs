using UnityEngine;

namespace ReelSpinGame_Lamps
{
    // 7セグ
    public class SegmentLamp : MonoBehaviour
    {
        enum LampID { A, B, C, D, E, F, G }        // 各ランプの対応位置

        private LampComponent[] lamps;        // 各セグメントのランプ

        void Awake()
        {
            lamps = GetComponentsInChildren<LampComponent>();
        }

        // 全て点灯
        public void TurnOffAll()
        {
            foreach (LampComponent lamp in lamps)
            {
                lamp.TurnOff();
            }
        }

        // 数字に合わせたランプを点灯
        public void TurnOnLampByNumber(int num)
        {
            TurnOffAll();
            LampComponent[] turnOnLamps;

            // 数字に合わせて点灯、消灯させるセグの配列を作る
            // 0(a,b,c,d,e,f)
            if (num == 0)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.E],
                    lamps[(int)LampID.F],
                };

                TurnOnLamps(turnOnLamps);
                return;
            }
            // 1(b,c)
            if (num == 1)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C],
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 2(a,b,d,e,g)
            if (num == 2)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.E],
                    lamps[(int)LampID.G]
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 3(a,b,c,d,g)
            if (num == 3)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.G],
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 4(b,c,f,g)
            if (num == 4)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.F],
                    lamps[(int)LampID.G]
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 5(a,c,d,f,g)
            if (num == 5)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.F],
                    lamps[(int)LampID.G]
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 6(a,c,d,e,f,g)
            if (num == 6)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.E],
                    lamps[(int)LampID.F],
                    lamps[(int)LampID.G]
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 7(a,b,c)
            if (num == 7)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C]
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 8(a,b,c,d,e,f,g)
            if (num == 8)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.E],
                    lamps[(int)LampID.F],
                    lamps[(int)LampID.G],
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            // 9(a,b,c,d,f,g,)
            if (num == 9)
            {
                turnOnLamps = new LampComponent[]
                {
                    lamps[(int)LampID.A],
                    lamps[(int)LampID.B],
                    lamps[(int)LampID.C],
                    lamps[(int)LampID.D],
                    lamps[(int)LampID.F],
                    lamps[(int)LampID.G],
                };

                TurnOnLamps(turnOnLamps);
                return;
            }

            TurnOffAll();
        }

        // ハイフンを表示
        public void TurnOnBar()
        {
            TurnOffAll();
            LampComponent[] turnOnLamps = new LampComponent[]
            {
                lamps[(int)LampID.G]
            };

            TurnOnLamps(turnOnLamps);
        }

        // Jを表示(JACGAME中)
        public void TurnOnJAC()
        {
            TurnOffAll();
            LampComponent[] turnOnLamps = new LampComponent[]
            {
                lamps[(int)LampID.B],
                lamps[(int)LampID.C],
                lamps[(int)LampID.D],
                lamps[(int)LampID.E],
            };

            TurnOnLamps(turnOnLamps);
        }

        // Eを表示
        public void TurnOnE()
        {
            TurnOffAll();
            LampComponent[] turnOnLamps = new LampComponent[]
            {
                lamps[(int)LampID.A],
                lamps[(int)LampID.D],
                lamps[(int)LampID.E],
                lamps[(int)LampID.F],
                lamps[(int)LampID.G],
            };

            TurnOnLamps(turnOnLamps);
        }

        // Rを表示
        public void TurnOnR()
        {
            TurnOffAll();
            LampComponent[] turnOnLamps = new LampComponent[]
            {
                lamps[(int)LampID.E],
                lamps[(int)LampID.G],
            };

            TurnOnLamps(turnOnLamps);
        }

        // 指定箇所のランプを点灯
        private void TurnOnLamps(LampComponent[] turnOnLamps)
        {
            foreach (LampComponent lamp in turnOnLamps)
            {
                lamp.TurnOn();
            }
        }
    }
}
