using ReelSpinGame_Save.Bonus;
using ReelSpinGame_Save.Medal;
using ReelSpinGame_Save.Player;
using System.Collections.Generic;

namespace ReelSpinGame_Save.Database
{
    // セーブ用のデータベース
    public class SaveDatabase
    {
        public int Setting { get; set; }                        // 台設定
        public bool IsUsingRandom { get; set; }                  // ランダム設定を使用しているか
        public PlayerSave Player { get; private set; }          // プレイヤー情報
        public MedalSave Medal { get; private set; }            // メダル情報
        public int FlagCounter { get; private set; }            // フラグカウンタ数値
        public List<int> LastReelPos { get; private set; }      // 最後に止まったリール位置
        public List<int> LastReelDelay { get; private set; }    // 最後に止めたリールのスベリコマ
        public BonusSave Bonus { get; private set; }            // ボーナス情報

        public SaveDatabase()
        {
            Setting = -1;
            IsUsingRandom = false;
            Player = new PlayerSave();
            Medal = new MedalSave();
            FlagCounter = 0;
            LastReelPos = new List<int> { 19, 19, 19 };
            LastReelDelay = new List<int> { 0, 0, 0, };
            Bonus = new BonusSave();
        }

        // データを初期化
        public void InitializeSave()
        {
            Player = null;
            Medal = null;
            LastReelPos = null;
            LastReelDelay = null;
            Bonus = null;

            Setting = -1;
            IsUsingRandom = false;
            Player = new PlayerSave();
            Medal = new MedalSave();
            FlagCounter = 0;
            LastReelPos = new List<int> { 19, 19, 19 };
            LastReelDelay = new List<int> { 0, 0, 0 };
            Bonus = new BonusSave();
        }

        // プレイヤー情報
        public void RecordPlayerSave(PlayerSave player) => Player = player;

        // メダル情報
        public void RecordMedalSave(MedalSave medal) => Medal = medal;

        // フラグカウンタ
        public void RecordFlagCounter(int flagCounter) => FlagCounter = flagCounter;

        // リール位置
        public void RecordReelPos(List<int> lastStopped) => LastReelPos = lastStopped;

        // スベリコマ数
        public void RecordLastReelDelay(List<int> lastReelDelay) => LastReelDelay = lastReelDelay;

        // ボーナス情報
        public void RecordBonusData(BonusSave bonus) => Bonus = bonus;
    }
}