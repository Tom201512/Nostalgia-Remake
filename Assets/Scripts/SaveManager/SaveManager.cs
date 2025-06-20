using System.IO;
using UnityEngine;
using ReelSpinGame_Util;
using System;

public class SaveManager
{
    // �Z�[�u�@�\

    // const

    // var

    // �Z�[�u�f�[�^
    public class SaveDatabase
    {
        // �V�X�e��

        // ��ݒ�
        public int Setting { get; private set; }

        // �v���C���[���

        // ���_�����
    }

    // �R���X�g���N�^
    public SaveManager()
    {

    }

    // func

    // �Z�[�u�t�H���_�쐬
    public bool GenerateSaveFolder()
    {
        string path = Application.persistentDataPath + "/Nostalgia";

        // �t�H���_�����邩�m�F
        if(Directory.Exists(path))
        {
            return false;
        }

        // �Ȃ��ꍇ�͍쐬
        Directory.CreateDirectory(path);
        Debug.Log("Directory is created");
        return true;
    }

    // �Z�[�u�t�@�C���쐬
    public bool GenerateSaveFile(int value)
    {
        string path = Application.persistentDataPath + "/Nostalgia/save.sv";
        
        try
        {
            using (FileStream file = File.OpenWrite(path))
            {
                Debug.Log("SaveFile is Open");
                // �����ɃZ�[�u�t�@�C������������
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

    // �Z�[�u�t�@�C���ǂݍ���
    public bool LoadSaveFile()
    {
        string path = Application.persistentDataPath + "/Nostalgia/save.sv";

        // �t�@�C�����Ȃ��ꍇ�͓ǂݍ��܂Ȃ�
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
                    // int�^�ɕϊ�
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
