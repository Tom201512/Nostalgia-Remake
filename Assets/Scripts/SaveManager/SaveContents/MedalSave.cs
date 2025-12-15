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
        public int Credit { get; private set; }             // クレジット枚数
        public int MaxBetAmount { get; private set; }       // 最高ベット枚数
        public int LastBetAmount { get; private set; }      // 最後にかけたメダル枚数
        public bool HasReplay { get; private set; }         // リプレイ状態か

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
            //Debug.Log("Credit:" + Credit);
            MaxBetAmount = medal.MaxBetAmount;
            //Debug.Log("MaxBetAmount:" + MaxBetAmount);
            LastBetAmount = medal.LastBetAmount;
            //Debug.Log("LastBetAmount:" + LastBetAmount);
            HasReplay = medal.HasReplay;
            //Debug.Log("HasReplay:" + HasReplay);
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add(Credit);
            //Debug.Log("Credit:" + Credit);
            data.Add(MaxBetAmount);
            //Debug.Log("MaxBetAmount:" + MaxBetAmount);
            data.Add(LastBetAmount);
            //Debug.Log("LastBetAmount:" + LastBetAmount);
            data.Add(HasReplay ? 1 : 0);
            //Debug.Log("HasReplay:" + HasReplay);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                Credit = br.ReadInt32();
                //Debug.Log("Credit:" + Credit);
                MaxBetAmount = br.ReadInt32();
                //Debug.Log("MaxBetAmount:" + MaxBetAmount);
                LastBetAmount = br.ReadInt32();
                //Debug.Log("LastBetAmount:" + LastBetAmount);
                HasReplay = (br.ReadInt32() == 1 ? true : false);
                //Debug.Log("HasReplay:" + HasReplay);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }
    }
}

