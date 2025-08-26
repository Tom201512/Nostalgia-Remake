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
        public int Credit { get; private set; }
        // �ō��x�b�g����
        public int MaxBetAmount { get; private set; }
        // �Ō�ɂ��������_������
        public int LastBetAmount { get; private set; }
        // ���v���C��Ԃ�
        public bool HasReplay { get; private set; }

        public MedalSave()
        {
            Credit = 0;
            MaxBetAmount = MaxBetLimit;
            LastBetAmount = 0;
            HasReplay = false;
        }

        // func

        // �f�[�^�L�^
        public void RecordData(MedalSystemData medal)
        {
            Credit = medal.Credit;
            MaxBetAmount = medal.MaxBetAmount;
            LastBetAmount = medal.LastBetAmount;
            HasReplay = medal.HasReplay;
        }

        // �Z�[�u
        public List<int> SaveData()
        {
            // �ϐ����i�[
            List<int> data = new List<int>();
            data.Add(Credit);
            data.Add(MaxBetAmount);
            data.Add(LastBetAmount);
            data.Add(HasReplay ? 1 : 0);

            // �f�o�b�O�p
            //Debug.Log("MedalData:");
            //foreach (int i in data)
            //{
            //    Debug.Log(i);
            //}

            return data;
        }

        // �ǂݍ���
        public bool LoadData(BinaryReader br)
        {
            try
            {
                // �N���W�b�g����
                Credit = br.ReadInt32();
                //Debug.Log("Credit:" + Credit);

                // �ő�x�b�g����
                MaxBetAmount = br.ReadInt32();
                //Debug.Log("MaxBetAmount:" + MaxBetAmount);

                // �Ō�Ɋ|��������
                LastBetAmount = br.ReadInt32();
                //Debug.Log("LastBetAmount:" + LastBetAmount);

                // ���v���C���
                HasReplay = (br.ReadInt32() == 1 ? true : false);
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

