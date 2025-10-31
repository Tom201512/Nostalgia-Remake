using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;
using static ReelSpinGame_Reels.ReelManagerModel.ReelID;

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
            try
            {
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            //Debug.Log("Directory is created");
            return true;
        }

        // �Z�[�u�t�@�C���쐬
        public bool GenerateSaveFileWithEncrypt()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

            // �O�̃Z�[�u������
            if (Directory.Exists(path))
            {
                DeleteSaveFile();
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
                    Debug.Log("Setting:" + CurrentSave.Setting);

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
                    Debug.Log("FlagCounter:" + CurrentSave.FlagCounter);

                    //���[����~�ʒu
                    foreach (int i in CurrentSave.LastReelPos)
                    {
                        dataBuffer.Add(i);
                    }

                    Debug.Log("ReelPos:" + CurrentSave.LastReelPos[(int)ReelLeft] + "," +
                        CurrentSave.LastReelPos[(int)ReelMiddle] + "," + CurrentSave.LastReelPos[(int)ReelRight]);

                    // �{�[�i�X���
                    foreach (int i in CurrentSave.Bonus.SaveData())
                    {
                        dataBuffer.Add(i);
                    }

                    // �n�b�V���l��������
                    //Debug.Log("ListLength:" +  dataBuffer.Count);

                    // ������ɂ��ăn�b�V���R�[�h�ɂ���
                    int hash = BitConverter.ToString(GetBytesFromList(dataBuffer)).GetHashCode();
                    //Debug.Log("Hash:" + hash);
                    //Debug.Log("HashBytes:" + BitConverter.ToString(BitConverter.GetBytes(hash)));

                    dataBuffer.Add(hash);

                    // ���ׂĂ̐��l���������񂾂�o�C�g�z��ɂ��A�Í������ĕۑ�
                    sw.Write(saveEncryptor.EncryptData(BitConverter.ToString(GetBytesFromList(dataBuffer))));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            Debug.Log("Save Encryption is succeeded");
            return true;
        }

        // �Z�[�u�ǂݍ���
        public bool LoadSaveFileWithDecryption()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";

            // �t�@�C�����Ȃ��ꍇ�͓ǂݍ��܂Ȃ�
            if (!File.Exists(path))
            {
                Debug.LogError("File not found");
                return false;
            }

            try
            {
                // �t�@�C���̕�����������
                using (FileStream file = File.OpenRead(path))
                {
                    // �f�[�^�ǂݍ��݈ʒu
                    int index = 0;

                    // �f�[�^
                    string data;
                    // �t�@�C���ǂݍ��݂����ĕ���
                    using (StreamReader stream = new StreamReader(file))
                    using (Stream baseStream = stream.BaseStream)
                    {
                        // ����
                        data = stream.ReadToEnd();
                        data = saveEncryptor.DecryptData(data);
                        //Debug.Log("File EOF");
                    }

                    // ��������o�C�g�z��ɖ߂������J�n�B�n�b�V���l�Q�Ƃ��s��
                    using (MemoryStream ms = new MemoryStream(GetBytesFromString(data)))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        using (Stream baseStream = br.BaseStream)
                        {
                            // �n�b�V���l�̎Q��
                            baseStream.Seek(-4, SeekOrigin.End);
                            int previousHash = br.ReadInt32();


                            // �n�b�V���l�ȊO��ǂݍ��݃��X�g�t�@�C�������
                            List<int> intData = new List<int>();
                            baseStream.Seek(0, SeekOrigin.Begin);

                            while (baseStream.Position != baseStream.Length - sizeof(int))
                            {
                                intData.Add(br.ReadInt32());
                            }

                            // �V������������X�g�̃n�b�V���l����v���邩�`�F�b�N����
                            int newHash = BitConverter.ToString(GetBytesFromList(intData)).GetHashCode();

                            // �n�b�V���l������Ȃ��ꍇ�͓ǂݍ��܂Ȃ�
                            if (previousHash != newHash)
                            {
                                Debug.LogError("Hash code is wrong");
                                return false;
                            }

                            Debug.Log("Hash is correct");

                            baseStream.Position = 0;

                            // �n�b�V���l�ȊO��ǂݍ���
                            while (baseStream.Position != baseStream.Length - sizeof(int))
                            {
                                if(SetValueFromData(br, index))
                                {
                                    index += 1;
                                }
                                else
                                {
                                    Debug.LogError("Failed to Load data at:" + index);
                                    return false;
                                }
                            }
                            //Debug.Log("Binary EOF");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Load failed");
                return false;
            }

            Debug.Log("Load with Decryption is done");
            return true;
        }

        // �Z�[�u�폜
        public bool DeleteSaveFile()
        {
            string path = Application.persistentDataPath + "/Nostalgia/save.sav";
            string keyPath = Application.persistentDataPath + "/Nostalgia/save.key";
            try
            {
                File.Delete(path);
                File.Delete(keyPath);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            Debug.Log("Save deletion succeeded");
            return true;
        }

        // �f�[�^�Ԓn���Ƃɐ������Z�b�g
        private bool SetValueFromData(BinaryReader br, int addressID)
        {
            try
            {
                switch (addressID)
                {
                    case (int)AddressID.Setting:
                        CurrentSave.RecordSlotSetting(br.ReadInt32());
                        Debug.Log("Setting:" + CurrentSave.Setting);
                        break;

                    case (int)AddressID.Player:
                        CurrentSave.Player.LoadData(br);
                        break;

                    case (int)AddressID.Medal:
                        CurrentSave.Medal.LoadData(br);
                        break;

                    case (int)AddressID.FlagC:
                        CurrentSave.RecordFlagCounter(br.ReadInt32());
                        Debug.Log("FlagCounter:" + CurrentSave.FlagCounter);
                        break;

                    case (int)AddressID.Reel:
                        // ��
                        CurrentSave.LastReelPos[(int)ReelLeft] = br.ReadInt32();
                        Debug.Log("Left:" + CurrentSave.LastReelPos[(int)ReelLeft]);
                        // ��
                        CurrentSave.LastReelPos[(int)ReelMiddle] = br.ReadInt32();
                        Debug.Log("Middle:" + CurrentSave.LastReelPos[(int)ReelMiddle]);
                        // �E
                        CurrentSave.LastReelPos[(int)ReelRight] = br.ReadInt32();
                        Debug.Log("Right:" + CurrentSave.LastReelPos[(int)ReelRight]);
                        break;

                    // �{�[�i�X���
                    case (int)AddressID.Bonus:
                        CurrentSave.Bonus.LoadData(br);
                        break;
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Loading error happened at:" + (AddressID)Enum.ToObject(typeof(ReelSymbols), addressID));
                return false;
            }

            return true;
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
