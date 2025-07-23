using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Save.Medal
{
    // メダル情報のセーブ
    public class MedalSave : ISavable
    {
        // const

        // var
        // クレジット枚数
        public int Credits { get; private set; }
        // 最高ベット枚数
        public int MaxBetAmounts { get; private set; }
        // 最後にかけたメダル枚数
        public int LastBetAmounts { get; private set; }
        // リプレイ状態か
        public bool HasReplay { get; private set; }

        public MedalSave()
        {
            Credits = 0;
            MaxBetAmounts = MaxBetLimit;
            LastBetAmounts = 0;
            HasReplay = false;
        }

        // func

        // データ記録
        public void RecordData(MedalSystemData medal)
        {
            Credits = medal.Credits;
            MaxBetAmounts = medal.MaxBetAmounts;
            LastBetAmounts = medal.LastBetAmounts;
            HasReplay = medal.HasReplay;
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add(Credits);
            data.Add(MaxBetAmounts);
            data.Add(LastBetAmounts);
            data.Add(HasReplay ? 1 : 0);

            // デバッグ用
            Debug.Log("MedalData:");
            foreach (int i in data)
            {
                Debug.Log(i);
            }

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // クレジット枚数
                Credits = bStream.ReadInt32();
                //Debug.Log("Credits:" + Credits);

                // 最大ベット枚数
                MaxBetAmounts = bStream.ReadInt32();
                //Debug.Log("MaxBetAmounts:" + MaxBetAmounts);

                // 最後に掛けた枚数
                LastBetAmounts = bStream.ReadInt32();
                //Debug.Log("LastBetAmounts:" + LastBetAmounts);

                // リプレイ状態
                HasReplay = (bStream.ReadInt32() == 1 ? true : false);
                //Debug.Log("HasReplay:" + HasReplay);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("MedalSystem Loaded");
            }

            return true;
        }
    }
}

