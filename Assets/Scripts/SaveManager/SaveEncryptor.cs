using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Save.Encryption
{
    // 暗号化機能
    public class SaveEncryptor
    {
        // const
        // ファイル末端からシード値のある場所
        private const int DateSeedPosFromLast = 5;
        
        // var
        // 日時のシード値
        public int DateSeed { get; private set; }

        // 暗号化のバイト配列
        private static byte[] cryptBytes;

        public SaveEncryptor()
        {
            DateSeed = 0;
            cryptBytes = new byte[] { 0, 0, 0, 0 };
        }

        // func

        // 暗号化

        // 暗号化バイト数値作成
        public void GenerateCryptBytes()
        {
            // シード値を割り当てる
            DateSeed = GetDateSeed();
            GenerateCryptByte(DateSeed);
        }

        // int型Listの暗号化
        public byte[] EncryptData(List<int> intLists)
        {
            // 暗号化したバイト配列
            List<byte> encryptedBytes = new List<byte>();

            // 各数値の暗号化
            foreach (int value in intLists)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                for (int i = 0; i < bytes.Length; i++)
                {
                    encryptedBytes.Add(bytes[i] ^= cryptBytes[i]);
                }
            }

            return encryptedBytes.ToArray();
        }

        // 暗号化バイトの生成
        private void GenerateCryptByte(int dateSeed)
        {
            System.Random rand = new System.Random(dateSeed);

            // シード値より乱数を得る
            for (int i = 0; i < sizeof(int); i++)
            {
                cryptBytes[i] = (byte)rand.Next(0xFF);
                Debug.Log("Byte:" + cryptBytes[i]);
            }
        }

        // 復号化

        // 暗号化数値の読み込み(読み込みの前に最初にすること)
        public void LoadCryptData(BinaryReader bStream)
        {

        }

        // 整数値の復号化
        public static int DecryptIntData(int value)
        {
            // 復号化された時のバイト
            byte[] decryptedBytes = BitConverter.GetBytes(value);

            // 復号化
            for (int i = 0; i < decryptedBytes.Length; i++)
            {
                decryptedBytes[i] ^= cryptBytes[i];
            }

            return BitConverter.ToInt32(decryptedBytes);
        }

        // 現在の日付をシード値にする
        private int GetDateSeed()
        {
            // 日付(年、月、日)と時間(ミリ秒まで)をすべて足し合わせてシード値にする
            return DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day +
                DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
        }
    }
}