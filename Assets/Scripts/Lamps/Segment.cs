using UnityEngine;

public class Segment : MonoBehaviour
{
    // 7セグ

    // const
    // 各ランプの対応位置
    public enum LampID {A, B, C, D, E, F, G}

    // var 
    // 各セグメントのランプ
    private LampComponent[] lamps;

    // func
    private void Awake()
    {
        lamps = GetComponentsInChildren<LampComponent>();
        Debug.Log("Lamp count:" + lamps.Length);
    }

    public void TurnOffAll()
    {
        foreach(LampComponent lamp in lamps)
        {
            lamp.TurnOff();
        }
    }

    public void TurnOnLampByNumber(int num)
    {
        LampComponent[] turnOnLamps;
        LampComponent[] turnOffLamps;

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

            turnOffLamps = new LampComponent[] 
            { 
                lamps[(int)LampID.G] 
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
            return;
        }
        // 1(b,c)
        if(num == 1)
        {
            turnOnLamps = new LampComponent[]
            {
                lamps[(int)LampID.B],
                lamps[(int)LampID.C],
            };

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.A],
                lamps[(int)LampID.D],
                lamps[(int)LampID.E],
                lamps[(int)LampID.F],
                lamps[(int)LampID.G]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.C],
                lamps[(int)LampID.F]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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
                lamps[(int)LampID.G]
            };

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.E],
                lamps[(int)LampID.F]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.A],
                lamps[(int)LampID.D],
                lamps[(int)LampID.E]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.B],
                lamps[(int)LampID.E]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.B]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[]
            {
                lamps[(int)LampID.D],
                lamps[(int)LampID.E],
                lamps[(int)LampID.F],
                lamps[(int)LampID.G]
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[] 
            {

            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
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

            turnOffLamps = new LampComponent[] 
            { 
                lamps[(int)LampID.E],
            };

            TurnOnLamps(turnOnLamps, turnOffLamps);
            return;
        }

        Debug.Log("Invalid Number");
        TurnOffAll();
    }

    private void TurnOnLamps(LampComponent[] turnOnLamps, LampComponent[] turnOffLamps)
    {
        foreach (LampComponent lamp in turnOnLamps)
        {
            lamp.TurnOn();
        }
        foreach (LampComponent lamp in turnOffLamps)
        {
            lamp.TurnOff();
        }
    }
}
