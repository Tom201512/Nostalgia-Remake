using ReelSpinGame_Interface;
using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_Datas
{
    // 当選ボーナス情報
    public class BonusHitData : ISavable
    {
        // var
        // 当選ボーナスID
        public BonusTypeID BonusID { get; private set; }
        // 成立時ゲーム数(成立時点でのゲーム数)
        public int BonusHitGame { get; private set; }
        // 入賞時ゲーム数(ボーナスを入賞させたゲーム)
        public int BonusStartGame { get; private set; }
        // ボーナス獲得枚数
        public int BonusPayout { get; private set; }
        // BIG当選時の色
        public BigColor BigColor { get; private set; }
        // ボーナス成立時のリール位置
        public List<int> BonusReelPos { get; private set; }
        // ボーナス成立時の押し順
        public List<int> BonusReelPushOrder { get; private set; }
        // ボーナス成立時のスベリコマ数
        public List<int> BonusReelDelay { get; private set; }

        // コンストラクタ
        public BonusHitData()
        {
            BonusID = BonusTypeID.BonusNone;
            BonusHitGame = 0;
            BonusStartGame = 0;
            BonusPayout = 0;
            BigColor = BigColor.None;
            BonusReelPos = new List<int> { 0, 0, 0, };
            BonusReelPushOrder = new List<int> { 0, 0, 0, };
            BonusReelDelay = new List<int> { 0, 0, 0, };
        }

        // func
        // 成立ボーナスセット
        public void SetBonusType(BonusTypeID bonusID) => BonusID = bonusID;
        // 成立時ゲーム数セット
        public void SetBonusHitGame(int game) => BonusHitGame = game;
        // 入賞時ゲーム数セット
        public void SetBonusStartGame(int game) => BonusStartGame = game;
        // ボーナス獲得枚数変更
        public void ChangeBonusPayout(int amount) => BonusPayout += amount;
        // ビッグチャンス時の色セット
        public void SetBigChanceColor(BigColor color) => BigColor = color;

        // 成立時のリール停止位置セット
        public void SetBonusReelPos(List<int> reelPos)
        {
            BonusReelPos[(int)ReelID.ReelLeft] = reelPos[(int)ReelID.ReelLeft];
            BonusReelPos[(int)ReelID.ReelMiddle] = reelPos[(int)ReelID.ReelMiddle];
            BonusReelPos[(int)ReelID.ReelRight] = reelPos[(int)ReelID.ReelRight];
            //Debug.Log("ReelPos:" + BonusReelPos[(int)ReelID.ReelLeft] + "," +
                    //BonusReelPos[(int)ReelID.ReelMiddle] + "," + BonusReelPos[(int)ReelID.ReelRight]);
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
            //Debug.Log("ReelDelay:" + BonusReelDelay[(int)ReelID.ReelLeft] + "," +
                   //BonusReelDelay[(int)ReelID.ReelMiddle] + "," + BonusReelDelay[(int)ReelID.ReelRight]);
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add((int)BonusID);
            //Debug.Log("BonusID:" + BonusID);
            data.Add(BonusHitGame);
            //Debug.Log("BonusHitGame:" + BonusHitGame);
            data.Add(BonusStartGame);
            //Debug.Log("BonusStartGame:" + BonusStartGame);
            data.Add(BonusPayout);
            //Debug.Log("BonusPayout:" + BonusPayout);
            data.Add((int)BigColor);
            //Debug.Log("BigColor:" + BigColor);

            foreach(int pos in BonusReelPos)
            {
                data.Add(pos);
            }
            //Debug.Log("ReelStopPos:" + BonusReelPos[(int)ReelID.ReelLeft] + "," + BonusReelPos[(int)ReelID.ReelMiddle] + "," + BonusReelPos[(int)ReelID.ReelRight]);

            foreach (int pos in BonusReelPushOrder)
            {
                data.Add(pos);
            }
            //Debug.Log("ReelStopPos:" + BonusReelPos[(int)ReelID.ReelLeft] + "," + BonusReelPos[(int)ReelID.ReelMiddle] + "," + BonusReelPos[(int)ReelID.ReelRight]);

            foreach (int delay in BonusReelDelay)
            {
                data.Add(delay);
            }
            //Debug.Log("ReelStopDelay:" + BonusReelDelay[(int)ReelID.ReelLeft] + "," + BonusReelDelay[(int)ReelID.ReelMiddle] + "," + BonusReelDelay[(int)ReelID.ReelRight]);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                BonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                Debug.Log("BonusID:" + BonusID);
                BonusHitGame = br.ReadInt32();
                Debug.Log("BonusHitGame:" + BonusHitGame);
                BonusStartGame = br.ReadInt32();
                Debug.Log("BonusStartGame:" + BonusStartGame);
                BonusPayout = br.ReadInt32();
                Debug.Log("BonusPayout:" + BonusStartGame);
                BigColor = (BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32());
                Debug.Log("BigColor:" + BigColor);

                for (int i = 0; i < BonusReelPos.Count; i++)
                {
                    BonusReelPos[i] = br.ReadInt32();
                }

                Debug.Log("ReelStopPos:" + BonusReelPos[(int)ReelID.ReelLeft] + "," + BonusReelPos[(int)ReelID.ReelMiddle] + "," + BonusReelPos[(int)ReelID.ReelRight]);


                for (int i = 0; i < BonusReelPushOrder.Count; i++)
                {
                    BonusReelPushOrder[i] = br.ReadInt32();
                }
                Debug.Log("ReelStopOrder:" + BonusReelPushOrder[(int)ReelID.ReelLeft] + "," + BonusReelPushOrder[(int)ReelID.ReelMiddle] + "," + BonusReelPushOrder[(int)ReelID.ReelRight]);

                for (int i = 0; i < BonusReelDelay.Count; i++)
                {
                    BonusReelDelay[i] = br.ReadInt32();
                }
                Debug.Log("ReelStopDelay:" + BonusReelDelay[(int)ReelID.ReelLeft] + "," + BonusReelDelay[(int)ReelID.ReelMiddle] + "," + BonusReelDelay[(int)ReelID.ReelRight]);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            Debug.Log("BonusData end");
            return true;
        }
    }
}