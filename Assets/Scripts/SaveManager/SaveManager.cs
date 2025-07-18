using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_System
{
    public class SaveManager
    {
        // セーブ機能

        // const

        // var
        // 現在のセーブデータ
        public SaveDatabase CurrentSave { get; private set; }

        // セーブデータ
        public class SaveDatabase
        {
            // システム

            // 台設定
            public int Setting { get; private set; }

            // プレイヤー情報

            // メダル情報

            public SaveDatabase()
            {
                Setting = 6;
            }

            // func

            // 台設定設定
            public void SetSetting(int setting) => Setting = setting;
        }

        // コンストラクタ
        public SaveManager()
        {
            CurrentSave = new SaveDatabase();
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
            Directory.CreateDirectory(path);
            Debug.Log("Directory is created");
            return true;
        }

        // セーブファイル作成
        public bool GenerateSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            try
            {
                using (FileStream file = File.OpenWrite(path))
                {
                    // ここにセーブファイルを書き込む
                    file.Write(BitConverter.GetBytes(CurrentSave.Setting));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                Debug.Log("Save is successed");
            }
            return true;
        }

        // セーブファイル読み込み
        public bool LoadSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            // ファイルがない場合は読み込まない
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                using (FileStream file = File.OpenRead(path))
                {
                    BinaryReader stream = new BinaryReader(file);
                    Stream baseStream = stream.BaseStream;

                    while (baseStream.Position != baseStream.Length)
                    {
                        // int型に変換
                        int value = stream.ReadInt32();
                        Debug.Log(value);

                        // インデックスごとにデータの割り振り(仮)
                        CurrentSave.SetSetting(value);
                    }
                    Debug.Log("EOF");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return true;
        }

        // チェックサムを得る
        public int GetCheckSum(int dataNum) => dataNum & 0xFF;
    }
}
