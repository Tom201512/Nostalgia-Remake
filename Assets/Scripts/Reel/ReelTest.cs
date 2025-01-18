using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelTest : MonoBehaviour
{
    // リール処理のテスト用

    // var
    [SerializeField] private SymbolSpin[] symbolSpins;
    private ReelManager manager;

    // Use this for initialization
    void Start()
    {
        manager = new ReelManager();
    }

    // Update is called once per frame
    void Update()
    {

    }
}