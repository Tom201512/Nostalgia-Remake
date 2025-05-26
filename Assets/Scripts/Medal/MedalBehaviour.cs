using System;
using UnityEngine;

namespace ReelSpinGame_Medal
{
	public class MedalBehaviour
	{
        // メダルの処理

        // const 
        // 最大クレジット枚数
        public const int MaxCredit = 50;
        // 最小クレジット枚数(0より少ない場合はセグに表示はしない)
        public const int MinCredit = -3;
        // 最大ベット枚数
        public const int MaxBet = 3;
        // 最大払い出し
        public const int MaxPayout = 15;

        // var
        // 残りベット枚数
        public int RemainingBet { get; private set; }
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
        // 最後に払い出したメダル枚数
        public int LastPayoutAmounts {get; private set; }
        // リプレイ状態か
        public bool HasReplay { get; private set; }

        // コンストラクタ
        public MedalBehaviour(int credits, int curretMaxBet, int lastBetAmounts, bool hasReplay)
        {
            CurrentBet = 0;
            PayoutAmounts = 0;
            LastPayoutAmounts = 0;

            Credits = credits;
            MaxBetAmounts = curretMaxBet;
            LastBetAmounts = lastBetAmounts;
            HasReplay = hasReplay;
        }

        // func
        // MAXベット枚数変更
        public void ChangeMaxBet(int maxBet) => MaxBetAmounts = maxBet;

        // 払い出し枚数変更
        public void ChangePayoutAmounts(int amounts) => PayoutAmounts = Math.Clamp(amounts, 0, MaxPayout);

        // クレジット枚数を増やす
        public void ChangeCredit(int amounts) => Credits = Math.Clamp(Credits += amounts, MinCredit, MaxCredit);

        // メダルリセット
        public void ResetMedal() => CurrentBet = 0;


        // リプレイにする
        public void EnableReplay()
        {
            HasReplay = true;
            SetRemainingBet(LastBetAmounts);
        }

        // リプレイ状態解除
        public void DisableReplay()
        {
            HasReplay = false;
            LastBetAmounts = 0;
        }

        // 残りベット枚数を設定
        public void SetRemainingBet(int amounts)
        {
            // 最後に払い出したメダルリセット
            LastPayoutAmounts = 0;

            // 現在のベット枚数よりも多く賭けると差分だけ掛ける
            // (2BETから1BETはリセットして調整)

            // 多い場合(1枚以上ベットされていること)
            if (amounts > CurrentBet && CurrentBet > 0)
            {
                Debug.Log("You bet more than current bet");
                RemainingBet = Math.Clamp(amounts - CurrentBet, 0, MaxBet);
            }

            // 少ない場合
            else
            {
                // 0枚ならそのまま
                RemainingBet = Math.Clamp(amounts, 0, MaxBet);
            }

            // もし現在のベットより少ない枚数ならリセット
            if (amounts < CurrentBet)
            {
                // ベットで使ったクレジット分を返す
                ChangeCredit(CurrentBet);
                ResetMedal();
            }
            LastBetAmounts = amounts;
            Debug.Log("Bet Received:" + RemainingBet);
        }

        // 投入処理
        public void InsertOneMedal()
        {
            RemainingBet -= 1;
            Debug.Log("Remaining:" + RemainingBet);
            Debug.Log("Bet Medal by 1");
            CurrentBet += 1;
           //HasMedalInserted.Invoke(1);

            // リプレイでなければクレジットを減らす
            if (!HasReplay)
            {
                ChangeCredit(-1);
            }
        }

        // 払い出し処理
        public void PayoutOneMedal()
        {
            // クレジット枚数を0枚にする(負数の場合)
            if (Credits < 0)
            {
                Credits = 0;
            }
            PayoutAmounts -= 1;
            // リプレイでなければ最後に払い出した枚数を増やす
            if(!HasReplay)
            {
                LastPayoutAmounts += 1;
            }
            // クレジット変更
            ChangeCredit(1);
            Debug.Log("Payout Medal by:" + 1);
        }
    }
}