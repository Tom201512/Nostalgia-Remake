using ReelSpinGame_Save.Database;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_System
{
    // オプションのセーブマネージャー
    public class OptionSaveManager : ISaveManage
    {
        // const

        // var
        // 現在のセーブデータ
        public OptionSave CurrentSave { get; private set; }

        // コンストラクタ
        public OptionSaveManager()
        {
            CurrentSave = new OptionSave();
        }

        // func

        // バッファ作成
        public List<int> GenerateDataBuffer()
        {
            // セーブファイルを作成するための整数型List作成
            List<int> dataBuffer = new List<int>();

            foreach (int i in CurrentSave.SaveData())
            {
                dataBuffer.Add(i);
            }

            // ハッシュ値書き込み
            // 文字列にしてハッシュコードにする
            int hash = BitConverter.ToString(SaveManager.GetBytesFromList(dataBuffer)).GetHashCode();
            dataBuffer.Add(hash);

            return dataBuffer;
        }

        // バッファ読み込み
        public bool LoadDataBuffer(Stream baseStream, BinaryReader br)
        {
            string path = Application.persistentDataPath + "/Nostalgia/core.sav";

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                Debug.LogError("File not found");
                return false;
            }

            try
            {
                // ハッシュ値以外を読み込む
                while (baseStream.Position != baseStream.Length - sizeof(int))
                {
                    if (!SetValueFromData(br))
                    {
                        Debug.LogError("Failed to Load data");
                        return false;
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

        // データセット
        private bool SetValueFromData(BinaryReader br)
        {
            try
            {
                CurrentSave.LoadData(br);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Loading error happened");
                return false;
            }

            return true;
        }
    }
}