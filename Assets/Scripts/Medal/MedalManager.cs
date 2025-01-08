using ReelSpinGame_Interface;
using ReelSpinGame_Observing;
using System;
using UnityEngine;

namespace ReelSpinGame_Medal
{
    //スロット内部のメダル管理
    public class MedalManager
    {
        // const

        // 最大クレジット枚数
        public const int MAX_CREDITS = 50;

        // 最大ベット枚数
        public const int MAX_BET = 3;

        // メダル更新のフレーム数
        public const float MEDAL_UPDATETIME = 0.12f;

        // 最大払い出し
        public const int MAX_PAYOUT = 15;


        // val

        // クレジット枚数
        public int Credits { get; private set; }

        // ベット枚数
        public int BetAmounts { get; private set; }

        // 払い出し枚数
        public int PayoutAmounts { get; private set; }

        // 最高ベット枚数
        public int MaxBetAmounts { get; private set; }


        // イベント用
        private MedalTest medalTest;


        public MedalManager(int _credits, int _currentMaxBet, MedalTest _medalTest)
        {
            this.Credits = _credits;
            BetAmounts = 0;
            PayoutAmounts = 0;
            this.MaxBetAmounts = _currentMaxBet;

            this.medalTest = _medalTest;

            // イベントを受ける
            this.medalTest.betMedal += BetMedals;
            this.medalTest.betMax += BetMaxMedals;
        }

        // func

        // ベット処理
        private void BetMedals(int amounts)
        {
            BetAmounts = Math.Clamp(BetAmounts + amounts, 0, MAX_BET);
            Debug.Log("Bet finished Current Bet:" + BetAmounts);
        }

        private void BetMaxMedals()
        {
            if(BetAmounts < MAX_BET)
            {
                BetAmounts = MAX_BET;
                Debug.Log("Bet finished Current Bet:" + BetAmounts);
            }

            else
            {
                Debug.Log("Already reached Max Bet:" + MaxBetAmounts);
            }
        }
    }
}
