using ReelSpinGame_Interface;
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

        // コンストラクタ
        public PlayerMedalData()
        {
            CurrentPlayerMedal = DefaultPlayerMedal;
            CurrentInMedal = 0;
            CurrentOutMedal = 0;
        }

        // セーブから読み込む場合
        public PlayerMedalData(int currentPlayerMedal, int currentInMedal, int currentOutMedal)
        {
            CurrentPlayerMedal = currentPlayerMedal;
            CurrentInMedal = currentInMedal;
            CurrentOutMedal = currentOutMedal;
        }

        // func

        // プレイヤーメダル増加
        public void IncreasePlayerMedal(int amounts) => CurrentPlayerMedal += amounts;
        // プレイヤーメダル減少
        public void DecreasePlayerMedal(int amounts)
        {
            CurrentPlayerMedal -= amounts;

            // 0になったら50追加
            while (CurrentPlayerMedal < 0)
            {
                CurrentPlayerMedal += DefaultPlayerMedal;
            }
        }

        // IN増加
        public void IncreaseInMedal(int amounts) => CurrentInMedal += amounts;
        // OUT増加
        public void IncreaseOutMedal(int amounts) => CurrentOutMedal += amounts;

        // データ書き込み
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add(CurrentPlayerMedal);
            data.Add(CurrentInMedal);
            data.Add(CurrentOutMedal);

            return data;
        }

        // データ読み込み
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // プレイヤー所持枚数
                CurrentPlayerMedal = bStream.ReadInt32();
                Debug.Log("CurrentPlayerMedal:" + CurrentPlayerMedal);

                // IN枚数
                CurrentInMedal = bStream.ReadInt32();
                Debug.Log("CurrentInMedal:" + CurrentInMedal);

                // OUT枚数
                CurrentOutMedal = bStream.ReadInt32();
                Debug.Log("CurrentOutMedal:" + CurrentOutMedal);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Debug.Log("MedalData Loaded");
            }

            return true;
        }
    }
}