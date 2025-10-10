using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
            Debug.Log("Credit:" + Credit);
            MaxBetAmount = medal.MaxBetAmount;
            Debug.Log("MaxBetAmount:" + MaxBetAmount);
            LastBetAmount = medal.LastBetAmount;
            Debug.Log("LastBetAmount:" + LastBetAmount);
            HasReplay = medal.HasReplay;
            Debug.Log("HasReplay:" + HasReplay);
        }

        // �Z�[�u
        public List<int> SaveData()
        {
            // �ϐ����i�[
            List<int> data = new List<int>();
            data.Add(Credit);
            Debug.Log("Credit:" + Credit);
            data.Add(MaxBetAmount);
            Debug.Log("MaxBetAmount:" + MaxBetAmount);
            data.Add(LastBetAmount);
            Debug.Log("LastBetAmount:" + LastBetAmount);
            data.Add(HasReplay ? 1 : 0);
            Debug.Log("HasReplay:" + HasReplay);

            return data;
        }

        // �ǂݍ���
        public bool LoadData(BinaryReader br)
        {
            try
            {
                Credit = br.ReadInt32();
                Debug.Log("Credit:" + Credit);
                MaxBetAmount = br.ReadInt32();
                Debug.Log("MaxBetAmount:" + MaxBetAmount);
                LastBetAmount = br.ReadInt32();
                Debug.Log("LastBetAmount:" + LastBetAmount);
                HasReplay = (br.ReadInt32() == 1 ? true : false);
                Debug.Log("HasReplay:" + HasReplay);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }
    }
}

