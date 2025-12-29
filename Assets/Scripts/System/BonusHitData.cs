using ReelSpinGame_Interface;
using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Datas
{
    // 当選ボーナス情報
    public class BonusHitData : ISavable
    {
        public BonusTypeID BonusID { get; private set; }            // 当選ボーナスID
        public int BonusHitGame { get; private set; }               // 成立時ゲーム数(成立時点でのゲーム数)
        public int BonusStartGame { get; private set; }             // 入賞時ゲーム数(ボーナスを入賞させたゲーム)
        public int BonusPayout { get; private set; }                // ボーナス獲得枚数
        public BigType BigType { get; private set; }              // BIG当選時の種類
        public List<int> BonusReelPos { get; private set; }         // ボーナス成立時のリール位置
        public List<int> BonusReelPushOrder { get; private set; }   // ボーナス成立時の押し順
        public List<int> BonusReelDelay { get; private set; }       // ボーナス成立時のスベリコマ数

        public BonusHitData()
        {
            BonusID = BonusTypeID.BonusNone;
            BonusHitGame = 0;
            BonusStartGame = 0;
            BonusPayout = 0;
            BigType = BigType.None;
            BonusReelPos = new List<int> { 0, 0, 0, };
            BonusReelPushOrder = new List<int> { 0, 0, 0, };
            BonusReelDelay = new List<int> { 0, 0, 0, };
        }

        public void SetBonusType(BonusTypeID bonusID) => BonusID = bonusID;         // 成立ボーナスセット
        public void SetBonusHitGame(int game) => BonusHitGame = game;               // 成立時ゲーム数セット
        public void SetBonusStartGame(int game) => BonusStartGame = game;           // 入賞時ゲーム数セット
        public void ChangeBonusPayout(int amount) => BonusPayout = amount;          // ボーナス獲得枚数変更
        public void SetBigType(BigType type) => BigType = type;            // ビッグチャンスの種類のセット

        // 成立時のリール停止位置セット
        public void SetBonusReelPos(List<int> reelPos)
        {
            BonusReelPos[(int)ReelID.ReelLeft] = reelPos[(int)ReelID.ReelLeft];
            BonusReelPos[(int)ReelID.ReelMiddle] = reelPos[(int)ReelID.ReelMiddle];
            BonusReelPos[(int)ReelID.ReelRight] = reelPos[(int)ReelID.ReelRight];
        }

        // 成立時の押し順セット
        public void SetBonusReelPushOrder(List<int> reelPushOrder)
        {
            BonusReelPushOrder[(int)ReelID.ReelLeft] = reelPushOrder[(int)ReelID.ReelLeft];
            BonusReelPushOrder[(int)ReelID.ReelMiddle] = reelPushOrder[(int)ReelID.ReelMiddle];
            BonusReelPushOrder[(int)ReelID.ReelRight] = reelPushOrder[(int)ReelID.ReelRight];
        }

        // 成立時のスベリコマ数セット
        public void SetBonusReelDelay(List<int> reelDelay)
        {
            BonusReelDelay[(int)ReelID.ReelLeft] = reelDelay[(int)ReelID.ReelLeft];
            BonusReelDelay[(int)ReelID.ReelMiddle] = reelDelay[(int)ReelID.ReelMiddle];
            BonusReelDelay[(int)ReelID.ReelRight] = reelDelay[(int)ReelID.ReelRight];
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add((int)BonusID);
            data.Add(BonusHitGame);
            data.Add(BonusStartGame);
            data.Add(BonusPayout);
            data.Add((int)BigType);

            foreach (int pos in BonusReelPos)
            {
                data.Add(pos);
            }

            foreach (int pos in BonusReelPushOrder)
            {
                data.Add(pos);
            }

            foreach (int delay in BonusReelDelay)
            {
                data.Add(delay);
            }

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                BonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                BonusHitGame = br.ReadInt32();
                BonusStartGame = br.ReadInt32();
                BonusPayout = br.ReadInt32();
                BigType = (BigType)Enum.ToObject(typeof(BigType), br.ReadInt32());

                for (int i = 0; i < BonusReelPos.Count; i++)
                {
                    BonusReelPos[i] = br.ReadInt32();
                }

                for (int i = 0; i < BonusReelPushOrder.Count; i++)
                {
                    BonusReelPushOrder[i] = br.ReadInt32();
                }

                for (int i = 0; i < BonusReelDelay.Count; i++)
                {
                    BonusReelDelay[i] = br.ReadInt32();
                }
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