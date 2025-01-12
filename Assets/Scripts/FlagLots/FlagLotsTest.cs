using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;
using System;
using UnityEngine;
using ReelSpinGame_Lots.Flag;

public class FlagLotsTest : MonoBehaviour
{
    // フラグ処理のテスト用
    private FlagLots flagLots;

    // イベント
    public event Action DrawLots;


    // Start is called before the first frame update
    void Awake()
    {
        flagLots = new FlagLots(this, 6, 0);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // DrawLots
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            DrawLots?.Invoke();
        }
    }
}

