using ReelSpinGame_Save.Database;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel.ReelID;
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
            Debug.Log("Setting:" + CurrentSave.Setting);

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
            Debug.Log("FlagCounter:" + CurrentSave.FlagCounter);

            //リール停止位置
            foreach (int i in CurrentSave.LastReelPos)
            {
                dataBuffer.Add(i);
            }

            Debug.Log("ReelPos:" + CurrentSave.LastReelPos[(int)ReelLeft] + "," +
                CurrentSave.LastReelPos[(int)ReelMiddle] + "," + CurrentSave.LastReelPos[(int)ReelRight]);

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
                Debug.Log("Load failed");
                return false;
            }

            Debug.Log("Load with Decryption is done");
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
                        Debug.Log("Setting:" + CurrentSave.Setting);
                        break;

                    case (int)AddressID.Player:
                        CurrentSave.Player.LoadData(br);
                        break;

                    case (int)AddressID.Medal:
                        CurrentSave.Medal.LoadData(br);
                        break;

                    case (int)AddressID.FlagC:
                        CurrentSave.RecordFlagCounter(br.ReadInt32());
                        Debug.Log("FlagCounter:" + CurrentSave.FlagCounter);
                        break;

                    case (int)AddressID.Reel:
                        // 左
                        CurrentSave.LastReelPos[(int)ReelLeft] = br.ReadInt32();
                        Debug.Log("Left:" + CurrentSave.LastReelPos[(int)ReelLeft]);
                        // 中
                        CurrentSave.LastReelPos[(int)ReelMiddle] = br.ReadInt32();
                        Debug.Log("Middle:" + CurrentSave.LastReelPos[(int)ReelMiddle]);
                        // 右
                        CurrentSave.LastReelPos[(int)ReelRight] = br.ReadInt32();
                        Debug.Log("Right:" + CurrentSave.LastReelPos[(int)ReelRight]);
                        break;

                    // ボーナス情報
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