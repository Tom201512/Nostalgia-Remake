using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;

namespace ReelSpinGame_System
{
    public class SaveManager
    {
        // セーブ機能

        // const
        // アドレス番地
        private enum AddressID { Setting, Player, Medal, Reel, Bonus}

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
            public PlayingDatabase Player { get; private set; }

            // メダル情報
            public MedalSystemSave Medal { get; private set; }

            // 最後に止まったリール位置
            public List<int> LastReelPos {  get; private set; }

            public SaveDatabase()
            {
                Setting = 6;
                Player = new PlayingDatabase();
                Medal = new MedalSystemSave();
                LastReelPos = new List<int> { 19, 19, 19 };
            }

            // func

            // 台設定設定
            public void SetSetting(int setting) => Setting = setting;

            // リール位置設定
            public void SetReelPos(List<int> lastStopped) => LastReelPos = lastStopped;
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
                    // 台設定
                    file.Write(BitConverter.GetBytes(CurrentSave.Setting));

                    // プレイヤーデータ
                    byte[] test = GetBytesFromList(CurrentSave.Player.SaveData());
                    file.Write(test);

                    // メダルデータ
                    test = GetBytesFromList(CurrentSave.Medal.SaveData());
                    file.Write(test);

                    // リール停止位置
                    test = GetBytesFromList(CurrentSave.LastReelPos);
                    file.Write(test);
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
                    int index = 0;
                    BinaryReader stream = new BinaryReader(file);
                    Stream baseStream = stream.BaseStream;

                    while (baseStream.Position != baseStream.Length)
                    {
                        SetValueFromData(stream, index);
                        index += 1;
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

        // データ番地ごとに数字をセット
        private void SetValueFromData(BinaryReader bStream, int addressID)
        {
            switch(addressID)
            {
                case (int)AddressID.Setting:
                    CurrentSave.SetSetting(bStream.ReadInt32());
                    Debug.Log("Setting:" + CurrentSave.Setting);

                    break;

                case (int)AddressID.Player:
                    Debug.Log("Player");
                    CurrentSave.Player.LoadData(bStream);
                    break;

                case (int)AddressID.Medal:
                    Debug.Log("Medal");
                    CurrentSave.Medal.LoadData(bStream);
                    break;

                case (int)AddressID.Reel:
                    Debug.Log("Reel");

                    // 左
                    CurrentSave.LastReelPos[(int)ReelLeft] = bStream.ReadInt32();
                    Debug.Log("ReelL:" + CurrentSave.LastReelPos[(int)ReelLeft]);

                    // 中
                    CurrentSave.LastReelPos[(int)ReelMiddle] = bStream.ReadInt32();
                    Debug.Log("ReelM:" + CurrentSave.LastReelPos[(int)ReelMiddle]);

                    // 右
                    CurrentSave.LastReelPos[(int)ReelRight] = bStream.ReadInt32();
                    Debug.Log("ReelR:" + CurrentSave.LastReelPos[(int)ReelRight]);

                    break;
            }
        }

        // int型ListからByte配列を得る
        private byte[] GetBytesFromList(List<int> lists)
        {
            List<byte> bytes = new List<byte>();

            Debug.Log("Byte Data Gen Start");

            // int型Listから数値を取る
            foreach (int i in lists)
            {
                Debug.Log("Int:" + i);
                // 全ての数値をbyte変換
                foreach(byte b in BitConverter.GetBytes(i))
                {
                    bytes.Add(b);
                }

                // デバッグ用、アドレス表示
                string[] datas = BitConverter.ToString(BitConverter.GetBytes(i)).Split("-");
                foreach (string d in datas)
                {
                    Debug.Log(d);
                }
            }

            return bytes.ToArray();
        }

        // チェックサムを得る
        public int GetCheckSum(int dataNum) => dataNum & 0xFF;
    }
}
