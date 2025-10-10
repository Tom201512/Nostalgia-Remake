﻿using ReelSpinGame_Interface;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace ReelSpinGame_Datas
{
    // プレイヤーのメダル情報
    public class PlayerMedalData: ISavable
    {
        // const
        // プレイヤー初期メダル枚数
        public const int DefaultPlayerMedal = 50;

        // var
        // 所有メダル枚数
        public int CurrentPlayerMedal { get; private set; }
        // 投入したメダル枚数(IN)
        public int CurrentInMedal { get; private set; }
        // 獲得メダル枚数(OUT)
        public int CurrentOutMedal { get; private set; }
        // 99999Gまでの差枚数を記録
        public List<int> MedalSlumpGraph { get; private set; }

        // コンストラクタ
        public PlayerMedalData()
        {
            CurrentPlayerMedal = DefaultPlayerMedal;
            CurrentInMedal = 0;
            CurrentOutMedal = 0;
            MedalSlumpGraph = new List<int>();
        }

        // func
        // データからセットする
        public void SetData(ISavable playerMedalData)
        {
            if(playerMedalData.GetType() == typeof(PlayerMedalData))
            {
                PlayerMedalData data = playerMedalData as PlayerMedalData;
                CurrentPlayerMedal = data.CurrentPlayerMedal;
                CurrentInMedal = data.CurrentInMedal;
                CurrentOutMedal = data.CurrentOutMedal;
            }
            else
            {
                throw new Exception("Data is not PlayerMedalData");
            }
        }

        // プレイヤーメダル増加
        public void IncreasePlayerMedal(int amount) => CurrentPlayerMedal += amount;
        // プレイヤーメダル減少
        public void DecreasePlayerMedal(int amount)
        {
            CurrentPlayerMedal -= amount;

            // 0になったら50追加
            while (CurrentPlayerMedal < 0)
            {
                CurrentPlayerMedal += DefaultPlayerMedal;
            }
        }

        // IN増加
        public void IncreaseInMedal(int amount) => CurrentInMedal += amount;
        // OUT増加
        public void IncreaseOutMedal(int amount) => CurrentOutMedal += amount;

        // 差枚を記録する
        public void CountCurrentSlumpGraph() => MedalSlumpGraph.Add(CurrentOutMedal - CurrentInMedal);

        // データ書き込み
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add(CurrentPlayerMedal);
            Debug.Log("CurrentPlayerMedal:" + CurrentPlayerMedal);
            data.Add(CurrentInMedal);
            Debug.Log("CurrentInMedal:" + CurrentInMedal);
            data.Add(CurrentOutMedal);
            Debug.Log("CurrentOutMedal:" + CurrentOutMedal);

            // 差枚数のデータ数を読み込み
            int loadCount = MedalSlumpGraph.Count;
            Debug.Log("SlumpGraphCount:" +  loadCount);
            data.Add(loadCount);

            for (int i = 0; i < loadCount; i++)
            {
                data.Add(MedalSlumpGraph[i]);
                Debug.Log("[" + i + "]:" + loadCount);
            }

            return data;
        }

        // データ読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                CurrentPlayerMedal = br.ReadInt32();
                Debug.Log("Current Medal:" + CurrentPlayerMedal);
                CurrentInMedal = br.ReadInt32();
                Debug.Log("Current IN:" + CurrentInMedal);
                CurrentOutMedal = br.ReadInt32();
                Debug.Log("Current OUT:" + CurrentOutMedal);

                int loadCount = br.ReadInt32();
                Debug.Log("LoadCount:" + loadCount);
                for (int i = 0; i < loadCount; i++)
                {
                    MedalSlumpGraph.Add(br.ReadInt32());
                    Debug.Log("Slump [" + i + "]:" + MedalSlumpGraph[i]);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            Debug.Log("Medal Load is done");
            return true;
        }
    }
}