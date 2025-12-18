using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_System
{

    // セーブマネージャーのインターフェース
    public interface ISaveManage
    {
        public List<int> GenerateDataBuffer(); // データバッファを作成
        public bool LoadDataBuffer(Stream baseStream, BinaryReader br); //データバッファの読み込み
    }

    // セーブ機能
    public class SaveManager
    {
        // const
        // セーブファイルアドレス
        const string PlayerSavePath = "/Nostalgia/save.sav"; // プレイヤーセーブ
        const string PlayerKeyPath = "/Nostalgia/save.key"; // プレイヤー暗号鍵
        const string OptionSavePath = "/Nostalgia/core.sav"; // オプションセーブ
        const string OptionKeyPath = "/Nostalgia/core.key"; // オプション暗号鍵

        // アドレス番地
        private enum AddressID { Setting, Player, Medal, FlagC, Reel, Bonus}

        // var
        // プレイヤーのセーブデータ
        public SaveDatabase PlayerSaveData { get { return playerSaveManager.CurrentSave; } }
        // オプションのセーブデータ
        public OptionSave OptionSave { get { return optionSaveManager.CurrentSave; } }

        PlayerSaveManager playerSaveManager; // プレイヤーのセーブマネージャー
        OptionSaveManager optionSaveManager; // オプションのセーブマネージャー
        SaveEncryptor saveEncryptor; // 暗号化機能

        // コンストラクタ
        public SaveManager()
        {
            saveEncryptor = new SaveEncryptor();
            playerSaveManager = new PlayerSaveManager();
            optionSaveManager = new OptionSaveManager();
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
            try
            {
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        // プレイヤーセーブ削除
        public void DeletePlayerSave()
        {
            string path = Application.persistentDataPath + PlayerSavePath;
            string keyPath = Application.persistentDataPath + PlayerKeyPath;
            DeleteSave(path, keyPath);
        }

        // プレイヤーセーブファイル作成
        public bool GeneratePlayerSave()
        {
            string path = Application.persistentDataPath + PlayerSavePath;
            string keyPath = Application.persistentDataPath + PlayerKeyPath;

            // 前のセーブを消去
            if (Directory.Exists(path))
            {
                DeleteSave(path, keyPath);
            }

            try
            {
                using (FileStream file = File.OpenWrite(path))
                using (StreamWriter sw = new StreamWriter(file))
                {
                    List<int> dataBuffer = new List<int>(playerSaveManager.GenerateDataBuffer());
                    // すべての数値を書き込んだらバイト配列にし、暗号化して保存
                    sw.Write(saveEncryptor.EncryptData(BitConverter.ToString(GetBytesFromList(dataBuffer)), keyPath));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        // プレイヤーセーブ読み込み
        public bool LoadPlayerSave()
        {
            string path = Application.persistentDataPath + PlayerSavePath;
            string keyPath = Application.persistentDataPath + PlayerKeyPath;

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                Debug.LogError("File not found");
                return false;
            }
            try
            {
                // ファイルの復号化をする
                using (FileStream file = File.OpenRead(path))
                {
                    string playerData = DecodeFile(file, keyPath); // プレイヤーのデータ

                    // 文字列をバイト配列に戻し復元開始。ハッシュ値参照も行う
                    using (MemoryStream ms = new MemoryStream(GetBytesFromString(playerData)))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        using (Stream baseStream = br.BaseStream)
                        {
                            // ハッシュ値が正しければデータ読み込み
                            if(CheckHash(baseStream, br))
                            {
                                playerSaveManager.LoadDataBuffer(baseStream, br);
                            }
                            else
                            {
                                return false;
                            }
                        }
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

        // オプションセーブファイル作成
        public bool GenerateOptionSave()
        {
            string path = Application.persistentDataPath + OptionSavePath;
            string keyPath = Application.persistentDataPath + OptionKeyPath;

            // 前のセーブを消去
            if (Directory.Exists(path))
            {
                DeleteSave(path, keyPath);
            }
            try
            {
                using (FileStream file = File.OpenWrite(path))
                using (StreamWriter sw = new StreamWriter(file))
                {
                    List<int> dataBuffer = new List<int>(optionSaveManager.GenerateDataBuffer());
                    Debug.Log(dataBuffer.Count);
                    // すべての数値を書き込んだらバイト配列にし、暗号化して保存
                    sw.Write(saveEncryptor.EncryptData(BitConverter.ToString(GetBytesFromList(dataBuffer)), keyPath));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        // オプションファイル読み込み
        public bool LoadOptionSave()
        {
            string path = Application.persistentDataPath + OptionSavePath;
            string keyPath = Application.persistentDataPath + OptionKeyPath;

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                Debug.LogError("File not found");
                return false;
            }
            try
            {
                // ファイルの復号化をする
                using (FileStream file = File.OpenRead(path))
                {
                    string optionData = DecodeFile(file, keyPath); // 設定データ

                    // 文字列をバイト配列に戻し復元開始。ハッシュ値参照も行う
                    using (MemoryStream ms = new MemoryStream(GetBytesFromString(optionData)))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        using (Stream baseStream = br.BaseStream)
                        {
                            // ハッシュ値が正しければデータ読み込み
                            if (CheckHash(baseStream, br))
                            {
                                optionSaveManager.LoadDataBuffer(baseStream, br);
                            }
                            else
                            {
                                return false;
                            }
                        }
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

        // オプションセーブ削除
        public void DeleteOptionSave()
        {
            string path = Application.persistentDataPath + OptionSavePath;
            string keyPath = Application.persistentDataPath + OptionKeyPath;
            DeleteSave(path, keyPath);
        }

        // ファイル削除
        bool DeleteSave(string path, string keyPath)
        {
            try
            {
                File.Delete(path);
                File.Delete(keyPath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        // ファイル復号
        string DecodeFile(FileStream file, string keyPath)
        {
            // データ
            string data;
            try
            {
                using (StreamReader streamRead = new StreamReader(file))
                using (Stream baseStream = streamRead.BaseStream)
                {
                    // 復号
                    data = streamRead.ReadToEnd();
                    data = saveEncryptor.DecryptData(data, keyPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }

            return data;
        }

        // ハッシュ値チェック
        bool CheckHash(Stream baseStream, BinaryReader br)
        {
            try
            {
                // ハッシュ値の参照
                baseStream.Seek(-4, SeekOrigin.End);
                int previousHash = br.ReadInt32();

                // ハッシュ値以外を読み込みリストファイルを作る
                List<int> intData = new List<int>();
                baseStream.Seek(0, SeekOrigin.Begin);

                while (baseStream.Position != baseStream.Length - sizeof(int))
                {
                    intData.Add(br.ReadInt32());
                }

                // 新しく作ったリストのハッシュ値が一致するかチェックする
                int newHash = BitConverter.ToString(GetBytesFromList(intData)).GetHashCode();

                // ハッシュ値が合わない場合は読み込まない
                if (previousHash != newHash)
                {
                    Debug.LogError("Hash code is wrong");
                    return false;
                }

                baseStream.Position = 0;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }


        // int型ListからByte配列を得る
        public static byte[] GetBytesFromList(List<int> lists)
        {
            List<byte> bytes = new List<byte>();

            // int型Listから数値を取る
            foreach (int i in lists)
            {
                // 全ての数値をbyte変換
                foreach (byte b in BitConverter.GetBytes(i))
                {
                    bytes.Add(b);
                }
            }

            return bytes.ToArray();
        }

        // 文字列からバイト配列を作成
        public static byte[] GetBytesFromString(string byteString)
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
