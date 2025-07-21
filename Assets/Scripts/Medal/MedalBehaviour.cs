using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
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
        public const int MaxBetLimit = 3;
        // 最大払い出し
        public const int MaxPayout = 15;

        // var

        // セーブ不要な情報
        // 残りベット枚数
        public int RemainingBet { get; set; }
        // ベット枚数
        public int CurrentBet { get; set; }
        // 払い出し枚数
        public int RemainingPayouts { get; set; }
        // 最後に払い出したメダル枚数
        public int LastPayoutAmounts {get; set; }
        // ベット完了しているか
        public bool FinishedBet { get;  set; }

        // セーブ必要な情報(セーブデータからアドレスをもらう)
        public MedalSystemSave MedalSave { get; set; }

        // セーブをする情報
        public class MedalSystemSave : ISavable
        {
            // const

            // var
            // クレジット枚数
            public int Credits { get; set; }
            // 最高ベット枚数
            public int MaxBetAmounts { get; set; }
            // 最後にかけたメダル枚数
            public int LastBetAmounts { get; set; }
            // リプレイ状態か
            public bool HasReplay { get; set; }

            public MedalSystemSave()
            {
                Credits = 0;
                MaxBetAmounts = MaxBetLimit;
                LastBetAmounts = 0;
                HasReplay = false;
            }

            // データ書き込み
            public List<int> SaveData()
            {
                // 変数を格納
                List<int> data = new List<int>();
                data.Add(Credits);
                data.Add(MaxBetAmounts);
                data.Add(LastBetAmounts);
                data.Add(HasReplay ?  1 : 0);

                return data;
            }

            // データ読み込み
            public bool LoadData(BinaryReader bStream)
            {
                try
                {
                    // クレジット枚数
                    Credits = bStream.ReadInt32();
                    Debug.Log("Credits:" + Credits);

                    // 最大ベット枚数
                    MaxBetAmounts = bStream.ReadInt32();
                    Debug.Log("MaxBetAmounts:" + MaxBetAmounts);

                    // 最後に掛けた枚数
                    LastBetAmounts = bStream.ReadInt32();
                    Debug.Log("LastBetAmounts:" + LastBetAmounts);

                    // リプレイ状態
                    HasReplay = (bStream.ReadInt32() == 1 ? true : false);
                    Debug.Log("HasReplay:" + HasReplay);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    Debug.Log("MedalSystem Loaded");
                }

                return true;
            }
        }

        // コンストラクタ
        public MedalBehaviour()
        {
            CurrentBet = 0;
            RemainingPayouts = 0;
            LastPayoutAmounts = 0;

            MedalSave = new MedalSystemSave();
            FinishedBet = false;
        }

        // func
        // セーブデータのメダル情報を読み込む
        public void LoadMedalSystemSave(MedalSystemSave save) => MedalSave = save;

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
                MedalSave.Credits = Math.Clamp(MedalSave.Credits += CurrentBet, MinCredit, MaxCredit);
                CurrentBet = 0;
            }
            MedalSave.LastBetAmounts = amounts;
            ////Debug.Log("Bet Received:" + RemainingBet);
        }

        // 投入処理
        public void InsertOneMedal()
        {
            RemainingBet -= 1;
           //////Debug.Log("Remaining:" + RemainingBet);
            //////Debug.Log("Bet Medal by 1");
            CurrentBet += 1;
           //HasMedalInserted.Invoke(1);

            // リプレイでなければクレジットを減らす
            if (!MedalSave.HasReplay)
            {
                MedalSave.Credits = Math.Clamp(MedalSave.Credits -= 1, MinCredit, MaxCredit);
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
            if (MedalSave.Credits < 0)
            {
                MedalSave.Credits = 0;
            }
            MedalSave.Credits = Math.Clamp(MedalSave.Credits += value, MinCredit, MaxCredit);
        }
    }
}