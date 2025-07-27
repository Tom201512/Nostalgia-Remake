using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

namespace ReelSpinGame_Save.Encryption
{
    // 暗号化機能
    public class SaveEncryptor
    {
        // const

        // AES 初期化ベクトル16文字
        private const string AES_IV = "bjnXjx1BvuRwie9D";
        // AES 暗号鍵32文字
        private const string AES_Key = "fzdqUCx36u7ctl9njUUSo14Ocvg5wh70";

        // AES設定
        private const int BlockSize = 128;
        private const int KeySize = 256;
        private const CipherMode Mode = CipherMode.CBC;
        private const PaddingMode Padding = PaddingMode.PKCS7;

        // func

        // セーブデータ(byte配列)の暗号化
        public string EncryptData(string plainText)
        {
            Debug.Log("Plain:" + plainText);

            using (Aes aes = Aes.Create())
            {
                // 暗号化作成

                aes.BlockSize = BlockSize;
                aes.KeySize = KeySize;
                aes.Mode = Mode;
                aes.Padding = Padding;

                aes.IV = Encoding.UTF8.GetBytes(AES_IV);
                aes.Key = Encoding.UTF8.GetBytes(AES_Key);

                GenerateKeyFile(aes.Key, aes.IV);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // 暗号化されたバイト配列
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

                Debug.Log("Cipher:" + Convert.ToBase64String(encryptedBytes));

                // Base64文字列に変換
                return (Convert.ToBase64String(encryptedBytes));
            }
        }

        // 復号
        
        public string DecryptData(string cipherText)
        {
            Debug.Log("Cipher:" + cipherText);

            // 復号されたテキスト
            string plainText = null;

            // 復号されたバイト配列
            byte[] decryptedBytes = new byte[] {0};

            using (Aes aes = Aes.Create())
            {
                // 復号作成

                aes.BlockSize = BlockSize;
                aes.KeySize = KeySize;
                aes.Mode = Mode;
                aes.Padding = Padding;

                LoadKeyFile(aes);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // 出力作成
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream cts = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cts))
                {
                    plainText = sr.ReadToEnd();
                }
            }

            Debug.Log("Plain:" + plainText);
            return plainText;
        }

        // AESキーの暗号化ファイル作成
        private void GenerateKeyFile(byte[] key, byte[] IV)
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.key";

            Debug.Log(BitConverter.ToString(key));
            Debug.Log(BitConverter.ToString(IV));

            // ランダム生成
            int seed = (int)DateTime.Now.Ticks;
            Debug.Log("Seed:" + seed);
            
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
            catch(Exception e)
            {
                throw new Exception(e.ToString());
            }

            Debug.Log("Key Saved");
        }

        // AESキーの読み込み
        private void LoadKeyFile(Aes aes)
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.key";

            try
            {
                using (FileStream fs = File.OpenRead(path))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // シークを終端に移動
                    br.BaseStream.Position = br.BaseStream.Length - sizeof(int);

                    // シード値を読む
                    int seed = br.ReadInt32();
                    Debug.Log("Seed:" + seed);

                    // 戻す
                    br.BaseStream.Position = 0;

                    // 暗号化シードから読み込む
                    aes.Key = DecryptBytesBySeed(br.ReadBytes(KeySize / 8), seed);
                    aes.IV = DecryptBytesBySeed(br.ReadBytes(BlockSize / 8), seed);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            Debug.Log(BitConverter.ToString(aes.Key));
            Debug.Log(BitConverter.ToString(aes.IV));

            Debug.Log("Key Loaded");
        }

        // タイムシードによる暗号化
        private byte[] EncryptBytesBySeed(byte[] data, int seed)
        {
            byte[] cryptedByte = data;
            System.Random rand = new System.Random(seed);

            Debug.Log("Plain:" + BitConverter.ToString(cryptedByte));

            for (int i = 0; i < cryptedByte.Length; i++)
            {
                cryptedByte[i] ^= (byte)rand.Next(255);
            }

            Debug.Log("Cipher:" + BitConverter.ToString(cryptedByte));

            return cryptedByte;
        }

        // 復号
        private byte[] DecryptBytesBySeed(byte[] data, int seed)
        {
            byte[] cryptedByte = data;
            System.Random rand = new System.Random(seed);

            Debug.Log("Plain:" + BitConverter.ToString(cryptedByte));

            for (int i = 0; i < cryptedByte.Length; i++)
            {
                cryptedByte[i] ^= (byte)rand.Next(255);
            }

            Debug.Log("Cipher:" + BitConverter.ToString(cryptedByte));

            return cryptedByte;
        }
    }
}