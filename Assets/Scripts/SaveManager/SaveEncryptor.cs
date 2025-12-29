using System;
using System.IO;
using System.Security.Cryptography;

namespace ReelSpinGame_Save.Encryption
{
    // 暗号化機能
    public class SaveEncryptor
    {
        // セーブデータ(byte配列)の暗号化
        public string EncryptData(string plainText, string keyPath)
        {
            using (Aes aes = Aes.Create())
            {
                // 暗号化作成
                aes.BlockSize = SaveEncryptionSetting.BlockSize;
                aes.KeySize = SaveEncryptionSetting.KeySize;
                aes.Mode = SaveEncryptionSetting.Mode;
                aes.Padding = SaveEncryptionSetting.Padding;

                aes.GenerateIV();
                aes.GenerateKey();

                GenerateKeyFile(aes.Key, aes.IV, keyPath);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encryptedBytes;

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cts = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cts))
                    {
                        sw.Write(plainText);
                    }
                    encryptedBytes = ms.ToArray();
                }

                // Base64文字列に変換
                return (Convert.ToBase64String(encryptedBytes));
            }
        }

        // AESキーの暗号化ファイル作成
        void GenerateKeyFile(byte[] key, byte[] IV, string path)
        {
            // ランダム生成
            int seed = (int)DateTime.Now.Ticks;

            try
            {
                using (FileStream fs = File.OpenWrite(path))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    // 暗号化する
                    bw.Write(EncryptBytesBySeed(key, seed));
                    bw.Write(EncryptBytesBySeed(IV, seed));
                    bw.Write(seed);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        // タイムシードによる暗号化
        byte[] EncryptBytesBySeed(byte[] data, int seed)
        {
            byte[] cryptedByte = data;
            System.Random rand = new System.Random(seed);

            for (int i = 0; i < cryptedByte.Length; i++)
            {
                cryptedByte[i] ^= (byte)rand.Next(255);
            }

            return cryptedByte;
        }
    }
}