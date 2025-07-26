using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;
using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;

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

        // �Í����@�\
        private SaveEncryptor saveEncryptor;

        // �R���X�g���N�^
        public SaveManager()
        {
            CurrentSave = new SaveDatabase();
            saveEncryptor = new SaveEncryptor();
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

            // �Í����@�\�̏�����
            saveEncryptor.GenerateCryptBytes();

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
                    // �Z�[�u�t�@�C�����쐬���邽�߂̐����^List�쐬
                    List<int> dataBuffer = new List<int>();

                    // ��ݒ�
                    dataBuffer.Add(CurrentSave.Setting);
                    //file.Write(BitConverter.GetBytes(CurrentSave.Setting));

                    // �v���C���[�f�[�^
                    foreach(int i in CurrentSave.Player.SaveData())
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.Player.SaveData()));

                    // ���_���f�[�^
                    foreach (int i in CurrentSave.Medal.SaveData())
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.Medal.SaveData()));

                    // �t���O�J�E���^
                    dataBuffer.Add(CurrentSave.FlagCounter);
                    //file.Write(BitConverter.GetBytes(CurrentSave.FlagCounter));

                    //���[����~�ʒu
                    //Debug.Log("ReelPos");
                    foreach (int i in CurrentSave.LastReelPos)
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.LastReelPos));

                    // �{�[�i�X���
                    //Debug.Log("Bonus");
                    foreach (int i in CurrentSave.Bonus.SaveData())
                    {
                        dataBuffer.Add(i);
                    }
                    //file.Write(GetBytesFromList(CurrentSave.Bonus.SaveData()));

                    // ���ׂĂ̐��l���������񂾂�Í���

                    // ��������
                    WriteSave(file, dataBuffer);
                }
            }
            catch (Exception e)
            {
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
            // ���ݓ����A�������Z�[�u����ǂݍ���

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
                    //Debug.Log("EOF");
                }
            }
            catch (Exception e)
            {
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
                throw new Exception(e.ToString());
            }
        }

        // �Z�[�u�f�[�^��������
        private void WriteSave(FileStream file, List<int> data)
        {
            file.Write(GetBytesFromList(data));
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
                        //Debug.Log("Setting:" + CurrentSave.Setting);

                        break;

                    case (int)AddressID.Player:
                        //Debug.Log("Player");
                        CurrentSave.Player.LoadData(bStream);
                        break;

                    case (int)AddressID.Medal:
                        //Debug.Log("Medal");
                        CurrentSave.Medal.LoadData(bStream);
                        break;

                    case (int)AddressID.FlagC:
                        //Debug.Log("FlagCounter");
                        CurrentSave.RecordFlagCounter(bStream.ReadInt32());
                        //Debug.Log("FlagCounter Loaded");
                        break;

                    case (int)AddressID.Reel:
                        //Debug.Log("Reel");

                        // ��
                        CurrentSave.LastReelPos[(int)ReelLeft] = bStream.ReadInt32();
                        //Debug.Log("ReelL:" + CurrentSave.LastReelPos[(int)ReelLeft]);

                        // ��
                        CurrentSave.LastReelPos[(int)ReelMiddle] = bStream.ReadInt32();
                        //Debug.Log("ReelM:" + CurrentSave.LastReelPos[(int)ReelMiddle]);

                        // �E
                        CurrentSave.LastReelPos[(int)ReelRight] = bStream.ReadInt32();
                        //Debug.Log("ReelR:" + CurrentSave.LastReelPos[(int)ReelRight]);

                        //Debug.Log("Reel Loaded");
                        break;

                    // �{�[�i�X���
                    case (int)AddressID.Bonus:
                        //Debug.Log("Bonus");
                        CurrentSave.Bonus.LoadData(bStream);
                        break;
                }
            }
            catch(Exception e)
            {
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

            // int�^List���琔�l�����
            foreach (int i in lists)
            {
                // �S�Ă̐��l��byte�ϊ�
                foreach(byte b in BitConverter.GetBytes(i))
                {
                    bytes.Add(b);
                }
            }

            return bytes.ToArray();
        }

        // �`�F�b�N�T���쐬
        private int MakeCheckSum(int dataNum) => dataNum & 0xFF;
    }
}
