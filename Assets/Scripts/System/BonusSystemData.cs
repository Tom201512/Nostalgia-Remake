using System;
using System.Collections.Generic;
using System.IO;

namespace ReelSpinGame_Bonus
{
	public class BonusSystemData
	{
        // ボーナス情報
        // const

        // ボーナスの種類
        public enum BonusTypeID { BonusNone, BonusBIG, BonusREG }

        // ボーナスの状態
        public enum BonusStatus { BonusNone, BonusBIGGames, BonusJACGames };

        // BIGボーナスで当選した色
        public enum BigColor {None, Red, Blue, Black};

        // 残り小役ゲーム数
        public const int BigGames = 30;
        // 残りJACIN
        public const int JacInTimes = 3;
        // JACゲーム中
        // 残りJACゲーム数
        public const int JacGames = 12;
        // 残り当選回数
        public const int JacHits = 8;

        // 最高で記録できる獲得枚数
        public const int MaxRecordPayout = 99999;
        // 連チャン区間であるゲーム数
        public const int MaxZoneGames = 50;

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

        public BonusSystemData()
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
            LastZonePayout = 0;
            HasZone = false;
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();

            data.Add((int)HoldingBonusID);
            data.Add((int)CurrentBonusStatus);
            data.Add((int)BigChanceColor);
            data.Add(RemainingBigGames);
            data.Add(RemainingJacIn);
            data.Add(RemainingJacHits);
            data.Add(RemainingJacGames);
            data.Add(CurrentBonusPayout);
            data.Add(CurrentZonePayout);
            data.Add(HasZone ? 1 : 0);
            data.Add(LastZonePayout);

            // デバッグ用
            //Debug.Log("BonusData:");
            //foreach (int i in data)
            //{
            //    Debug.Log(i);
            //}

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                // ストック中のボーナス
                HoldingBonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                //Debug.Log("HoldingBonusID:" + HoldingBonusID);
                // 現在のボーナス状態
                CurrentBonusStatus = (BonusStatus)Enum.ToObject(typeof(BonusStatus), br.ReadInt32());
                //Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
                // ビッグチャンス時の色
                BigChanceColor = (BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32());
                //Debug.Log("BigChanceColor:" + BigChanceColor);
                // 残りBIGゲーム数
                RemainingBigGames = br.ReadInt32();
                //Debug.Log("RemainingBigGames:" + RemainingBigGames);
                // 残りJACIN
                RemainingJacIn = br.ReadInt32();
                //Debug.Log("RemainingJacIn:" + RemainingJacIn);
                // 残りJACゲーム中当選回数
                RemainingJacHits = br.ReadInt32();
                //Debug.Log("RemainingJacHits:" + RemainingJacHits);
                // 残りJACゲーム数
                RemainingJacGames = br.ReadInt32();
                //Debug.Log("RemainingJacGames:" + RemainingJacGames);
                // 現在のボーナス獲得枚数
                CurrentBonusPayout = br.ReadInt32();
                //Debug.Log("CurrentBonusPayout:" + CurrentBonusPayout);
                // 連チャン区間中のボーナス枚数
                CurrentZonePayout = br.ReadInt32();
                //Debug.Log("CurrentZonePayout:" + CurrentZonePayout);
                // 連チャン区間中か
                HasZone = (br.ReadInt32() == 1 ? true : false);
                //Debug.Log("HasZone:" + HasZone);
                // 最終連チャン区間
                LastZonePayout = br.ReadInt32();

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("BonusData end");
            }

            return true;
        }
    }
}