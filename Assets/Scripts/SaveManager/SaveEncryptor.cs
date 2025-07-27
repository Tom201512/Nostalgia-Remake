using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

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

        // func

        // セーブデータ(byte配列)の暗号化
        public string EncryptData(string plainText)
        {
            Debug.Log("Plain:" + plainText);

            using (Aes aes = Aes.Create())
            {
                // 暗号化作成

                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.IV = Encoding.UTF8.GetBytes(AES_IV);
                aes.Key = Encoding.UTF8.GetBytes(AES_Key);

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

                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.IV = Encoding.UTF8.GetBytes(AES_IV);
                aes.Key = Encoding.UTF8.GetBytes(AES_Key);

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
    }
}