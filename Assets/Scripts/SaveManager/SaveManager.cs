using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using static ReelSpinGame_Reels.ReelManagerModel.ReelID;

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
            try
            {
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            //Debug.Log("Directory is created");
            return true;
        }

        // セーブファイル作成
        public bool GenerateSaveFileWithEncrypt()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

            // 前のセーブを消去
            if (Directory.Exists(path))
            {
                DeleteSaveFile();
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
                    //Debug.Log("ListLength:" +  dataBuffer.Count);

                    // 文字列にしてハッシュコードにする
                    int hash = BitConverter.ToString(GetBytesFromList(dataBuffer)).GetHashCode();
                    //Debug.Log("Hash:" + hash);
                    //Debug.Log("HashBytes:" + BitConverter.ToString(BitConverter.GetBytes(hash)));

                    dataBuffer.Add(hash);

                    // すべての数値を書き込んだらバイト配列にし、暗号化して保存
                    sw.Write(saveEncryptor.EncryptData(BitConverter.ToString(GetBytesFromList(dataBuffer))));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            Debug.Log("Save Encryption is succeeded");
            return true;
        }

        // セーブ読み込み
        public bool LoadSaveFileWithDecryption()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

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
                    // データ読み込み位置
                    int index = 0;

                    // データ
                    string data;
                    // ファイル読み込みをして復号
                    using (StreamReader stream = new StreamReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        // 復号
                        data = stream.ReadToEnd();
                        data = saveEncryptor.DecryptData(data);
                        //Debug.Log("File EOF");
                    }

                    // 文字列をバイト配列に戻し復元開始。ハッシュ値参照も行う
                    using (MemoryStream ms = new MemoryStream(GetBytesFromString(data)))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        using (Stream baseStream = br.BaseStream)
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

                            Debug.Log("Hash is correct");

                            baseStream.Position = 0;

                            // ハッシュ値以外を読み込む
                            while (baseStream.Position != baseStream.Length - sizeof(int))
                            {
                                if(SetValueFromData(br, index))
                                {
                                    index += 1;
                                }
                                else
                                {
                                    Debug.LogError("Failed to Load data at:" + index);
                                    return false;
                                }
                            }
                            //Debug.Log("Binary EOF");
                        }
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

        // セーブ削除
        public bool DeleteSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";
            string keyPath = Application.persistentDataPath + "/Nostalgia/save.key";
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

            Debug.Log("Save deletion succeeded");
            return true;
        }

        // データ番地ごとに数字をセット
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
            catch(Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Loading error happened at:" + (AddressID)Enum.ToObject(typeof(ReelSymbols), addressID));
                return false;
            }

            return true;
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
