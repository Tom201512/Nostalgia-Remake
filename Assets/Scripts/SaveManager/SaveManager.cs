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

        // セーブファイル作成
        public bool GenerateSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            // 暗号化機能の初期化
            //saveEncryptor.GenerateCryptBytes();

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

                    // プレイヤーデータ
                    foreach(int i in CurrentSave.Player.SaveData())
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
                    //Debug.Log("ReelPos");
                    foreach (int i in CurrentSave.LastReelPos)
                    {
                        dataBuffer.Add(i);
                    }

                    // ボーナス情報
                    //Debug.Log("Bonus");
                    foreach (int i in CurrentSave.Bonus.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // 書き込み
                    file.Write(GetBytesFromList(dataBuffer));
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

                    /*
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
                    }*/

                    // すべての数値を書き込んだら暗号化して書き込む
                    sw.Write(saveEncryptor.EncryptData("TEST"));
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
            // セーブを読み込む
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

                    using (BinaryReader stream = new BinaryReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        while (baseStream.Position != baseStream.Length)
                        {
                            SetValueFromData(stream, index);
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
                using (FileStream file = File.OpenRead(path))
                {
                    int index = 0;
                    using (StreamReader stream = new StreamReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        // 復号
                        string data = stream.ReadToEnd();
                        Debug.Log("Cipher:" + data);
                        /*
                        while (baseStream.Position != baseStream.Length)
                        {
                            SetValueFromData(stream, index);
                            index += 1;
                        }*/

                        Debug.Log(saveEncryptor.DecryptData(data));

                        Debug.Log("EOF");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Load failed");
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("Load is succeeded");
            }
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
    }
}
