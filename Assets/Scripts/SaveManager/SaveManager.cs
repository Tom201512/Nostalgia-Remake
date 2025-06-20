using System.IO;
using UnityEngine;
using ReelSpinGame_Util;
using System;

public class SaveManager
{
    // セーブ機能

    // const

    // var

    // セーブデータ
    public class SaveDatabase
    {
        // システム

        // 台設定
        public int Setting { get; private set; }

        // プレイヤー情報

        // メダル情報
    }

    // コンストラクタ
    public SaveManager()
    {

    }

    // func

    // セーブフォルダ作成
    public bool GenerateSaveFolder()
    {
        string path = Application.persistentDataPath + "/Nostalgia";

        // フォルダがあるか確認
        if(Directory.Exists(path))
        {
            return false;
        }

        // ない場合は作成
        Directory.CreateDirectory(path);
        Debug.Log("Directory is created");
        return true;
    }

    // セーブファイル作成
    public bool GenerateSaveFile(int value)
    {
        string path = Application.persistentDataPath + "/Nostalgia/save.sv";
        
        try
        {
            using (FileStream file = File.OpenWrite(path))
            {
                Debug.Log("SaveFile is Open");
                // ここにセーブファイルを書き込む
                file.Write(BitConverter.GetBytes(value));
                Debug.Log("SaveFile is Closed");
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.ToString());
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
                    Debug.Log(stream.ReadInt32());
                }
                Debug.Log("EOF");
            }
        }
        catch(Exception e)
        {
            throw new Exception(e.ToString());
        }

        return true;
    }
    public int GetCheckSum(int dataNum) => dataNum & 0xFF;
}
