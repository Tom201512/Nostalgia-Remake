using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
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

        // メダル情報
        public PlayerMedalData PlayerMedalData { get; private set; }

        // 当選させたボーナス(IDごとに)
        //public List<BonusHitData> BonusHitRecord { get; private set; }

        // ビッグチャンス成立回数
        public int BigTimes { get; private set; }
        // ボーナスゲーム成立回数
        public int RegTimes { get; private set; }

        // コンストラクタ
        public PlayerDatabase()
        {
            TotalGames = 0;
            CurrentGames = 0;
            PlayerMedalData = new PlayerMedalData();
            //BonusHitRecord = new List<BonusHitData>();
            BigTimes = 0;
            RegTimes = 0;
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
        public void LoadSaveData(ISavable loadData)
        {
            if (loadData.GetType() == typeof(PlayerSave))
            {
                PlayerSave save = new PlayerSave();
                save = loadData as PlayerSave;

                TotalGames = save.TotalGames;
                CurrentGames = save.CurrentGames;
                PlayerMedalData.SetData(save.PlayerMedalData);
                //SetBonusRecord(save);
                BigTimes = save.BigTimes;
                RegTimes = save.RegTimes;
            }
            else
            {
                throw new Exception("Loaded data is not PlayerData");
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
        // ビッグチャンス回数の増加
        public void IncreaseBigChance() => BigTimes += 1;
        // ボーナスゲーム回数の増加
        public void IncreaseBonusGame() => RegTimes += 1;

        /*
        // ボーナス履歴追加(成立時に使用)
        public void AddBonusResult(BonusTypeID bonusType)
        {
            BonusHitData buffer = new BonusHitData();
            BonusHitRecord.Add(buffer);
            BonusHitRecord[^1].SetBonusType(bonusType);
            BonusHitRecord[^1].SetBonusHitGame(CurrentGames);
        }

        // 直近のボーナス履歴の入賞を記録
        public void SetLastBonusStart() => BonusHitRecord[^1].SetBonusStartGame(CurrentGames);
        // 直近のビッグチャンス時の色を記録
        public void SetLastBigChanceColor(BigColor color) => BonusHitRecord[^1].SetBigChanceColor(color);
        // 現在のボーナス履歴に払い出しを追加する
        public void ChangeLastBonusPayout(int payout) => BonusHitRecord[^1].ChangeBonusPayout(payout);*/

        // ボーナス履歴情報を読み込む

        /*
        private void SetBonusRecord(PlayerSave save)
        {
            foreach(BonusHitData b in save.BonusHitRecord)
            {
                BonusHitRecord.Add(b);
            }
        }*/
    }
}
