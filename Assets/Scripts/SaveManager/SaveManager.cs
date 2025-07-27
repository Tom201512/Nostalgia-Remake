using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;
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
            //saveEncryptor.GenerateCryptBytes();

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

                    // �v���C���[�f�[�^
                    foreach(int i in CurrentSave.Player.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // ���_���f�[�^
                    foreach (int i in CurrentSave.Medal.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // �t���O�J�E���^
                    dataBuffer.Add(CurrentSave.FlagCounter);

                    //���[����~�ʒu
                    //Debug.Log("ReelPos");
                    foreach (int i in CurrentSave.LastReelPos)
                    {
                        dataBuffer.Add(i);
                    }

                    // �{�[�i�X���
                    //Debug.Log("Bonus");
                    foreach (int i in CurrentSave.Bonus.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // ��������
                    file.Write(GetBytesFromList(dataBuffer));
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

        // �Z�[�u�t�@�C���쐬(�Í���)
        public bool GenerateSaveFileWithEncrypt()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

            // �O�̃Z�[�u������
            if (Directory.Exists(path))
            {
                Debug.Log("Overwrite file");
                File.Delete(path);
            }

            try
            {
                using (FileStream file = File.OpenWrite(path))
                using (StreamWriter sw = new StreamWriter(file))
                {
                    // �Z�[�u�t�@�C�����쐬���邽�߂̐����^List�쐬
                    List<int> dataBuffer = new List<int>();

                    // ��ݒ�
                    dataBuffer.Add(CurrentSave.Setting);

                    /*
                    // �v���C���[�f�[�^
                    foreach (int i in CurrentSave.Player.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // ���_���f�[�^
                    foreach (int i in CurrentSave.Medal.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // �t���O�J�E���^
                    dataBuffer.Add(CurrentSave.FlagCounter);

                    //���[����~�ʒu
                    foreach (int i in CurrentSave.LastReelPos)
                    {
                        dataBuffer.Add(i);
                    }

                    // �{�[�i�X���
                    foreach (int i in CurrentSave.Bonus.SaveData())
                    {
                        dataBuffer.Add(i);
                    }*/

                    // ���ׂĂ̐��l���������񂾂�Í������ď�������
                    sw.Write(saveEncryptor.EncryptData("TEST"));
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
            // �Z�[�u��ǂݍ���
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

                    using (BinaryReader stream = new BinaryReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        while (baseStream.Position != baseStream.Length)
                        {
                            SetValueFromData(stream, index);
                            index += 1;
                        }

                        Debug.Log("EOF");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return true;
        }

        // �Z�[�u�ǂݍ���(�Í�������)
        public bool LoadSaveFileWithDecryption()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

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
                    using (StreamReader stream = new StreamReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        // ����
                        string data = stream.ReadToEnd();
                        Debug.Log("Cipher:" + data);
                        /*
                        while (baseStream.Position != baseStream.Length)
                        {
                            SetValueFromData(stream, index);
                            index += 1;
                        }*/

                        Debug.Log(saveEncryptor.DecryptData(data));

                        Debug.Log("EOF");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Load failed");
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("Load is succeeded");
            }
            return true;
        }

        // �f�o�b�O�p
        public void DecrpytionTest()
        {
            string data = "Test";
            Debug.Log("Encrypting start");
            data = saveEncryptor.EncryptData(data);
            Debug.Log("Decrypting start");
            data = saveEncryptor.DecryptData(data);
            Debug.Log("Decrypted:" + data);

            data = "t34y2GKpvvBLEIiPrK9/gQ=";
            //Debug.Log("Decrypting start");
            //data = saveEncryptor.DecryptData(data);
            //Debug.Log("Decrypted:" + data);
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
        public static byte[] GetBytesFromList(List<int> lists)
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
    }
}
