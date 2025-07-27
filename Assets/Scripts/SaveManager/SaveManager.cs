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
                    }

                    // ���ׂĂ̐��l���������񂾂�o�C�g�z��ɂ��A�Í������ĕۑ�
                    sw.Write(saveEncryptor.EncryptData(BitConverter.ToString(GetBytesFromList(dataBuffer))));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            Debug.Log("Save Encryption is succeeded");
            return true;
        }

        // �Z�[�u�t�@�C���ǂݍ���(���o�[�W�����̓ǂݍ���)
        public bool LoadOldSaveFile()
        {
            // �Z�[�u��ǂݍ���
            string path = Application.persistentDataPath + "/Nostalgia/save.sv";

            // �t�@�C�����Ȃ��ꍇ�͓ǂݍ��܂Ȃ�
            if (!File.Exists(path))
            {
                Debug.Log("No old save file");
                return false;
            }

            try
            {
                using (FileStream file = File.OpenRead(path))
                {
                    int index = 0;

                    using (BinaryReader br = new BinaryReader(file))
                    using (Stream baseStream = br.BaseStream)
                    {
                        while (baseStream.Position != baseStream.Length)
                        {
                            SetValueFromData(br, index);
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

            // �Â��t�@�C���͏���
            File.Delete(path);

            Debug.Log("Load Done. Old file is deleted");
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
                // �t�@�C���̕�����������
                using (FileStream file = File.OpenRead(path))
                {
                    // �ǂݍ��݈ʒu
                    int index = 0;

                    // �f�[�^
                    string data;
                    // �t�@�C���ǂݍ��݂����ĕ�������
                    using (StreamReader stream = new StreamReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        // ����
                        data = stream.ReadToEnd();
                        data = saveEncryptor.DecryptData(data);
                        Debug.Log("File EOF");
                    }

                    // ��������o�C�g�z��ɖ߂������J�n
                    using (MemoryStream ms = new MemoryStream(GetBytesFromString(data)))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        using (Stream baseStream = br.BaseStream)
                        {
                            while (baseStream.Position != baseStream.Length)
                            {
                                SetValueFromData(br, index);
                                index += 1;
                            }
                            Debug.Log("Binary EOF");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Load failed");
                throw new Exception(e.ToString());
            }

            Debug.Log("Load with Decryption done");
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
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

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
        private void SetValueFromData(BinaryReader br, int addressID)
        {
            try
            {
                switch (addressID)
                {
                    case (int)AddressID.Setting:
                        CurrentSave.RecordSlotSetting(br.ReadInt32());

                        break;

                    case (int)AddressID.Player:
                        CurrentSave.Player.LoadData(br);
                        break;

                    case (int)AddressID.Medal:
                        CurrentSave.Medal.LoadData(br);
                        break;

                    case (int)AddressID.FlagC:
                        CurrentSave.RecordFlagCounter(br.ReadInt32());
                        break;

                    case (int)AddressID.Reel:
                        // ��
                        CurrentSave.LastReelPos[(int)ReelLeft] = br.ReadInt32();
                        // ��
                        CurrentSave.LastReelPos[(int)ReelMiddle] = br.ReadInt32();
                        // �E
                        CurrentSave.LastReelPos[(int)ReelRight] = br.ReadInt32();
                        break;

                    // �{�[�i�X���
                    case (int)AddressID.Bonus:
                        CurrentSave.Bonus.LoadData(br);
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

        // �����񂩂�o�C�g�z����쐬
        private byte[] GetBytesFromString(string byteString)
        {
            List<byte> bytes = new List<byte>();
            string[] buffer = byteString.Split("-");
            
            foreach(string s in buffer)
            {
                bytes.Add(Convert.ToByte(s, 16));
            }

            return bytes.ToArray();
        }
    }
}
