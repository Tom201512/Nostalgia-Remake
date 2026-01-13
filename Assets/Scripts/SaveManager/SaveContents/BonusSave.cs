using ReelSpinGame_Bonus;
using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Save.Bonus
{
    // ボーナス情報のセーブ
    public class BonusSave : ISavable
    {
        public BonusTypeID HoldingBonusID { get; set; }         // 現在ストックしているボーナス
        public BonusStatus CurrentBonusStatus { get; set; }     // ボーナス状態
        public BigType BigChanceType { get; set; }            // BIGボーナス当選時の種類

        public int RemainingBigGames { get; set; }          // 残り小役ゲーム数
        public int RemainingJacIn { get; set; }             // 残りJACIN
        public int RemainingJacGames { get; set; }          // 残りJACゲーム数
        public int RemainingJacHits { get; set; }           // 残り当選回数

        public int CurrentBonusPayout { get; set; }         // このボーナスでの獲得枚数
        public int CurrentZonePayout { get; set; }          // 連チャン区間中のボーナス枚数
        public bool HasZone { get; set; }                   // 連チャン区間にいるか
        public int LastZonePayout { get; set; }             // 最後に獲得した連チャン区間の枚数

        public BonusSave()
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
            HasZone = false;
            LastZonePayout = 0;
        }

        // データ記録
        public void RecordData(BonusSystemData bonus)
        {
            HoldingBonusID = bonus.HoldingBonusID;
            CurrentBonusStatus = bonus.CurrentBonusStatus;
            BigChanceType = bonus.BigChanceType;
            RemainingBigGames = bonus.RemainingBigGames;
            RemainingJacIn = bonus.RemainingJacIn;
            RemainingJacHits = bonus.RemainingJacHits;
            RemainingJacGames = bonus.RemainingJacGames;
            CurrentBonusPayout = bonus.CurrentBonusPayout;
            CurrentZonePayout = bonus.CurrentZonePayout;
            LastZonePayout = bonus.LastZonePayout;
            HasZone = bonus.HasZone;
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
            data.Add(LastZonePayout);
            data.Add(HasZone ? 1 : 0);

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
                LastZonePayout = br.ReadInt32();
                HasZone = (br.ReadInt32() == 1 ? true : false);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }
    }
}

