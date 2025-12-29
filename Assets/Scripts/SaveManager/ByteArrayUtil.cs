using System;
using System.Collections.Generic;

namespace ReelSpinGame_Save.Util
{
    // バイト配列関連の関数
    public static class ByteArrayUtil
    {
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

            foreach (string s in buffer)
            {
                bytes.Add(Convert.ToByte(s, 16));
            }

            return bytes.ToArray();
        }
    }
}