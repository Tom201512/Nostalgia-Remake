using System;
using UnityEngine;

namespace ReelSpinGame_Medal
{
	public class MedalBehavior
	{
        // メダルの処理

        // const 
        // 最大クレジット枚数
        public const int MaxCredit = 50;
        // 最小クレジット枚数(0より少ない場合はセグに表示はしない)
        public const int MinCredit = -3;
        // 最大ベット枚数
        public const int MaxBetLimit = 3;
        // 最大払い出し
        public const int MaxPayout = 15;

        // var

        // 残りベット枚数
        public int RemainingBet { get; set; }
        // ベット枚数
        public int CurrentBet { get; set; }
        // 払い出し枚数
        public int RemainingPayouts { get; set; }
        // 最後に払い出したメダル枚数
        public int LastPayoutAmounts { get; set; }
        // ベット完了しているか
        public bool FinishedBet { get;  set; }

        // セーブ用のデータ
        public class MedalSystemData
        {
            // var
            // クレジット枚数
            public int Credits { get; set; }
            // 最高ベット枚数
            public int MaxBetAmounts { get; set; }
            // 最後にかけたメダル枚数
            public int LastBetAmounts { get; set; }
            // リプレイ状態か
            public bool HasReplay { get; set; }

            public MedalSystemData()
            {
                Credits = 0;
                MaxBetAmounts = MaxBetLimit;
                LastBetAmounts = 0;
                HasReplay = false;
            }
        }

        // メダル情報(セーブに使う)
        public MedalSystemData system { get; set; }

        // コンストラクタ
        public MedalBehavior()
        {
            CurrentBet = 0;
            RemainingPayouts = 0;
            LastPayoutAmounts = 0;
            FinishedBet = false;

            system = new MedalSystemData();
        }

        // func

        // 残りベット枚数を設定
        public void SetRemainingBet(int amounts)
        {
            FinishedBet = false;
            // 最後に払い出したメダルリセット
            LastPayoutAmounts = 0;

            // 現在のベット枚数よりも多く賭けると差分だけ掛ける
            // (2BETから1BETはリセットして調整)

            // 多い場合(1枚以上ベットされていること)
            if (amounts > CurrentBet && CurrentBet > 0)
            {
                ////Debug.Log("You bet more than current bet");
                RemainingBet = Math.Clamp(amounts - CurrentBet, 0, MaxBetLimit);
            }

            // 少ない場合
            else
            {
                // 0枚ならそのまま
                RemainingBet = Math.Clamp(amounts, 0, MaxBetLimit);
            }

            // もし現在のベットより少ない枚数ならリセット
            if (amounts < CurrentBet)
            {
                // ベットで使ったクレジット分を返す
                system.Credits = Math.Clamp(system.Credits += CurrentBet, MinCredit, MaxCredit);
                CurrentBet = 0;
            }
            system.LastBetAmounts = amounts;
        }

        // 投入処理
        public void InsertOneMedal()
        {
            RemainingBet -= 1;
            CurrentBet += 1;

            // リプレイでなければクレジットを減らす
            if (!system.HasReplay)
            {
                system.Credits = Math.Clamp(system.Credits -= 1, MinCredit, MaxCredit);
            }

            // 残り枚数が0になったら終了
            if(RemainingBet == 0)
            {
                FinishedBet = true;
            }
        }

        // 払い出し処理
        public void PayoutOneMedal()
        {
            RemainingPayouts -= 1;
            LastPayoutAmounts += 1;
        }

        // クレジットの増減
        public void ChangeCredits(int value)
        {
            // クレジット枚数を0枚にする(負数の場合)
            if (system.Credits < 0)
            {
                system.Credits = 0;
            }
            system.Credits = Math.Clamp(system.Credits += value, MinCredit, MaxCredit);
        }
    }
}