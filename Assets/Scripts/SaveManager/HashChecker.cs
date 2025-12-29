using ReelSpinGame_Save.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Save.Encryption
{
    // ハッシュ値チェック
    public class HashChecker
    {
        // ハッシュ値チェック
        public bool CheckHash(Stream baseStream, BinaryReader br)
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
                int newHash = BitConverter.ToString(ByteArrayUtil.GetBytesFromList(intData)).GetHashCode();

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
    }
}