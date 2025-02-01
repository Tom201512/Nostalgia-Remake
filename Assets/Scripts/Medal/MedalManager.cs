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
        public const int MaxCredit = 50;

        // 最大ベット枚数
        public const int MaxBet = 3;

        // メダル更新の間隔(ミリ秒)
        public const int MedalUpdateTime = 120;

        // 最大払い出し
        public const int MaxPayout = 15;


        // var

        // クレジット枚数
        public int Credits { get; private set; }

        // ベット枚数
        public int CurrentBet { get; private set; }

        // 払い出し枚数
        public int PayoutAmounts { get; private set; }

        // 最高ベット枚数
        public int MaxBetAmounts { get; private set; }

        // 残りベット枚数
        private int remainingBet;

        // 処理用タイマー
        private Timer updateTimer;


        // コンストラクタ
        public MedalManager(int _credits, int _currentMaxBet)
        {
            this.Credits = _credits;
            CurrentBet = 0;
            PayoutAmounts = 0;
            this.MaxBetAmounts = _currentMaxBet;

            // 処理用タイマー作成
            updateTimer = new Timer(MedalUpdateTime);
        }

        // デストラクタ
        ~MedalManager()
        {
            // Timerのストップ
            updateTimer.Stop();
            updateTimer.Elapsed -= InsertMedal;
            updateTimer.Elapsed -= PayoutMedal;
        }


        // func

        // ベット処理開始
        public void StartBet(int amounts)
        {
            // 処理ををしていないかチェック
            if (!updateTimer.Enabled)
            {
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)

                if(amounts != CurrentBet && amounts <= MaxBetAmounts)
                {
                    remainingBet = Math.Clamp(SetRemaining(amounts), 0, MaxBet);

                    // もし現在のベットより少ない枚数ならリセット
                    if(amounts < CurrentBet)
                    {
                       ResetMedal();
                    }

                    Debug.Log("Bet Received:" + remainingBet);

                    updateTimer.Elapsed += InsertMedal;
                    updateTimer.Start();
                }
                else
                {
                    if(amounts > MaxBetAmounts)
                    {
                        Debug.Log("The MAX Bet is now :" + MaxBetAmounts);
                    }
                    else
                    {
                        Debug.Log("You already Bet:" + amounts);
                    }
                }
            }
            else
            {
                Debug.Log("Insert is enabled");
            }
        }

        // 払い出し開始
        public void StartPayout(int amounts)
        {
            // 払い出しをしていないかチェック
            if (!updateTimer.Enabled)
            {
                PayoutAmounts = Math.Clamp(PayoutAmounts + amounts, 0, MaxPayout);
                updateTimer.Elapsed += PayoutMedal;
                updateTimer.Start();
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }

        // メダルリセット
        private void ResetMedal()
        {
            CurrentBet = 0;
            Debug.Log("Reset Bet");
        }

        // 
        private int SetRemaining(int amount)
        {
            // 現在のベット枚数よりも多く賭けると差分だけ掛ける
            // (2BETから1BETはリセットして調整)

            // 多い場合(1枚以上ベットされていること)
            if(amount > CurrentBet && CurrentBet > 0)
            {
                Debug.Log("You bet more than current bet");
                return amount - CurrentBet;
            }

            // 少ない場合
            // 0枚ならそのまま
            return amount;
        }


        // コルーチン用

        // 投入処理
        private void InsertMedal(object sender, ElapsedEventArgs e)
        {
            remainingBet -= 1;
            Debug.Log("Bet Medal by 1");
            CurrentBet += 1;

            // 全て払い出したら処理終了
            if (remainingBet <= 0)
            {
                updateTimer.Stop();
                updateTimer.Elapsed -= InsertMedal;

                Debug.Log("Bet Finished");
                Debug.Log("CurrentBet:" + CurrentBet);
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

                Debug.Log("Payout Finished");
            }
        }
    }
}
