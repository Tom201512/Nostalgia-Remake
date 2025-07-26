using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;
using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;

namespace ReelSpinGame_System
{
    public class SaveManager
    {
        // セーブ機能

        // const
        // アドレス番地
        private enum AddressID { Setting, Player, Medal, FlagC, Reel, Bonus}

        // var
        // 現在のセーブデータ
        public SaveDatabase CurrentSave { get; private set; }

        // 暗号化機能
        private SaveEncryptor saveEncryptor;

        // コンストラクタ
        public SaveManager()
        {
            CurrentSave = new SaveDatabase();
            saveEncryptor = new SaveEncryptor();
        }

        // func
        // セーブフォルダ作成
        public bool GenerateSaveFolder()
        {
            string path = Application.persistentDataPath + "/Nostalgia";

            // フォルダがあるか確認
            if (Directory.Exists(path))
            {
                return false;
            }

            // ない場合は作成
            Directory.CreateDirectory(path);
            Debug.Log("Directory is created");
            return true;
        }

        // セーブファイル作成
        public bool GenerateSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            // 暗号化機能の初期化
            saveEncryptor.GenerateCryptBytes();

            // 前のセーブを消去
            if (Directory.Exists(path))
            {
                Debug.Log("Overwrite file");
                File.Delete(path);
            }

            try
            {
                using (FileStream file = File.OpenWrite(path))
                {
                    // セーブファイルを作成するための整数型List作成
                    List<int> dataBuffer = new List<int>();

                    // 台設定
                    dataBuffer.Add(CurrentSave.Setting);
                    //file.Write(BitConverter.GetBytes(CurrentSave.Setting));

                    // プレイヤーデータ
                    foreach(int i in CurrentSave.Player.SaveData())
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.Player.SaveData()));

                    // メダルデータ
                    foreach (int i in CurrentSave.Medal.SaveData())
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.Medal.SaveData()));

                    // フラグカウンタ
                    dataBuffer.Add(CurrentSave.FlagCounter);
                    //file.Write(BitConverter.GetBytes(CurrentSave.FlagCounter));

                    //リール停止位置
                    //Debug.Log("ReelPos");
                    foreach (int i in CurrentSave.LastReelPos)
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.LastReelPos));

                    // ボーナス情報
                    //Debug.Log("Bonus");
                    foreach (int i in CurrentSave.Bonus.SaveData())
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.Bonus.SaveData()));

                    // すべての数値を書き込んだら暗号化

                    // 書き込み
                    WriteSave(file, dataBuffer);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                Debug.Log("Save is succeeded");
            }
            return true;
        }

        // セーブファイル読み込み
        public bool LoadSaveFile()
        {
            // 現在日時、時刻をセーブから読み込む

            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                using (FileStream file = File.OpenRead(path))
                {
                    int index = 0;
                    BinaryReader stream = new BinaryReader(file);
                    Stream baseStream = stream.BaseStream;

                    while (baseStream.Position != baseStream.Length)
                    {
                        SetValueFromData(stream, index);
                        index += 1;
                    }
                    //Debug.Log("EOF");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return true;
        }

        // セーブ削除
        public void DeleteSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // セーブデータ書き込み
        private void WriteSave(FileStream file, List<int> data)
        {
            file.Write(GetBytesFromList(data));
        }

        // データ番地ごとに数字をセット
        private void SetValueFromData(BinaryReader bStream, int addressID)
        {
            try
            {
                switch (addressID)
                {
                    case (int)AddressID.Setting:
                        CurrentSave.RecordSlotSetting(bStream.ReadInt32());
                        //Debug.Log("Setting:" + CurrentSave.Setting);

                        break;

                    case (int)AddressID.Player:
                        //Debug.Log("Player");
                        CurrentSave.Player.LoadData(bStream);
                        break;

                    case (int)AddressID.Medal:
                        //Debug.Log("Medal");
                        CurrentSave.Medal.LoadData(bStream);
                        break;

                    case (int)AddressID.FlagC:
                        //Debug.Log("FlagCounter");
                        CurrentSave.RecordFlagCounter(bStream.ReadInt32());
                        //Debug.Log("FlagCounter Loaded");
                        break;

                    case (int)AddressID.Reel:
                        //Debug.Log("Reel");

                        // 左
                        CurrentSave.LastReelPos[(int)ReelLeft] = bStream.ReadInt32();
                        //Debug.Log("ReelL:" + CurrentSave.LastReelPos[(int)ReelLeft]);

                        // 中
                        CurrentSave.LastReelPos[(int)ReelMiddle] = bStream.ReadInt32();
                        //Debug.Log("ReelM:" + CurrentSave.LastReelPos[(int)ReelMiddle]);

                        // 右
                        CurrentSave.LastReelPos[(int)ReelRight] = bStream.ReadInt32();
                        //Debug.Log("ReelR:" + CurrentSave.LastReelPos[(int)ReelRight]);

                        //Debug.Log("Reel Loaded");
                        break;

                    // ボーナス情報
                    case (int)AddressID.Bonus:
                        //Debug.Log("Bonus");
                        CurrentSave.Bonus.LoadData(bStream);
                        break;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {

            }
        }

        // int型ListからByte配列を得る
        private byte[] GetBytesFromList(List<int> lists)
        {
            List<byte> bytes = new List<byte>();

            // int型Listから数値を取る
            foreach (int i in lists)
            {
                // 全ての数値をbyte変換
                foreach(byte b in BitConverter.GetBytes(i))
                {
                    bytes.Add(b);
                }
            }

            return bytes.ToArray();
        }

        // チェックサム作成
        private int MakeCheckSum(int dataNum) => dataNum & 0xFF;
    }
}
