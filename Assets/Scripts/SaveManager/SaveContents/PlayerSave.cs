using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Analytics;
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

            public int TotalGames { get; private set; }             // 総ゲーム数(ボーナス中除く)
            public int CurrentGames { get; private set; }           // ボーナス間ゲーム数
            public int BigTimes { get; private set; }               // ビッグチャンス成立回数
            public int RegTimes { get; private set; }               // ボーナスゲーム成立回数

            public PlayerMedalData PlayerMedalData { get; private set; }            // メダル情報
            public List<BonusHitData> BonusHitRecord { get; private set; }          // 当選させたボーナス(IDごとに)
            public AnalyticsData PlayerAnalyticsData { get; private set; }          // 解析データ

            // コンストラクタ
            public PlayerSave()
            {
                TotalGames = 0;
                CurrentGames = 0;
                BigTimes = 0;
                RegTimes = 0;
                PlayerMedalData = new PlayerMedalData();
                BonusHitRecord = new List<BonusHitData>();
                PlayerAnalyticsData = new AnalyticsData();
            }

            // データ記録
            public void RecordData(PlayerDatabase playerData)
            {
                TotalGames = playerData.TotalGames;
                CurrentGames = playerData.CurrentGames;
                BigTimes = playerData.BigTimes;
                RegTimes = playerData.RegTimes;
                PlayerMedalData = playerData.PlayerMedalData;
                BonusHitRecord = playerData.BonusHitRecord;
                PlayerAnalyticsData = playerData.PlayerAnalyticsData;
            }

            // セーブ
            public List<int> SaveData()
            {
                // 変数データをすべて格納
                List<int> data = new List<int>();

                data.Add(TotalGames);
                data.Add(CurrentGames);
                data.Add(BigTimes);
                data.Add(RegTimes);

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

                // 解析情報
                foreach (int list in PlayerAnalyticsData.SaveData())
                {
                    data.Add(list);
                }

                return data;
            }

            // セーブ読み込み
            public bool LoadData(BinaryReader br)
            {
                try
                {
                    TotalGames = br.ReadInt32();
                    CurrentGames = br.ReadInt32();
                    BigTimes = br.ReadInt32();
                    RegTimes = br.ReadInt32();

                    // メダル情報読み込み
                    PlayerMedalData.LoadData(br);

                    // ボーナス履歴読み込み
                    // ボーナス履歴数

                    int bonusResultCount = br.ReadInt32();

                    // 履歴分読み込む
                    for (int i = 0; i < bonusResultCount; i++)
                    {
                        BonusHitData buffer = new BonusHitData();
                        buffer.LoadData(br);
                        BonusHitRecord.Add(buffer);
                    }

                    // 解析情報読み込み
                    PlayerAnalyticsData.LoadData(br);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
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

