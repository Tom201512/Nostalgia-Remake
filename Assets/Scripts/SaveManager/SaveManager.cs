using ReelSpinGame_Interface;
using ReelSpinGame_Save.Bonus;
using ReelSpinGame_Save.Medal;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;

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

        // 暗号化のバイト配列
        private static byte[] cryptBytes;

        // セーブデータ
        public class SaveDatabase
        {
            // システム

            // 台設定
            public int Setting { get; private set; }

            // プレイヤー情報
            public PlayerSave Player { get; private set; }

            // メダル情報
            public MedalSave Medal { get; private set; }

            // フラグカウンタ数値
            public int FlagCounter { get; private set; }

            // 最後に止まったリール位置
            public List<int> LastReelPos {  get; private set; }

            // ボーナス情報
            public BonusSave Bonus { get; private set; }

            public SaveDatabase()
            {
                Setting = 6;
                Player = new PlayerSave();
                Medal = new MedalSave();
                FlagCounter = 0;
                LastReelPos = new List<int> { 19, 19, 19 };
                Bonus = new BonusSave();
            }

            // func

            // 各種情報記録
            // 台設定
            public void RecordSlotSetting(int setting) => Setting = setting;

            // プレイヤー情報
            public void RecordPlayerSave(ISavable player)
            {
                if (player.GetType() == typeof(PlayerSave))
                {
                    Player = player as PlayerSave;
                }
                else
                {
                    throw new Exception("Save data is not PlayerSave");
                }
            }

            // メダル情報
            public void RecordMedalSave(ISavable medal)
            {
                if(medal.GetType() == typeof(MedalSave))
                {
                    Medal = medal as MedalSave;
                }
                else
                {
                    throw new Exception("Save data is not MedalData");
                }
            }
            
            // フラグカウンタ
            public void RecordFlagCounter(int flagCounter) => FlagCounter = flagCounter;

            // リール位置
            public void RecordReelPos(List<int> lastStopped) => LastReelPos = lastStopped;

            // ボーナス情報
            public void RecordBonusData(ISavable bonus)
            {
                {
                    if (bonus.GetType() == typeof(BonusSave))
                    {
                        Bonus = bonus as BonusSave;
                    }
                    else
                    {
                        throw new Exception("Save data is not BonusSave");
                    }
                }
            }
        }

        // コンストラクタ
        public SaveManager()
        {
            CurrentSave = new SaveDatabase();
            cryptBytes = new byte[] { 0x00, 0x00, 0x00, 0x00 };
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
                    // ここにセーブファイルを書き込む
                    // 台設定
                    file.Write(BitConverter.GetBytes(CurrentSave.Setting));

                    // デバッグ用
                    //Debug.Log("Setting:");
                    //foreach (string d in BitConverter.ToString(BitConverter.GetBytes(CurrentSave.Setting)).Split("-"))
                    //{
                    //    Debug.Log(d);
                    //}

                    // プレイヤーデータ
                    file.Write(GetBytesFromList(CurrentSave.Player.SaveData()));

                    // メダルデータ
                    file.Write(GetBytesFromList(CurrentSave.Medal.SaveData()));

                    // フラグカウンタ
                    file.Write(BitConverter.GetBytes(CurrentSave.FlagCounter));

                    // デバッグ用
                    //Debug.Log("FlagCounter:");
                    //foreach (string d in BitConverter.ToString(BitConverter.GetBytes(CurrentSave.FlagCounter)).Split("-"))
                    //{
                    //    Debug.Log(d);
                    //}

                    //リール停止位置
                    //Debug.Log("ReelPos");
                    file.Write(GetBytesFromList(CurrentSave.LastReelPos));

                    // ボーナス情報
                    //Debug.Log("Bonus");
                    file.Write(GetBytesFromList(CurrentSave.Bonus.SaveData()));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("Save is succeeded");
            }
            return true;
        }

        // セーブファイル読み込み
        public bool LoadSaveFile()
        {
            // 暗号化数値の読み込み

            //デバッグ用
            GenerateCryptBytes(5929843);

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

            //Debug.Log("Byte Data Gen Start");

            // int型Listから数値を取る
            foreach (int i in lists)
            {
                //Debug.Log("Int:" + i);
                // 全ての数値をbyte変換
                foreach(byte b in BitConverter.GetBytes(i))
                {
                    bytes.Add(b);
                }

                // デバッグ用、アドレス表示
                //string[] datas = BitConverter.ToString(BitConverter.GetBytes(i)).Split("-");
                //foreach (string d in datas)
                //{
                    //Debug.Log(d);
                //}
            }

            return bytes.ToArray();
        }

        // チェックサムを得る
        private int GetCheckSum(int dataNum) => dataNum & 0xFF;

        // 暗号化バイト数値作成
        private void GenerateCryptBytes(int dateNum)
        {
            System.Random rand = new System.Random(dateNum);
            // シード値より乱数を得る
            for(int i = 0; i < sizeof(int); i++)
            {
                cryptBytes[i] = (byte)rand.Next(0xFF);
                Debug.Log("Byte:" + cryptBytes[i]);
            }
        }

        // int型Listの暗号化
        private byte[] EncryptData(List<int> intLists)
        {
            // 暗号化したバイト配列
            List<byte> encryptedBytes = new List<byte>();

            // 各数値の暗号化
            foreach(int value in intLists)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                for (int i = 0; i < bytes.Length; i++)
                {
                    encryptedBytes.Add(bytes[i] |= cryptBytes[i]);
                }
            }

            return encryptedBytes.ToArray();
        }

        // 整数値の復号化
        public static int DecryptIntData(int value)
        {
            // 復号化された時のバイト
            byte[] decryptedBytes = BitConverter.GetBytes(value);

            // 復号化
            for (int i = 0; i <  decryptedBytes.Length; i++)
            {
                decryptedBytes[i] |= cryptBytes[i];
            }

            return BitConverter.ToInt32(decryptedBytes);
        }
    }
}
