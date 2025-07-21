using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using System.Collections.Generic;
using static ReelSpinGame_Bonus.BonusBehaviour;
using UnityEngine;
using System.IO;
using System;

namespace ReelSpinGame_System
{
    public class PlayingDatabase : ISavable
    {
        // プレイ中情報

        // const

        // 記録可能ゲーム数
        public const int MaximumTotalGames = 99999;

        // var
        // ゲーム数情報
        // 総ゲーム数(ボーナス中除く)
        public int TotalGames { get; private set; }
        // ボーナス間ゲーム数
        public int CurrentGames { get; private set; }

        // メダル情報
        public PlayerMedalData PlayerMedalData { get; private set; }

        // 当選させたボーナス(IDごとに)
        public List<BonusHitData> BonusHitDatas { get; private set; }

        // ビッグチャンス成立回数
        public int BigTimes { get; private set; }
        // ボーナスゲーム成立回数
        public int RegTimes { get; private set; }

        // コンストラクタ
        public PlayingDatabase()
        {
            TotalGames = 0;
            CurrentGames = 0;
            PlayerMedalData = new PlayerMedalData();
            BonusHitDatas = new List<BonusHitData>();
            BigTimes = 0;
            RegTimes = 0;
        }

        // func

        // 各種データ数値変更

        // ゲーム数を増やす
        public void IncreaseGameValue()
        {
            TotalGames += 1;
            CurrentGames += 1;
        }
        // ボーナス間ゲーム数リセット
        public void ResetCurrentGame() => CurrentGames = 0;

        // ボーナス履歴追加(成立時に使用)
        public void AddBonusResult(BonusType bonusType)
        {
            BonusHitData buffer = new BonusHitData();
            BonusHitDatas.Add(buffer);
            BonusHitDatas[^1].SetBonusType(bonusType);
            BonusHitDatas[^1].SetBonusHitGame(CurrentGames);
        }

        // 直近のボーナス履歴の入賞を記録
        public void SetLastBonusStart() => BonusHitDatas[^1].SetBonusStartGame(CurrentGames);
        // 直近のビッグチャンス時の色を記録
        public void SetLastBigChanceColor(BigColor color) => BonusHitDatas[^1].SetBigChanceColor(color);
        // 現在のボーナス履歴に払い出しを追加する
        public void ChangeLastBonusPayouts(int payouts) => BonusHitDatas[^1].ChangeBonusPayouts(payouts);

        // ビッグチャンス回数の増加
        public void IncreaseBigChance() => BigTimes += 1;
        // ボーナスゲーム回数の増加
        public void IncreaseBonusGame() => RegTimes += 1;

        // セーブ書き込み
        public List<int> SaveData()
        {
            // 変数データをすべて格納
            List<int> data = new List<int>();

            data.Add(TotalGames);
            data.Add(CurrentGames);
            
            // メダル情報
            foreach(int list in PlayerMedalData.SaveData())
            {
                data.Add(list);
            }

            // ボーナス情報の数
            data.Add(BonusHitDatas.Count);

            // ボーナス情報
            for(int i = 0; i < BonusHitDatas.Count; i++)
            {
                foreach (int list in BonusHitDatas[i].SaveData())
                {
                    data.Add(list);
                }
            }

            // BIG/REG回数
            data.Add(BigTimes);
            data.Add(RegTimes);

            // デバッグ用
            Debug.Log("PlayerData:");
            foreach(int i in data)
            {
                Debug.Log(i);
            }

            return data;
        }

        // セーブ読み込み
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // ゲーム数読み込み
                TotalGames = bStream.ReadInt32();
                Debug.Log("TotalGames:" + TotalGames);

                CurrentGames = bStream.ReadInt32();
                Debug.Log("CurrentGames:" + CurrentGames);

                // メダル情報読み込み
                PlayerMedalData.LoadData(bStream);

                // ボーナス履歴読み込み
                // ボーナス履歴数

                int bonusResultCounts = bStream.ReadInt32();
                Debug.Log("BonusResultCounts:" + bonusResultCounts);

                // 履歴分読み込む
                for (int i = 0; i < bonusResultCounts; i++)
                {
                    BonusHitData buffer = new BonusHitData();
                    BonusHitDatas.Add(buffer);
                    buffer.LoadData(bStream);
                }

                Debug.Log("BonusLoad END");

                // BIG回数
                BigTimes = bStream.ReadInt32();
                Debug.Log("BigTimes:" + BigTimes);

                // REG回数
                RegTimes = bStream.ReadInt32();
                Debug.Log("RegTimes:" + RegTimes);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                Debug.Log("PlayerData Read is done");
            }

            return true;
        }
    }
}
