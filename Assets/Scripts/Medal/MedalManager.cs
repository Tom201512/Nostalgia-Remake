using System;
using System.Timers;
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

        // メダル更新の間隔(ミリ秒)
        public const int MEDAL_UPDATETIME = 120;

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

        // 処理用タイマー
        Timer updateTimer;


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
            this.medalTest.BetMedal += BetMedals;
            this.medalTest.BetMax += BetMaxMedals;
            this.medalTest.StartPayout += StartPayout;


            // 処理用タイマー作成
            updateTimer = new Timer(MEDAL_UPDATETIME);
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

        private void StartPayout(int amounts)
        {
            // 払い出しをしていないかチェック
            if(!updateTimer.Enabled)
            {
                PayoutAmounts = Math.Clamp(PayoutAmounts + amounts, 0, MAX_PAYOUT);
                updateTimer.Elapsed += PayoutMedal;
                updateTimer.Start();
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }


        // 払い出し処理
        private void PayoutMedal(object sender, ElapsedEventArgs e)
        {
            PayoutAmounts -= 1;
            Debug.Log("Payout Medal by 1");

            // 全て払い出したら処理終了
            if(PayoutAmounts == 0) 
            { 
                updateTimer.Stop();
                updateTimer.Elapsed -= PayoutMedal;
            }
        }
    }
}
