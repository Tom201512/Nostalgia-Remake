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
        public int Credit { get; private set; }
        // 最高ベット枚数
        public int MaxBetAmount { get; private set; }
        // 最後にかけたメダル枚数
        public int LastBetAmount { get; private set; }
        // リプレイ状態か
        public bool HasReplay { get; private set; }

        public MedalSave()
        {
            Credit = 0;
            MaxBetAmount = MaxBetLimit;
            LastBetAmount = 0;
            HasReplay = false;
        }

        // func

        // データ記録
        public void RecordData(MedalSystemData medal)
        {
            Credit = medal.Credit;
            MaxBetAmount = medal.MaxBetAmount;
            LastBetAmount = medal.LastBetAmount;
            HasReplay = medal.HasReplay;
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add(Credit);
            data.Add(MaxBetAmount);
            data.Add(LastBetAmount);
            data.Add(HasReplay ? 1 : 0);

            // デバッグ用
            //Debug.Log("MedalData:");
            //foreach (int i in data)
            //{
            //    Debug.Log(i);
            //}

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                // クレジット枚数
                Credit = br.ReadInt32();
                //Debug.Log("Credit:" + Credit);

                // 最大ベット枚数
                MaxBetAmount = br.ReadInt32();
                //Debug.Log("MaxBetAmount:" + MaxBetAmount);

                // 最後に掛けた枚数
                LastBetAmount = br.ReadInt32();
                //Debug.Log("LastBetAmount:" + LastBetAmount);

                // リプレイ状態
                HasReplay = (br.ReadInt32() == 1 ? true : false);
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

