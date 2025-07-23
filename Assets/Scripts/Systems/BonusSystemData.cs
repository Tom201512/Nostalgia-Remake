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
        public enum BonusType { BonusNone, BonusBIG, BonusREG }

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
        public const int MaxRecordPayouts = 99999;
        // 連チャン区間であるゲーム数
        public const int MaxZoneGames = 50;

        // var
        // 現在ストックしているボーナス
        public BonusType HoldingBonusID { get; set; }
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
        public int CurrentBonusPayouts { get; set; }
        // 連チャン区間中のボーナス枚数
        public int CurrentZonePayouts { get; set; }
        // 連チャン区間にいるか
        public bool HasZone { get; set; }

        public BonusSystemData()
        {
            HoldingBonusID = BonusType.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigChanceColor = BigColor.None;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
            CurrentBonusPayouts = 0;
            CurrentZonePayouts = 0;
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
            data.Add(CurrentBonusPayouts);
            data.Add(CurrentZonePayouts);
            data.Add(HasZone ? 1 : 0);

            // デバッグ用
            //Debug.Log("BonusData:");
            //foreach (int i in data)
            //{
            //    Debug.Log(i);
            //}

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // ストック中のボーナス
                HoldingBonusID = (BonusType)Enum.ToObject(typeof(BonusType), bStream.ReadInt32());
                //Debug.Log("HoldingBonusID:" + HoldingBonusID);
                // 現在のボーナス状態
                CurrentBonusStatus = (BonusStatus)Enum.ToObject(typeof(BonusStatus), bStream.ReadInt32());
                //Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
                // ビッグチャンス時の色
                BigChanceColor = (BigColor)Enum.ToObject(typeof(BigColor), bStream.ReadInt32());
                //Debug.Log("BigChanceColor:" + BigChanceColor);
                // 残りBIGゲーム数
                RemainingBigGames = bStream.ReadInt32();
                //Debug.Log("RemainingBigGames:" + RemainingBigGames);
                // 残りJACIN
                RemainingJacIn = bStream.ReadInt32();
                //Debug.Log("RemainingJacIn:" + RemainingJacIn);
                // 残りJACゲーム中当選回数
                RemainingJacHits = bStream.ReadInt32();
                //Debug.Log("RemainingJacHits:" + RemainingJacHits);
                // 残りJACゲーム数
                RemainingJacGames = bStream.ReadInt32();
                //Debug.Log("RemainingJacGames:" + RemainingJacGames);
                // 現在のボーナス獲得枚数
                CurrentBonusPayouts = bStream.ReadInt32();
                //Debug.Log("CurrentBonusPayouts:" + CurrentBonusPayouts);
                // 連チャン区間中のボーナス枚数
                CurrentZonePayouts = bStream.ReadInt32();
                //Debug.Log("CurrentZonePayouts:" + CurrentZonePayouts);
                // 連チャン区間中か
                HasZone = (bStream.ReadInt32() == 1 ? true : false);
                //Debug.Log("HasZone:" + HasZone);

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