using ReelSpinGame_Save.Database;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_System
{
    // プレイヤーのセーブ機能
    public class PlayerSaveManager : ISaveManage
    {
        // const
        // アドレス番地
        private enum AddressID { Setting, Player, Medal, FlagC, Reel, Bonus }

        // var
        // 現在のセーブデータ
        public SaveDatabase CurrentSave { get; private set; }

        // コンストラクタ
        public PlayerSaveManager()
        {
            CurrentSave = new SaveDatabase();
        }

        // func

        // バッファ作成
        public List<int> GenerateDataBuffer()
        {
            // セーブファイルを作成するための整数型List作成
            List<int> dataBuffer = new List<int>();

            // 台設定
            dataBuffer.Add(CurrentSave.Setting);

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
            int hash = BitConverter.ToString(SaveManager.GetBytesFromList(dataBuffer)).GetHashCode();
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
                        Debug.LogError("Failed to Load data at:" + (AddressID)Enum.ToObject(typeof(AddressID), index));
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
                        CurrentSave.RecordSlotSetting(br.ReadInt32());
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
                Debug.LogError("Loading error happened at:" + (AddressID)Enum.ToObject(typeof(ReelSymbols), addressID));
                return false;
            }

            return true;
        }
    }
}