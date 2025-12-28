using System;

namespace ReelSpinGame_Medal
{
    // メダルの処理
    public class MedalBehavior
    {
        public const int MaxCredit = 50;        // 最大クレジット枚数
        public const int MinCredit = -3;        // 最小クレジット枚数
        public const int MaxBetLimit = 3;       // 最大ベット枚数
        public const int MaxPayout = 15;        // 最大払い出し

        public int RemainingBet { get; set; }       // 残りベット枚数
        public int CurrentBet { get; set; }         // ベット枚数
        public int RemainingPayout { get; set; }    // 払い出し枚数
        public int LastPayoutAmount { get; set; }   // 最後に払い出したメダル枚数
        public bool FinishedBet { get; set; }      // ベット完了しているか

        // セーブ用のデータ
        public class MedalSystemData
        {
            public int Credit { get; set; }             // クレジット枚数
            public int MaxBetAmount { get; set; }       // 最高ベット枚数
            public int LastBetAmount { get; set; }      // 最後にかけたメダル枚数
            public bool HasReplay { get; set; }         // リプレイ状態か

            public MedalSystemData()
            {
                Credit = 0;
                MaxBetAmount = MaxBetLimit;
                LastBetAmount = 0;
                HasReplay = false;
            }
        }

        public MedalSystemData system { get; set; }     // メダル情報データ

        public MedalBehavior()
        {
            CurrentBet = 0;
            RemainingPayout = 0;
            LastPayoutAmount = 0;
            FinishedBet = false;

            system = new MedalSystemData();
        }

        // 残りベット枚数を設定
        public void SetRemainingBet(int amount)
        {
            FinishedBet = false;
            // 最後に払い出したメダルリセット
            LastPayoutAmount = 0;

            // 現在のベット枚数よりも多く賭けると差分だけ掛ける
            // (2BETから1BETはリセットして調整)

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
                system.Credit = Math.Clamp(system.Credit += CurrentBet, MinCredit, MaxCredit);
                CurrentBet = 0;
            }
            system.LastBetAmount = amount;
        }

        // 投入処理
        public void InsertOneMedal()
        {
            RemainingBet -= 1;
            CurrentBet += 1;

            // リプレイでなければクレジットを減らす
            if (!system.HasReplay)
            {
                system.Credit = Math.Clamp(system.Credit -= 1, MinCredit, MaxCredit);
            }

            // 残り枚数が0になったら終了
            if (RemainingBet == 0)
            {
                FinishedBet = true;
            }
        }

        // 払い出し処理
        public void PayoutOneMedal()
        {
            RemainingPayout -= 1;
            LastPayoutAmount += 1;
        }

        // クレジットの増減
        public void ChangeCredit(int value)
        {
            // クレジット枚数を0枚にする(負数の場合)
            if (system.Credit < 0)
            {
                system.Credit = 0;
            }
            system.Credit = Math.Clamp(system.Credit += value, MinCredit, MaxCredit);
        }
    }
}