using ReelSpinGame_Interface;
using ReelSpinGame_Save.Bonus;
using ReelSpinGame_Save.Medal;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;

namespace ReelSpinGame_System
{
    public class SaveManager
    {
        // �Z�[�u�@�\

        // const
        // �A�h���X�Ԓn
        private enum AddressID { Setting, Player, Medal, FlagC, Reel, Bonus}

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
            public PlayerSave Player { get; private set; }

            // ���_�����
            public MedalSave Medal { get; private set; }

            // �t���O�J�E���^���l
            public int FlagCounter { get; private set; }

            // �Ō�Ɏ~�܂������[���ʒu
            public List<int> LastReelPos {  get; private set; }

            // �{�[�i�X���
            public BonusSave Bonus { get; private set; }

            public SaveDatabase()
            {
                Setting = 6;
                Player = new PlayerSave();
                Medal = new MedalSave();
                FlagCounter = 0;
                LastReelPos = new List<int> { 19, 19, 19 };
                Bonus = new BonusSave();
            }

            // func

            // �e����L�^
            // ��ݒ�
            public void RecordSlotSetting(int setting) => Setting = setting;

            // �v���C���[���
            public void RecordSaveData(ISavable player)
            {
                if (player.GetType() == typeof(PlayerSave))
                {
                    Player = player as PlayerSave;
                }
                else
                {
                    throw new Exception("Save data is not PlayerSave");
                }
            }

            // ���_�����
            public void RecordMedalSave(ISavable medal)
            {
                if(medal.GetType() == typeof(MedalSave))
                {
                    Medal = medal as MedalSave;
                }
                else
                {
                    throw new Exception("Save data is not MedalData");
                }
            }
            
            // �t���O�J�E���^
            public void RecordFlagCounter(int flagCounter) => FlagCounter = flagCounter;

            // ���[���ʒu
            public void RecordReelPos(List<int> lastStopped) => LastReelPos = lastStopped;

            // �{�[�i�X���
            public void RecordBonusData(ISavable bonus)
            {
                {
                    if (bonus.GetType() == typeof(BonusSave))
                    {
                        Bonus = bonus as BonusSave;
                    }
                    else
                    {
                        throw new Exception("Save data is not BonusSave");
                    }
                }
            }
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

            // �O�̃Z�[�u������
            if (Directory.Exists(path))
            {
                Debug.Log("Overwrite file");
                File.Delete(path);
            }

            try
            {
                using (FileStream file = File.OpenWrite(path))
                {
                    // �����ɃZ�[�u�t�@�C������������
                    // ��ݒ�
                    file.Write(BitConverter.GetBytes(CurrentSave.Setting));

                    // �f�o�b�O�p
                    Debug.Log("Setting:");
                    foreach (string d in BitConverter.ToString(BitConverter.GetBytes(CurrentSave.Setting)).Split("-"))
                    {
                        Debug.Log(d);
                    }

                    // �v���C���[�f�[�^
                    file.Write(GetBytesFromList(CurrentSave.Player.SaveData()));

                    // ���_���f�[�^
                    file.Write(GetBytesFromList(CurrentSave.Medal.SaveData()));

                    // �t���O�J�E���^
                    file.Write(BitConverter.GetBytes(CurrentSave.FlagCounter));

                    // �f�o�b�O�p
                    Debug.Log("FlagCounter:");
                    foreach (string d in BitConverter.ToString(BitConverter.GetBytes(CurrentSave.FlagCounter)).Split("-"))
                    {
                        Debug.Log(d);
                    }

                    //���[����~�ʒu
                    Debug.Log("ReelPos");
                    file.Write(GetBytesFromList(CurrentSave.LastReelPos));

                    // �{�[�i�X���
                    Debug.Log("Bonus");
                    file.Write(GetBytesFromList(CurrentSave.Bonus.SaveData()));
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                Application.Quit();
                throw new Exception(e.ToString());
            }
            finally
            {
                Debug.Log("Save is succeeded");
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
                Debug.Log(e.ToString());
                Application.Quit();
                throw new Exception(e.ToString());
            }

            return true;
        }

        // �Z�[�u�폜
        public void DeleteSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                Application.Quit();
                throw new Exception(e.ToString());
            }
        }

        // �f�[�^�Ԓn���Ƃɐ������Z�b�g
        private void SetValueFromData(BinaryReader bStream, int addressID)
        {
            try
            {
                switch (addressID)
                {
                    case (int)AddressID.Setting:
                        CurrentSave.RecordSlotSetting(bStream.ReadInt32());
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

                    case (int)AddressID.FlagC:
                        Debug.Log("FlagCounter");
                        CurrentSave.RecordFlagCounter(bStream.ReadInt32());
                        Debug.Log("FlagCounter Loaded");
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

                        Debug.Log("Reel Loaded");
                        break;

                    // �{�[�i�X���
                    case (int)AddressID.Bonus:
                        Debug.Log("Bonus");
                        CurrentSave.Bonus.LoadData(bStream);
                        break;
                }
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
                Application.Quit();
                throw new Exception(e.ToString());
            }
            finally
            {

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
