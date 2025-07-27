using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using ReelSpinGame_System;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ReelSpinGame_Save.Player
{
    namespace ReelSpinGame_System
    {
        public class PlayerSave : ISavable
        {
            // プレイヤーセーブデータ

            // var
            // ゲーム数情報
            // 総ゲーム数(ボーナス中除く)
            public int TotalGames { get; private set; }
            // ボーナス間ゲーム数
            public int CurrentGames { get; private set; }

            // メダル情報
            public PlayerMedalData PlayerMedalData { get; private set; }

            // 当選させたボーナス(IDごとに)
            public List<BonusHitData> BonusHitRecord { get; private set; }

            // ビッグチャンス成立回数
            public int BigTimes { get; private set; }
            // ボーナスゲーム成立回数
            public int RegTimes { get; private set; }

            // コンストラクタ
            public PlayerSave()
            {
                TotalGames = 0;
                CurrentGames = 0;
                PlayerMedalData = new PlayerMedalData();
                BonusHitRecord = new List<BonusHitData>();
                BigTimes = 0;
                RegTimes = 0;
            }

            // func

            // データ記録
            public void RecordData(PlayerDatabase playerData)
            {
                TotalGames = playerData.TotalGames;
                CurrentGames = playerData.CurrentGames;
                PlayerMedalData = playerData.PlayerMedalData;
                BonusHitRecord = playerData.BonusHitRecord;
                BigTimes = playerData.BigTimes;
                RegTimes = playerData.RegTimes;
            }

            // セーブ
            public List<int> SaveData()
            {
                // 変数データをすべて格納
                List<int> data = new List<int>();

                data.Add(TotalGames);
                data.Add(CurrentGames);

                // メダル情報
                foreach (int list in PlayerMedalData.SaveData())
                {
                    data.Add(list);
                }

                // ボーナス情報の数
                data.Add(BonusHitRecord.Count);

                // ボーナス情報
                for (int i = 0; i < BonusHitRecord.Count; i++)
                {
                    foreach (int list in BonusHitRecord[i].SaveData())
                    {
                        data.Add(list);
                    }
                }

                // BIG/REG回数
                data.Add(BigTimes);
                data.Add(RegTimes);

                return data;
            }

            // セーブ読み込み
            public bool LoadData(BinaryReader br)
            {
                try
                {
                    // ゲーム数読み込み
                    TotalGames = br.ReadInt32();
                    //Debug.Log("TotalGames:" + TotalGames);

                    CurrentGames = br.ReadInt32();
                    //Debug.Log("CurrentGames:" + CurrentGames);

                    // メダル情報読み込み
                    PlayerMedalData.LoadData(br);

                    // ボーナス履歴読み込み
                    // ボーナス履歴数

                    int bonusResultCounts = br.ReadInt32();
                    //Debug.Log("BonusResultCounts:" + bonusResultCounts);

                    // 履歴分読み込む
                    for (int i = 0; i < bonusResultCounts; i++)
                    {
                        BonusHitData buffer = new BonusHitData();
                        BonusHitRecord.Add(buffer);
                        buffer.LoadData(br);
                    }

                    //Debug.Log("BonusLoad END");

                    // BIG回数
                    BigTimes = br.ReadInt32();
                    //Debug.Log("BigTimes:" + BigTimes);

                    // REG回数
                    RegTimes = br.ReadInt32();
                    //Debug.Log("RegTimes:" + RegTimes);
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
                finally
                {
                    //Debug.Log("PlayerData Read is done");
                }

                return true;
            }
        }
    }

}

