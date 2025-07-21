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
        // �Z�[�u�@�\

        // const
        // �A�h���X�Ԓn
        private enum AddressID { Setting, Player, Medal, Reel, Bonus}

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
            public PlayingDatabase Player { get; private set; }

            // ���_�����
            public MedalSystemSave Medal { get; private set; }

            // �Ō�Ɏ~�܂������[���ʒu
            public List<int> LastReelPos {  get; private set; }

            public SaveDatabase()
            {
                Setting = 6;
                Player = new PlayingDatabase();
                Medal = new MedalSystemSave();
                LastReelPos = new List<int> { 19, 19, 19 };
            }

            // func

            // ��ݒ�ݒ�
            public void SetSetting(int setting) => Setting = setting;

            // ���[���ʒu�ݒ�
            public void SetReelPos(List<int> lastStopped) => LastReelPos = lastStopped;
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
                    // ��ݒ�
                    file.Write(BitConverter.GetBytes(CurrentSave.Setting));

                    // �v���C���[�f�[�^
                    byte[] test = GetBytesFromList(CurrentSave.Player.SaveData());
                    file.Write(test);

                    // ���_���f�[�^
                    test = GetBytesFromList(CurrentSave.Medal.SaveData());
                    file.Write(test);

                    // ���[����~�ʒu
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

        // �f�[�^�Ԓn���Ƃɐ������Z�b�g
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

                    // ��
                    CurrentSave.LastReelPos[(int)ReelLeft] = bStream.ReadInt32();
                    Debug.Log("ReelL:" + CurrentSave.LastReelPos[(int)ReelLeft]);

                    // ��
                    CurrentSave.LastReelPos[(int)ReelMiddle] = bStream.ReadInt32();
                    Debug.Log("ReelM:" + CurrentSave.LastReelPos[(int)ReelMiddle]);

                    // �E
                    CurrentSave.LastReelPos[(int)ReelRight] = bStream.ReadInt32();
                    Debug.Log("ReelR:" + CurrentSave.LastReelPos[(int)ReelRight]);

                    break;
            }
        }

        // int�^List����Byte�z��𓾂�
        private byte[] GetBytesFromList(List<int> lists)
        {
            List<byte> bytes = new List<byte>();

            Debug.Log("Byte Data Gen Start");

            // int�^List���琔�l�����
            foreach (int i in lists)
            {
                Debug.Log("Int:" + i);
                // �S�Ă̐��l��byte�ϊ�
                foreach(byte b in BitConverter.GetBytes(i))
                {
                    bytes.Add(b);
                }

                // �f�o�b�O�p�A�A�h���X�\��
                string[] datas = BitConverter.ToString(BitConverter.GetBytes(i)).Split("-");
                foreach (string d in datas)
                {
                    Debug.Log(d);
                }
            }

            return bytes.ToArray();
        }

        // �`�F�b�N�T���𓾂�
        public int GetCheckSum(int dataNum) => dataNum & 0xFF;
    }
}
