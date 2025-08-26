using ReelSpinGame_Interface;
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

        // コンストラクタ
        public BonusHitData()
        {
            BonusID = BonusTypeID.BonusNone;
            BonusHitGame = 0;
            BonusStartGame = 0;
            BonusPayout = 0;
            BigColor = BigColor.None;
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

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add((int)BonusID);
            data.Add(BonusHitGame);
            data.Add(BonusStartGame);
            data.Add(BonusPayout);
            data.Add((int)BigColor);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            // ボーナス当選履歴(1つずつ読み込み。5バイトで読み込み)

            try
            {
                // ボーナス履歴
                BonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                //Debug.Log("BonusID:" + BonusID);
                // ボーナス成立時G
                BonusHitGame = br.ReadInt32();
                //Debug.Log("BonusHit:" + BonusHitGame);
                // ボーナス当選G
                BonusStartGame = br.ReadInt32();
                //Debug.Log("BonusStartGame:" + BonusStartGame);
                // ボーナス払い出し枚数
                BonusPayout = br.ReadInt32();
                //Debug.Log("BonusPayout:" + BonusPayout);
                // BIG時の色
                BigColor = (BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32());
                //Debug.Log("BigColor:" + BigColor);
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