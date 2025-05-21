using ReelSpinGame_System;
using System;
using System.Timers;
using UnityEngine;
using ReelSpinGame_Datas;

namespace ReelSpinGame_Medal
{
    //スロット内部のメダル管理
    public class MedalManager
    {
        // const
        // 最大クレジット枚数
        public const int MaxCredit = 50;
        // 最小クレジット枚数(0より少ない場合はセグに表示はしない)
        public const int MinCredit = -3;
        // 最大ベット枚数
        public const int MaxBet = 3;
        // メダル更新の間隔(ミリ秒)
        public const int MedalUpdateTime = 120;
        // 最大払い出し
        public const int MaxPayout = 15;

        // var
        // 残りベット枚数
        private int remainingBet;
        // 処理用タイマー
        public Timer UpdateTimer { get; private set; }

        // クレジット枚数
        public int Credits { get; private set; }
        // ベット枚数
        public int CurrentBet { get; private set; }
        // 払い出し枚数
        public int PayoutAmounts { get; private set; }
        // 最高ベット枚数
        public int MaxBetAmounts { get; private set; }
        // 最後にかけたメダル枚数
        public int LastBetAmounts { get; private set; }
        // リプレイ状態か
        public bool HasReplay { get; private set; }

        // 受け取る側のメダルデータ
        private MedalData receiverMedalData;

        // コンストラクタ
        public MedalManager(int credits, int curretMaxBet, int lastBetAmounts, bool hasReplay, MedalData receiverMedalData)
        {
            Credits = credits;
            CurrentBet = 0;
            PayoutAmounts = 0;
            MaxBetAmounts = curretMaxBet;
            LastBetAmounts = 0;
            HasReplay = hasReplay;
            // 処理用タイマー作成
            UpdateTimer = new Timer(MedalUpdateTime);

            // プレイヤーのメダルデータを得る
            this.receiverMedalData = receiverMedalData;
        }

        // デストラクタ
        ~MedalManager()
        {
            // Timerのストップ
            UpdateTimer.Stop();
            UpdateTimer.Dispose();
        }

        // func
        // MAXベット枚数変更
        public void ChangeMaxBet(int maxBet)
        {
            Debug.Log("Change MAXBET:" + maxBet);
            MaxBetAmounts = maxBet;
        }

        // クレジット枚数を増やす
        public void ChangeCredit(int amount) => Credits = Math.Clamp(Credits += amount, MinCredit, MaxCredit);

        // MAX_BET用の処理
        public void StartMAXBet()
        {
            Debug.Log("Received MAX_BET");
            StartBet(MaxBetAmounts);
        }

        // ベット処理開始
        public void StartBet(int amounts)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!HasReplay && !UpdateTimer.Enabled)
            {
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)
                if(amounts != CurrentBet && amounts <= MaxBetAmounts)
                {
                    SetRemaining(amounts);

                    // もし現在のベットより少ない枚数ならリセット
                    if(amounts < CurrentBet)
                    {
                        // ベットで使ったクレジット分を返す
                        ChangeCredit(CurrentBet);
                        ResetMedal();
                    }

                    Debug.Log("Bet Received:" + remainingBet);

                    // メダルの投入を開始する(残りはフレーム処理)
                    LastBetAmounts = amounts;
                    InsertMedal();
                    UpdateTimer.Elapsed += UpdateInsert;
                    UpdateTimer.Start();
                }

                // ベットがすでに終わっている、またはMAXベットの場合(Debug)
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

            // 処理中でメダルが入れられない場合
            else
            {
                if(HasReplay)
                {
                    Debug.Log("Replay is enabled");
                }
                else
                {
                    Debug.Log("Insert is enabled");
                }
            }
        }

        // 払い出し開始
        public void StartPayout(int amounts)
        {
            // 払い出しをしていないかチェック
            if (!UpdateTimer.Enabled)
            {
                // クレジット枚数が0より少ないかチェック
                if(Credits < 0)
                {
                    Credits = 0;
                }

                PayoutAmounts = Math.Clamp(PayoutAmounts + amounts, 0, MaxPayout);

                // メダルの払い出しを開始する(残りはフレーム処理)

                if (amounts > 0)
                {
                    PayoutMedal();
                    UpdateTimer.Elapsed += UpdatePayout;
                    UpdateTimer.Start();
                }
                else
                {
                    Debug.Log("No Payouts");
                }
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }

        // 最後に掛けた枚数を反映させる
        public void ApplyLastBetToReceiver()
        {
            receiverMedalData.DecreasePlayerMedal(LastBetAmounts);
            receiverMedalData.IncreaseInMedal(LastBetAmounts);
        }

        // メダルリセット
        public void ResetMedal()
        {
            CurrentBet = 0;
            Debug.Log("Reset Bet");
        }

        // リプレイ状態にする(前回と同じメダル枚数をかける)
        public void SetReplay()
        {
            Debug.Log("Enable Replay" + LastBetAmounts);
            SetRemaining(LastBetAmounts);
            receiverMedalData.IncreaseOutMedal(3);
            InsertMedal();
            UpdateTimer.Elapsed += UpdateInsert;
            UpdateTimer.Start();

            HasReplay = true;
        }

        // リプレイ状態解除
        public void DisableReplay()
        {
            HasReplay = false;
            LastBetAmounts = 0;
        }

        // 残りベット枚数を設定
        private void SetRemaining(int amount)
        {
            // 現在のベット枚数よりも多く賭けると差分だけ掛ける
            // (2BETから1BETはリセットして調整)

            // 多い場合(1枚以上ベットされていること)
            if(amount > CurrentBet && CurrentBet > 0)
            {
                Debug.Log("You bet more than current bet");
                remainingBet = Math.Clamp(amount - CurrentBet, 0, MaxBet);
            }

            // 少ない場合
            else
            {
                // 0枚ならそのまま
                remainingBet = Math.Clamp(amount, 0, MaxBet);
            }
        }

        // コルーチン用

        private void UpdateInsert(object sender, ElapsedEventArgs e)
        {
            // 投入処理
            if (remainingBet > 0)
            {
                InsertMedal();
            }
            // 全て投入したら処理終了
            else
            {
                UpdateTimer.Stop();
                UpdateTimer.Elapsed -= UpdateInsert;

                Debug.Log("Bet Finished");
                Debug.Log("CurrentBet:" + CurrentBet);
            }
        }

        private void UpdatePayout(object sender, ElapsedEventArgs e)
        {
            // 払い出し処理
            if (PayoutAmounts > 0)
            {
                PayoutMedal();
            }
            // 全て払い出したら処理終了
            else
            {
                UpdateTimer.Stop();
                UpdateTimer.Elapsed -= UpdatePayout;

                Debug.Log("Payout Finished");
            }
        }

        // 投入処理
        private void InsertMedal()
        {
            remainingBet -= 1;
            Debug.Log("Remaining:" + remainingBet);
            Debug.Log("Bet Medal by 1");
            CurrentBet += 1;

            if (!HasReplay)
            {
                ChangeCredit(-1);
            }
        }

        // 払い出し処理
        private void PayoutMedal()
        {
            PayoutAmounts -= 1;
            ChangeCredit(1);
            receiverMedalData.IncreasePlayerMedal(1);
            receiverMedalData.IncreaseOutMedal(1);
            Debug.Log("Payout Medal by 1");
        }
    }
}
