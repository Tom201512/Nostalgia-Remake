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
            List<int> data = new List<int>();
            data.Add(Credit);
            data.Add(MaxBetAmount);
            data.Add(LastBetAmount);
            data.Add(HasReplay ? 1 : 0);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                Credit = br.ReadInt32();
                MaxBetAmount = br.ReadInt32();
                LastBetAmount = br.ReadInt32();
                HasReplay = (br.ReadInt32() == 1 ? true : false);
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

