using ReelSpinGame_Reels;
using ReelSpinGame_Save;
using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_System
{
    // プレイヤーのセーブ機能
    public class PlayerSaveManager : ISaveManage
    {
        // アドレス番地
        enum AddressID
        {
            Setting,
            Player,
            Medal,
            FlagC,
            Reel,
            Bonus,
        }

        public SaveDatabase CurrentSave { get; private set; }        // 現在のセーブデータ

        public PlayerSaveManager()
        {
            CurrentSave = new SaveDatabase();
        }

        // バッファ作成
        public List<int> GenerateDataBuffer()
        {
            // セーブファイルを作成するための整数型List作成
            List<int> dataBuffer = new List<int>();

            dataBuffer.Add(CurrentSave.Setting);
            dataBuffer.Add(CurrentSave.IsUsingRandom == true ? 1 : 0);

            // プレイヤーデータ
            foreach (int i in CurrentSave.Player.SaveData())
            {
                dataBuffer.Add(i);
            }

            // メダルデータ
            foreach (int i in CurrentSave.Medal.SaveData())
            {
                dataBuffer.Add(i);
            }

            // フラグカウンタ
            dataBuffer.Add(CurrentSave.FlagCounter);

            //リール停止位置
            foreach (int i in CurrentSave.LastReelPos)
            {
                dataBuffer.Add(i);
            }

            // ボーナス情報
            foreach (int i in CurrentSave.Bonus.SaveData())
            {
                dataBuffer.Add(i);
            }

            // ハッシュ値書き込み
            // 文字列にしてハッシュコードにする
            int hash = BitConverter.ToString(ByteArrayUtil.GetBytesFromList(dataBuffer)).GetHashCode();
            dataBuffer.Add(hash);

            return dataBuffer;
        }

        // バッファ読み込み
        public bool LoadDataBuffer(Stream baseStream, BinaryReader br)
        {
            int index = 0; // 読み込み時のID
            try
            {
                // ハッシュ値以外を読み込む
                while (baseStream.Position != baseStream.Length - sizeof(int))
                {
                    if (SetValueFromData(br, index))
                    {
                        index += 1;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        // データセット
        private bool SetValueFromData(BinaryReader br, int addressID)
        {
            try
            {
                switch (addressID)
                {
                    case (int)AddressID.Setting:
                        CurrentSave.Setting = br.ReadInt32();
                        CurrentSave.IsUsingRandom = (br.ReadInt32() == 1 ? true : false);
                        break;

                    case (int)AddressID.Player:
                        CurrentSave.Player.LoadData(br);
                        break;

                    case (int)AddressID.Medal:
                        CurrentSave.Medal.LoadData(br);
                        break;

                    case (int)AddressID.FlagC:
                        CurrentSave.RecordFlagCounter(br.ReadInt32());
                        break;

                    case (int)AddressID.Reel:
                        CurrentSave.LastReelPos[(int)ReelID.ReelLeft] = br.ReadInt32();
                        CurrentSave.LastReelPos[(int)ReelID.ReelMiddle] = br.ReadInt32();
                        CurrentSave.LastReelPos[(int)ReelID.ReelRight] = br.ReadInt32();
                        break;

                    case (int)AddressID.Bonus:
                        CurrentSave.Bonus.LoadData(br);
                        break;
                }
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