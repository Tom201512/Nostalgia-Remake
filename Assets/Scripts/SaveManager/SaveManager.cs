using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_System
{
    public class SaveManager
    {
        // �Z�[�u�@�\

        // const

        // var
        // ���݂̃Z�[�u�f�[�^
        public SaveDatabase CurrentSave { get; private set; }

        // �Z�[�u�f�[�^
        public class SaveDatabase
        {
            // �V�X�e��

            // ��ݒ�
            public int Setting { get; private set; }

            // �v���C���[���

            // ���_�����

            public SaveDatabase()
            {
                Setting = 6;
            }

            // func

            // ��ݒ�ݒ�
            public void SetSetting(int setting) => Setting = setting;
        }

        // �R���X�g���N�^
        public SaveManager()
        {
            CurrentSave = new SaveDatabase();
        }

        // func

        // �Z�[�u�t�H���_�쐬
        public bool GenerateSaveFolder()
        {
            string path = Application.persistentDataPath + "/Nostalgia";

            // �t�H���_�����邩�m�F
            if (Directory.Exists(path))
            {
                return false;
            }

            // �Ȃ��ꍇ�͍쐬
            Directory.CreateDirectory(path);
            Debug.Log("Directory is created");
            return true;
        }

        // �Z�[�u�t�@�C���쐬
        public bool GenerateSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            try
            {
                using (FileStream file = File.OpenWrite(path))
                {
                    // �����ɃZ�[�u�t�@�C������������
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
                        int value = stream.ReadInt32();
                        Debug.Log(value);

                        // �C���f�b�N�X���ƂɃf�[�^�̊���U��(��)
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

        // �`�F�b�N�T���𓾂�
        public int GetCheckSum(int dataNum) => dataNum & 0xFF;
    }
}
