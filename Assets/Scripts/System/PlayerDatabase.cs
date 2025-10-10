using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using ReelSpinGame_Datas.Analytics;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_System
{
    public class PlayerDatabase : IHasSave
    {
        // プレイヤー情報

        // const
        // 記録可能ゲーム数
        public const int MaximumTotalGames = 99999;

        // var
        // ゲーム数情報
        // 総ゲーム数(ボーナス中除く)
        public int TotalGames { get; private set; }
        // ボーナス間ゲーム数
        public int CurrentGames { get; private set; }

        // ビッグチャンス成立回数
        public int BigTimes { get; private set; }
        // ボーナスゲーム成立回数
        public int RegTimes { get; private set; }

        // メダル情報
        public PlayerMedalData PlayerMedalData { get; private set; }

        // 当選させたボーナス(IDごとに)
        public List<BonusHitData> BonusHitRecord { get; private set; }

        // 解析データ
        public AnalyticsData PlayerAnalyticsData { get; private set; }

        // コンストラクタ
        public PlayerDatabase()
        {
            TotalGames = 0;
            CurrentGames = 0;
            BigTimes = 0;
            RegTimes = 0;
            PlayerMedalData = new PlayerMedalData();
            BonusHitRecord = new List<BonusHitData>();
            PlayerAnalyticsData = new AnalyticsData();
        }

        // func
        // セーブデータにする
        public ISavable MakeSaveData()
        {
            PlayerSave save = new PlayerSave();
            save.RecordData(this);

            return save;
        }

        // セーブを読み込む
        public bool LoadSaveData(ISavable loadData)
        {
            if (loadData.GetType() == typeof(PlayerSave))
            {
                PlayerSave save = new PlayerSave();
                save = loadData as PlayerSave;

                TotalGames = save.TotalGames;
                CurrentGames = save.CurrentGames;
                BigTimes = save.BigTimes;
                RegTimes = save.RegTimes;
                PlayerMedalData.SetData(save.PlayerMedalData);
                BonusHitRecord = save.BonusHitRecord;
                PlayerAnalyticsData = save.PlayerAnalyticsData;
                return true;
            }
            else
            {
                Debug.LogError("Loaded data is not PlayerData");
                return false;
            }
        }

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
        public void AddBonusResult(BonusTypeID bonusType)
        {
            BonusHitData buffer = new BonusHitData();
            BonusHitRecord.Add(buffer);
            BonusHitRecord[^1].SetBonusType(bonusType);
            BonusHitRecord[^1].SetBonusHitGame(CurrentGames);
        }

        // 直近のボーナス履歴の入賞を記録
        public void SetLastBonusStart()
        {
            BonusHitRecord[^1].SetBonusStartGame(CurrentGames);

            // どちらのボーナスが立ったか確認してカウントする
            if (BonusHitRecord[^1].BonusID == BonusTypeID.BonusBIG)
            {
                BigTimes += 1;
            }
            else
            {
                RegTimes += 1;
            }
        }
        // 直近のビッグチャンス時の色を記録
        public void SetLastBigChanceColor(BigColor color) => BonusHitRecord[^1].SetBigChanceColor(color);
        // 現在のボーナス履歴に払い出しを追加する
        public void ChangeLastBonusPayout(int payout) => BonusHitRecord[^1].ChangeBonusPayout(payout);
        // 現在のボーナス履歴の成立時出目を記録
        public void SetBonusHitPos(List<int> lastPos) => BonusHitRecord[^1].SetBonusReelPos(lastPos);
        // 現在のボーナス履歴の成立時スベリを記録
        public void SetBonusHitDelay(List<int> lastDelay) => BonusHitRecord[^1].SetBonusReelDelay(lastDelay);
    }
}
