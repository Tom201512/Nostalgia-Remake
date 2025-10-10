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
        // var
        // 現在ストックしているボーナス
        public BonusTypeID HoldingBonusID { get; set; }
        // ボーナス状態
        public BonusStatus CurrentBonusStatus { get; set; }
        // BIGボーナス当選時の色
        public BigColor BigChanceColor { get; set; }

        // 小役ゲーム中
        // 残り小役ゲーム数
        public int RemainingBigGames { get; set; }
        // 残りJACIN
        public int RemainingJacIn { get; set; }

        // JACゲーム中
        // 残りJACゲーム数
        public int RemainingJacGames { get; set; }
        // 残り当選回数
        public int RemainingJacHits { get; set; }

        // このボーナスでの獲得枚数
        public int CurrentBonusPayout { get; set; }
        // 連チャン区間中のボーナス枚数
        public int CurrentZonePayout { get; set; }
        // 連チャン区間にいるか
        public bool HasZone { get; set; }
        // 最後に獲得した連チャン区間の枚数
        public int LastZonePayout { get; set; }

        public BonusSave()
        {
            HoldingBonusID = BonusTypeID.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigChanceColor = BigColor.None;
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
            BigChanceColor = bonus.BigChanceColor;
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
            Debug.Log("HoldingBonusID:" + HoldingBonusID);
            data.Add((int)CurrentBonusStatus);
            Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
            data.Add((int)BigChanceColor);
            Debug.Log("BigChanceColor:" + BigChanceColor);
            data.Add(RemainingBigGames);
            Debug.Log("RemainingBigGames:" + RemainingBigGames);
            data.Add(RemainingJacIn);
            Debug.Log("RemainingJacIn:" + RemainingJacIn);
            data.Add(RemainingJacHits);
            Debug.Log("RemainingJacHits:" + RemainingJacHits);
            data.Add(RemainingJacGames);
            Debug.Log("RemainingJacGames:" + RemainingJacGames);
            data.Add(CurrentBonusPayout);
            Debug.Log("CurrentBonusPayout:" + CurrentBonusPayout);
            data.Add(CurrentZonePayout);
            Debug.Log("CurrentZonePayout:" + CurrentZonePayout);
            data.Add(LastZonePayout);
            Debug.Log("LastZonePayout:" + LastZonePayout);
            data.Add(HasZone ? 1 : 0);
            Debug.Log("HasZone:" + HasZone);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                HoldingBonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                Debug.Log("HoldingBonusID:" + HoldingBonusID);
                CurrentBonusStatus = (BonusStatus)Enum.ToObject(typeof(BonusStatus), br.ReadInt32());
                Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
                BigChanceColor = (BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32());
                Debug.Log("BigChanceColor:" + BigChanceColor);
                RemainingBigGames = br.ReadInt32();
                Debug.Log("RemainingBigGames:" + RemainingBigGames);
                RemainingJacIn = br.ReadInt32();
                Debug.Log("RemainingJacIn:" + RemainingJacIn);
                RemainingJacHits = br.ReadInt32();
                Debug.Log("RemainingJacHits:" + RemainingJacHits);
                RemainingJacGames = br.ReadInt32();
                Debug.Log("RemainingJacGames:" + RemainingJacGames);
                CurrentBonusPayout = br.ReadInt32();
                Debug.Log("CurrentBonusPayout:" + CurrentBonusPayout);
                CurrentZonePayout = br.ReadInt32();
                Debug.Log("CurrentZonePayout:" + CurrentZonePayout);
                LastZonePayout = br.ReadInt32();
                Debug.Log("LastZonePayout:" + LastZonePayout);
                HasZone = (br.ReadInt32() == 1 ? true : false);
                Debug.Log("HasZone:" + HasZone);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {

            }

            Debug.Log("BonusData end");
            return true;
        }
    }
}

