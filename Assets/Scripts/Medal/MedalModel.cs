using System;

namespace ReelSpinGame_Medal
{
    // メダルの処理
    public class MedalModel
    {
        public const int MaxCredit = 50;        // 最大クレジット枚数
        public const int MaxBetLimit = 3;       // 最大ベット枚数
        public const int MaxPayout = 15;        // 最大払い出し

        public int RemainingBet { get; set; }       // 残りベット枚数
        public int CurrentBet { get; set; }         // ベット枚数
        public int RemainingPayout { get; set; }    // 払い出し枚数
        public int LastPayoutAmount { get; set; }   // 最後に払い出したメダル枚数
        public bool IsFinishedBet { get; set; }     // ベット完了しているか

        public MedalSystemData Data { get; set; }     // メダル情報データ

        public MedalModel()
        {
            CurrentBet = 0;
            RemainingPayout = 0;
            LastPayoutAmount = 0;
            IsFinishedBet = false;
            Data = new MedalSystemData();
        }

        // 残りベット枚数を設定
        public void SetRemainingBet(int amount)
        {
            IsFinishedBet = false;
            // 最後に払い出したメダルリセット
            LastPayoutAmount = 0;

            // 現在のベット枚数と比べてベット枚数を調整する
            // 多い場合(1枚以上ベットされていること)
            if (amount > CurrentBet && CurrentBet > 0)
            {
                RemainingBet = Math.Clamp(amount - CurrentBet, 0, MaxBetLimit);
            }

            // 少ない場合
            else
            {
                // 0枚ならそのまま
                RemainingBet = Math.Clamp(amount, 0, MaxBetLimit);
            }

            // もし現在のベットより少ない枚数ならリセット
            if (amount < CurrentBet)
            {
                // ベットで使ったクレジット分を返す
                Data.Credit = Math.Clamp(Data.Credit += CurrentBet, 0, MaxCredit);
                CurrentBet = 0;
            }
            Data.LastBetAmount = amount;
        }

        // クレジットの増減
        public void ChangeCredit(int value)
        {
            // クレジット枚数を0枚にする(負数の場合)
            if (Data.Credit < 0)
            {
                Data.Credit = 0;
            }
            Data.Credit = Math.Clamp(Data.Credit += value, 0, MaxCredit);
        }

        // 投入処理
        public void InsertOneMedal()
        {
            RemainingBet -= 1;
            CurrentBet += 1;

            // リプレイでなければクレジットを減らす
            if (!Data.HasReplay)
            {
                Data.Credit = Math.Clamp(Data.Credit -= 1, 0, MaxCredit);
            }

            // 残り枚数が0になったら終了
            if (RemainingBet == 0)
            {
                IsFinishedBet = true;
            }
        }

        // 払い出し処理
        public void PayoutOneMedal()
        {
            RemainingPayout -= 1;
            LastPayoutAmount += 1;
        }
    }
}