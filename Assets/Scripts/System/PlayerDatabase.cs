using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Analytics;
using ReelSpinGame_Interface;
using ReelSpinGame_Save.Player;
using System.Collections.Generic;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_System
{
    // プレイヤー情報
    public class PlayerDatabase : IHasSave<PlayerSave>
    {
        public const int MaximumTotalGames = 99999;        // 記録可能ゲーム数

        public int TotalGames { get; private set; }         // 総ゲーム数(ボーナス中除く)
        public int CurrentGames { get; private set; }       // ボーナス間ゲーム数
        public int BigTimes { get; private set; }           // ビッグチャンス成立回数
        public int RegTimes { get; private set; }           // ボーナスゲーム成立回数

        public PlayerMedalData PlayerMedalData { get; private set; }        // メダル情報
        public List<BonusHitData> BonusHitRecord { get; private set; }      // 当選させたボーナス(IDごとに)
        public AnalyticsData PlayerAnalyticsData { get; private set; }      // 解析データ

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

        // セーブデータにする
        public PlayerSave MakeSaveData()
        {
            PlayerSave save = new PlayerSave();
            save.RecordData(this);

            return save;
        }

        // セーブを読み込む
        public void LoadSaveData(PlayerSave loadData)
        {
            TotalGames = loadData.TotalGames;
            CurrentGames = loadData.CurrentGames;
            BigTimes = loadData.BigTimes;
            RegTimes = loadData.RegTimes;
            PlayerMedalData.SetData(loadData.PlayerMedalData);
            BonusHitRecord = loadData.BonusHitRecord;
            PlayerAnalyticsData = loadData.PlayerAnalyticsData;
        }

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

        // 直近のビッグチャンス時の種類を記録
        public void SetLastBigChanceColor(BigType color) => BonusHitRecord[^1].SetBigType(color);
        // 現在のボーナス履歴に払い出しを追加する
        public void ChangeLastBonusPayout(int payout) => BonusHitRecord[^1].ChangeBonusPayout(payout);
        // 現在のボーナス履歴の成立時出目を記録
        public void SetBonusHitPos(List<int> lastPos) => BonusHitRecord[^1].SetBonusReelPos(lastPos);
        // 現在のボーナス成立時の押し順を記録
        public void SetBonusPushOrder(List<int> pushOrder) => BonusHitRecord[^1].SetBonusReelPushOrder(pushOrder);
        // 現在のボーナス履歴の成立時スベリを記録
        public void SetBonusHitDelay(List<int> lastDelay) => BonusHitRecord[^1].SetBonusReelDelay(lastDelay);
    }
}
