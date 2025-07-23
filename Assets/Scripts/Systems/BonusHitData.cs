using ReelSpinGame_Bonus;
using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using static ReelSpinGame_Bonus.BonusBehavior;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // 当選ボーナス情報
    public class BonusHitData : ISavable
    {
        // var
        // 当選ボーナスID
        public BonusType BonusID { get; private set; }
        // 成立時ゲーム数(成立時点でのゲーム数)
        public int BonusHitGame { get; private set; }
        // 入賞時ゲーム数(ボーナスを入賞させたゲーム)
        public int BonusStartGame { get; private set; }
        // ボーナス獲得枚数
        public int BonusPayouts { get; private set; }
        // BIG当選時の色
        public BigColor BigColor { get; private set; }

        // コンストラクタ
        public BonusHitData()
        {
            BonusID = BonusType.BonusNone;
            BonusHitGame = 0;
            BonusStartGame = 0;
            BonusPayouts = 0;
            BigColor = BigColor.None;
        }

        // func
        // 成立ボーナスセット
        public void SetBonusType(BonusType bonusID) => BonusID = bonusID;
        // 成立時ゲーム数セット
        public void SetBonusHitGame(int game) => BonusHitGame = game;
        // 入賞時ゲーム数セット
        public void SetBonusStartGame(int game) => BonusStartGame = game;
        // ボーナス獲得枚数変更
        public void ChangeBonusPayouts(int amounts) => BonusPayouts += amounts;
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
            data.Add(BonusPayouts);
            data.Add((int)BigColor);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader bStream)
        {
            // ボーナス当選履歴(1つずつ読み込み。5バイトで読み込み)

            try
            {
                // ボーナス履歴
                BonusID = (BonusType)Enum.ToObject(typeof(BonusType), bStream.ReadInt32());
                Debug.Log("BonusID:" + BonusID);
                // ボーナス成立時G
                BonusHitGame = bStream.ReadInt32();
                Debug.Log("BonusHit:" + BonusHitGame);
                // ボーナス当選G
                BonusStartGame = bStream.ReadInt32();
                Debug.Log("BonusStartGame:" + BonusStartGame);
                // ボーナス払い出し枚数
                BonusPayouts = bStream.ReadInt32();
                Debug.Log("BonusPayouts:" + BonusPayouts);
                // BIG時の色
                BigColor = (BigColor)Enum.ToObject(typeof(BigColor), bStream.ReadInt32());
                Debug.Log("BigColor:" + BigColor);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Debug.Log("BonusData end");
            }

            return true;
        }
    }
}