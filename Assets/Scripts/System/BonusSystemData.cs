using System;
using System.Collections.Generic;
using System.IO;

namespace ReelSpinGame_Bonus
{
    // ボーナス情報
    public class BonusSystemData
    {
        public enum BonusTypeID { BonusNone, BonusBIG, BonusREG }               // ボーナスの種類
        public enum BonusStatus { BonusNone, BonusBIGGames, BonusJACGames };    // ボーナスの状態
        public enum BigType { None, Red, Blue, Black };                         // BIGの種類

        public const int BigGames = 30;         // 残り小役ゲーム数
        public const int JacInTimes = 3;        // 残りJACIN
        public const int JacGames = 12;         // 残りJACゲーム数
        public const int JacHits = 8;           // 残り当選回数

        public const int MaxRecordPayout = 99999;       // 最高で記録できる獲得枚数
        public const int MaxZoneGames = 50;             // 連チャン区間であるゲーム数

        public BonusTypeID HoldingBonusID { get; set; }         // 現在ストックしているボーナス
        public BonusStatus CurrentBonusStatus { get; set; }     // ボーナス状態
        public BigType BigChanceType { get; set; }              // BIGボーナス当選時の種類

        public int RemainingBigGames { get; set; }          // 残り小役ゲーム数
        public int RemainingJacIn { get; set; }             // 残りJACIN
        public int RemainingJacGames { get; set; }          // 残りJACゲーム数
        public int RemainingJacHits { get; set; }           // 残り当選回数

        public int CurrentBonusPayout { get; set; }         // このボーナスでの獲得枚数\
        public int CurrentZonePayout { get; set; }          // 連チャン区間中のボーナス枚数
        public bool HasZone { get; set; }                   // 連チャン区間にいるか
        public int LastZonePayout { get; set; }             // 最後に獲得した連チャン区間の枚数

        public bool HasBonusStarted { get; set; }           // ボーナスが開始したか
        public bool HasBonusFinished { get; set; }          // ボーナスが終了したか

        public BonusSystemData()
        {
            HoldingBonusID = BonusTypeID.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigChanceType = BigType.None;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
            CurrentBonusPayout = 0;
            CurrentZonePayout = 0;
            LastZonePayout = 0;
            HasZone = false;
            HasBonusStarted = false;
            HasBonusFinished = false;
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();

            data.Add((int)HoldingBonusID);
            data.Add((int)CurrentBonusStatus);
            data.Add((int)BigChanceType);
            data.Add(RemainingBigGames);
            data.Add(RemainingJacIn);
            data.Add(RemainingJacHits);
            data.Add(RemainingJacGames);
            data.Add(CurrentBonusPayout);
            data.Add(CurrentZonePayout);
            data.Add(HasZone ? 1 : 0);
            data.Add(LastZonePayout);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                HoldingBonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                CurrentBonusStatus = (BonusStatus)Enum.ToObject(typeof(BonusStatus), br.ReadInt32());
                BigChanceType = (BigType)Enum.ToObject(typeof(BigType), br.ReadInt32());
                RemainingBigGames = br.ReadInt32();
                RemainingJacIn = br.ReadInt32();
                RemainingJacHits = br.ReadInt32();
                RemainingJacGames = br.ReadInt32();
                CurrentBonusPayout = br.ReadInt32();
                CurrentZonePayout = br.ReadInt32();
                HasZone = (br.ReadInt32() == 1 ? true : false);
                LastZonePayout = br.ReadInt32();

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return true;
        }
    }
}