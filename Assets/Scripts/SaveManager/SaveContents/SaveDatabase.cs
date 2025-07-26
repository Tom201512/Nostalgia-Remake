using ReelSpinGame_Interface;
using ReelSpinGame_Save.Bonus;
using ReelSpinGame_Save.Medal;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using System;
using System.Collections.Generic;

namespace ReelSpinGame_Save.Database
{
    // セーブ用のデータベース
    public class SaveDatabase
    {
        // システム

        // 台設定
        public int Setting { get; private set; }

        // プレイヤー情報
        public PlayerSave Player { get; private set; }

        // メダル情報
        public MedalSave Medal { get; private set; }

        // フラグカウンタ数値
        public int FlagCounter { get; private set; }

        // 最後に止まったリール位置
        public List<int> LastReelPos { get; private set; }

        // ボーナス情報
        public BonusSave Bonus { get; private set; }

        public SaveDatabase()
        {
            Setting = 6;
            Player = new PlayerSave();
            Medal = new MedalSave();
            FlagCounter = 0;
            LastReelPos = new List<int> { 19, 19, 19 };
            Bonus = new BonusSave();
        }

        // func

        // 各種情報記録
        // 台設定
        public void RecordSlotSetting(int setting) => Setting = setting;

        // プレイヤー情報
        public void RecordPlayerSave(ISavable player)
        {
            if (player.GetType() == typeof(PlayerSave))
            {
                Player = player as PlayerSave;
            }
            else
            {
                throw new Exception("Save data is not PlayerSave");
            }
        }

        // メダル情報
        public void RecordMedalSave(ISavable medal)
        {
            if (medal.GetType() == typeof(MedalSave))
            {
                Medal = medal as MedalSave;
            }
            else
            {
                throw new Exception("Save data is not MedalData");
            }
        }

        // フラグカウンタ
        public void RecordFlagCounter(int flagCounter) => FlagCounter = flagCounter;

        // リール位置
        public void RecordReelPos(List<int> lastStopped) => LastReelPos = lastStopped;

        // ボーナス情報
        public void RecordBonusData(ISavable bonus)
        {
            {
                if (bonus.GetType() == typeof(BonusSave))
                {
                    Bonus = bonus as BonusSave;
                }
                else
                {
                    throw new Exception("Save data is not BonusSave");
                }
            }
        }
    }
}