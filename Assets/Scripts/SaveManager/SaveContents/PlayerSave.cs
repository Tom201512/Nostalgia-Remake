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
            // 総ゲーム数(ボーナス中除く)
            public int TotalGames { get; private set; }
            // ボーナス間ゲーム数
            public int CurrentGames { get; private set; }

            // ビッグチャンス成立回数
            public int BigTimes { get; private set; }
            // ボーナスゲーム成立回数
            public int RegTimes { get; private set; }

            // メダル情報
            public PlayerMedalData PlayerMedalData { get; private set; }
            // 当選させたボーナス(IDごとに)
            public List<BonusHitData> BonusHitRecord { get; private set; }
            // 解析データ
            public AnalyticsData PlayerAnalyticsData { get; private set; }

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

            // func

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
                Debug.Log("TotalGames:" + TotalGames);
                data.Add(CurrentGames);
                Debug.Log("CurrentGames:" + CurrentGames);

                // BIG/REG回数
                data.Add(BigTimes);
                Debug.Log("BigTimes:" + BigTimes);
                data.Add(RegTimes);
                Debug.Log("RegTimes:" + RegTimes);

                // メダル情報
                foreach (int list in PlayerMedalData.SaveData())
                {
                    data.Add(list);
                }

                // ボーナス情報の数
                data.Add(BonusHitRecord.Count);
                Debug.Log("BonusData count :" + BonusHitRecord.Count);

                // ボーナス情報
                for (int i = 0; i < BonusHitRecord.Count; i++)
                {
                    foreach (int list in BonusHitRecord[i].SaveData())
                    {
                        data.Add(list);
                    }
                }

                // 解析情報
                foreach(int list in PlayerAnalyticsData.SaveData())
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
                    // ゲーム数読み込み
                    TotalGames = br.ReadInt32();
                    Debug.Log("TotalGames:" + TotalGames);

                    CurrentGames = br.ReadInt32();
                    Debug.Log("CurrentGames:" + CurrentGames);

                    // BIG回数
                    BigTimes = br.ReadInt32();
                    Debug.Log("BigTimes:" + BigTimes);

                    // REG回数
                    RegTimes = br.ReadInt32();
                    Debug.Log("RegTimes:" + RegTimes);

                    // メダル情報読み込み
                    PlayerMedalData.LoadData(br);

                    // ボーナス履歴読み込み
                    // ボーナス履歴数

                    int bonusResultCount = br.ReadInt32();
                    Debug.Log("BonusResultCount:" + bonusResultCount);

                    // 履歴分読み込む
                    for (int i = 0; i < bonusResultCount; i++)
                    {
                        Debug.Log("BonusResult[" + i + "]:");
                        BonusHitData buffer = new BonusHitData();
                        buffer.LoadData(br);
                        BonusHitRecord.Add(buffer);
                    }

                    // 解析情報読み込み
                    PlayerAnalyticsData.LoadData(br);

                    Debug.Log("PlyaerLoad END");

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

