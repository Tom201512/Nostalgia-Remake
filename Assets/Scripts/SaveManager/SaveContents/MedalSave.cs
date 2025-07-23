using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Save.Medal
{
    // ���_�����̃Z�[�u
    public class MedalSave : ISavable
    {
        // const

        // var
        // �N���W�b�g����
        public int Credits { get; private set; }
        // �ō��x�b�g����
        public int MaxBetAmounts { get; private set; }
        // �Ō�ɂ��������_������
        public int LastBetAmounts { get; private set; }
        // ���v���C��Ԃ�
        public bool HasReplay { get; private set; }

        public MedalSave()
        {
            Credits = 0;
            MaxBetAmounts = MaxBetLimit;
            LastBetAmounts = 0;
            HasReplay = false;
        }

        // func

        // �f�[�^�L�^
        public void RecordData(MedalSystemData medal)
        {
            Credits = medal.Credits;
            MaxBetAmounts = medal.MaxBetAmounts;
            LastBetAmounts = medal.LastBetAmounts;
            HasReplay = medal.HasReplay;
        }

        // �Z�[�u
        public List<int> SaveData()
        {
            // �ϐ����i�[
            List<int> data = new List<int>();
            data.Add(Credits);
            data.Add(MaxBetAmounts);
            data.Add(LastBetAmounts);
            data.Add(HasReplay ? 1 : 0);

            // �f�o�b�O�p
            Debug.Log("MedalData:");
            foreach (int i in data)
            {
                Debug.Log(i);
            }

            return data;
        }

        // �ǂݍ���
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // �N���W�b�g����
                Credits = bStream.ReadInt32();
                //Debug.Log("Credits:" + Credits);

                // �ő�x�b�g����
                MaxBetAmounts = bStream.ReadInt32();
                //Debug.Log("MaxBetAmounts:" + MaxBetAmounts);

                // �Ō�Ɋ|��������
                LastBetAmounts = bStream.ReadInt32();
                //Debug.Log("LastBetAmounts:" + LastBetAmounts);

                // ���v���C���
                HasReplay = (bStream.ReadInt32() == 1 ? true : false);
                //Debug.Log("HasReplay:" + HasReplay);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("MedalSystem Loaded");
            }

            return true;
        }
    }
}

