using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace ReelSpinGame_Save.Encryption
{
    // 複合化機能
    public class SaveDecryptor
    {
        // ファイル復号
        public string DecodeFile(FileStream file, string keyPath)
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
                    data = DecryptData(data, keyPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(e.ToString());
            }

            return data;
        }

        // 復号
        string DecryptData(string cipherText, string keyPath)
        {
            string plainText = null;

            using (Aes aes = Aes.Create())
            {
                // 復号作成
                aes.BlockSize = SaveEncryptionSetting.BlockSize;
                aes.KeySize = SaveEncryptionSetting.KeySize;
                aes.Mode = SaveEncryptionSetting.Mode;
                aes.Padding = SaveEncryptionSetting.Padding;

                LoadKeyFile(aes, keyPath);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // 出力作成
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream cts = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cts))
                {
                    plainText = sr.ReadToEnd();
                }
            }

            return plainText;
        }

        // AESキーの読み込み
        void LoadKeyFile(Aes aes, string path)
        {
            try
            {
                using (FileStream fs = File.OpenRead(path))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // シークを終端に移動
                    br.BaseStream.Position = br.BaseStream.Length - sizeof(int);

                    // シード値を読む
                    int seed = br.ReadInt32();

                    // 戻す
                    br.BaseStream.Position = 0;

                    // 暗号化シードから読み込む
                    aes.Key = DecryptBytesBySeed(br.ReadBytes(SaveEncryptionSetting.KeySize / 8), seed);
                    aes.IV = DecryptBytesBySeed(br.ReadBytes(SaveEncryptionSetting.BlockSize / 8), seed);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(e.ToString());
            }
        }

        // シード値から復号
        byte[] DecryptBytesBySeed(byte[] data, int seed)
        {
            byte[] decryptedByte = data;
            System.Random rand = new System.Random(seed);

            for (int i = 0; i < decryptedByte.Length; i++)
            {
                decryptedByte[i] ^= (byte)rand.Next(255);
            }

            return decryptedByte;
        }
    }
}