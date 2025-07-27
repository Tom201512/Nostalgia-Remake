using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
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

        // セーブファイル作成(暗号化)
        public bool GenerateSaveFileWithEncrypt()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

            // 前のセーブを消去
            if (Directory.Exists(path))
            {
                Debug.Log("Overwrite file");
                File.Delete(path);
            }

            try
            {
                using (FileStream file = File.OpenWrite(path))
                using (StreamWriter sw = new StreamWriter(file))
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

                    // すべての数値を書き込んだらバイト配列にし、暗号化して保存
                    sw.Write(saveEncryptor.EncryptData(BitConverter.ToString(GetBytesFromList(dataBuffer))));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            Debug.Log("Save Encryption is succeeded");
            return true;
        }

        // セーブファイル読み込み(旧バージョンの読み込み)
        public bool LoadOldSaveFile()
        {
            // セーブを読み込む
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                Debug.Log("No old save file");
                return false;
            }

            try
            {
                using (FileStream file = File.OpenRead(path))
                {
                    int index = 0;

                    using (BinaryReader br = new BinaryReader(file))
                    using (Stream baseStream = br.BaseStream)
                    {
                        while (baseStream.Position != baseStream.Length)
                        {
                            SetValueFromData(br, index);
                            index += 1;
                        }

                        Debug.Log("EOF");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            // 古いファイルは消す
            File.Delete(path);

            Debug.Log("Load Done. Old file is deleted");
            return true;
        }

        // セーブ読み込み(暗号化あり)
        public bool LoadSaveFileWithDecryption()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                // ファイルの復号化をする
                using (FileStream file = File.OpenRead(path))
                {
                    // 読み込み位置
                    int index = 0;

                    // データ
                    string data;
                    // ファイル読み込みをして復号する
                    using (StreamReader stream = new StreamReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        // 復号
                        data = stream.ReadToEnd();
                        data = saveEncryptor.DecryptData(data);
                        Debug.Log("File EOF");
                    }

                    // 文字列をバイト配列に戻し復元開始
                    using (MemoryStream ms = new MemoryStream(GetBytesFromString(data)))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        using (Stream baseStream = br.BaseStream)
                        {
                            while (baseStream.Position != baseStream.Length)
                            {
                                SetValueFromData(br, index);
                                index += 1;
                            }
                            Debug.Log("Binary EOF");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Load failed");
                throw new Exception(e.ToString());
            }

            Debug.Log("Load with Decryption done");
            return true;
        }

        // デバッグ用
        public void DecrpytionTest()
        {
            string data = "Test";
            Debug.Log("Encrypting start");
            data = saveEncryptor.EncryptData(data);
            Debug.Log("Decrypting start");
            data = saveEncryptor.DecryptData(data);
            Debug.Log("Decrypted:" + data);

            data = "t34y2GKpvvBLEIiPrK9/gQ=";
            //Debug.Log("Decrypting start");
            //data = saveEncryptor.DecryptData(data);
            //Debug.Log("Decrypted:" + data);
        }

        // セーブ削除
        public void DeleteSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

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
        private void SetValueFromData(BinaryReader br, int addressID)
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
                        // 左
                        CurrentSave.LastReelPos[(int)ReelLeft] = br.ReadInt32();
                        // 中
                        CurrentSave.LastReelPos[(int)ReelMiddle] = br.ReadInt32();
                        // 右
                        CurrentSave.LastReelPos[(int)ReelRight] = br.ReadInt32();
                        break;

                    // ボーナス情報
                    case (int)AddressID.Bonus:
                        CurrentSave.Bonus.LoadData(br);
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
        public static byte[] GetBytesFromList(List<int> lists)
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

        // 文字列からバイト配列を作成
        private byte[] GetBytesFromString(string byteString)
        {
            List<byte> bytes = new List<byte>();
            string[] buffer = byteString.Split("-");
            
            foreach(string s in buffer)
            {
                bytes.Add(Convert.ToByte(s, 16));
            }

            return bytes.ToArray();
        }
    }
}
