using ReelSpinGame_System;
using System;
using System.Collections.Generic;
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
        // AES 暗号鍵
        private const string AES_Key = "fzdqUCx36u7ctl9njUUSo14Ocvg5wh70";

        // func

        // 暗号化

        // int型Listの暗号化
        public string EncryptData(List<int> intLists)
        {
            // 暗号化されたバイト配列
            byte[] encryptedBytes;

            // 暗号文
            string encryptedString = "";

            using (Aes aes = Aes.Create())
            {
                // 暗号化作成

                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.GenerateIV();

                using ICryptoTransform encryptor =
                    aes.CreateEncryptor(Encoding.UTF8.GetBytes(AES_Key), Encoding.UTF8.GetBytes(AES_IV));

                // 整数型リストをbyte配列へ変換
                encryptedBytes = SaveManager.GetBytesFromList(intLists);
                // 文字に変換

                encryptedString = Convert.ToBase64String(encryptedBytes);
                Debug.Log("Data:" + encryptedString);

                return (encryptedString);
            }
        }

        // 復号

        // 暗号化されたバイト配列を読み込む
        public byte[] DecryptData(byte[] encryptedBytes)
        {
            // 復号されたバイト配列
            List<byte> decryptedBytes = new List<byte>();

            using (Aes aes = Aes.Create())
            {
                // 復号作成

                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.GenerateIV();

                using ICryptoTransform decryptor =
                    aes.CreateDecryptor(Encoding.UTF8.GetBytes(AES_Key), Encoding.UTF8.GetBytes(AES_IV));

                // 出力用のストリーム
                using MemoryStream inStream = new MemoryStream(encryptedBytes);

                // 復号する
                using CryptoStream cStream = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read);
                using BinaryReader bReader = new BinaryReader(cStream);
                Stream baseStream = bReader.BaseStream;

                // 復号用のbyteリスト
                List<byte> decriptingBytes = new List<byte>();

                while (baseStream.Position != baseStream.Length)
                {
                    decryptedBytes.Add(bReader.ReadByte());
                }
            }

            // デバッグ用
            string buffer = BitConverter.ToString(decryptedBytes.ToArray());
            Debug.Log("Data:" + buffer);

            return decryptedBytes.ToArray();
        }
    }
}