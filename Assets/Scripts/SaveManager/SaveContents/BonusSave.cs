using ReelSpinGame_Bonus;
using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Save.Bonus
{
    // �{�[�i�X���̃Z�[�u
    public class BonusSave : ISavable
    {
        // var
        // ���݃X�g�b�N���Ă���{�[�i�X
        public BonusType HoldingBonusID { get; set; }
        // �{�[�i�X���
        public BonusStatus CurrentBonusStatus { get; set; }
        // BIG�{�[�i�X���I���̐F
        public BigColor BigChanceColor { get; set; }

        // �����Q�[����
        // �c�菬���Q�[����
        public int RemainingBigGames { get; set; }
        // �c��JACIN
        public int RemainingJacIn { get; set; }

        // JAC�Q�[����
        // �c��JAC�Q�[����
        public int RemainingJacGames { get; set; }
        // �c�蓖�I��
        public int RemainingJacHits { get; set; }

        // ���̃{�[�i�X�ł̊l������
        public int CurrentBonusPayouts { get; set; }
        // �A�`������Ԓ��̃{�[�i�X����
        public int CurrentZonePayouts { get; set; }
        // �A�`������Ԃɂ��邩
        public bool HasZone { get; set; }

        public BonusSave()
        {
            HoldingBonusID = BonusType.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigChanceColor = BigColor.None;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
            CurrentBonusPayouts = 0;
            CurrentZonePayouts = 0;
            HasZone = false;
        }

        // �f�[�^�L�^
        public void RecordData(BonusSystemData bonus)
        {
            HoldingBonusID = bonus.HoldingBonusID;
            CurrentBonusStatus = bonus.CurrentBonusStatus;
            BigChanceColor = bonus.BigChanceColor;
            RemainingBigGames = bonus.RemainingBigGames;
            RemainingJacIn = bonus.RemainingJacIn;
            RemainingJacHits = bonus.RemainingJacHits;
            RemainingJacGames = bonus.RemainingJacGames;
            CurrentBonusPayouts = bonus.CurrentBonusPayouts;
            CurrentZonePayouts = bonus.CurrentZonePayouts;
            HasZone = bonus.HasZone;
        }

        // �Z�[�u
        public List<int> SaveData()
        {
            // �ϐ����i�[
            List<int> data = new List<int>();

            data.Add((int)HoldingBonusID);
            data.Add((int)CurrentBonusStatus);
            data.Add((int)BigChanceColor);
            data.Add(RemainingBigGames);
            data.Add(RemainingJacIn);
            data.Add(RemainingJacHits);
            data.Add(RemainingJacGames);
            data.Add(CurrentBonusPayouts);
            data.Add(CurrentZonePayouts);
            data.Add(HasZone ? 1 : 0);

            // �f�o�b�O�p
            Debug.Log("BonusData:");
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
                // �X�g�b�N���̃{�[�i�X
                HoldingBonusID = (BonusType)Enum.ToObject(typeof(BonusType), bStream.ReadInt32());
                //Debug.Log("HoldingBonusID:" + HoldingBonusID);
                // ���݂̃{�[�i�X���
                CurrentBonusStatus = (BonusStatus)Enum.ToObject(typeof(BonusStatus), bStream.ReadInt32());
                //Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
                // �r�b�O�`�����X���̐F
                BigChanceColor = (BigColor)Enum.ToObject(typeof(BigColor), bStream.ReadInt32());
                //Debug.Log("BigChanceColor:" + BigChanceColor);
                // �c��BIG�Q�[����
                RemainingBigGames = bStream.ReadInt32();
                //Debug.Log("RemainingBigGames:" + RemainingBigGames);
                // �c��JACIN
                RemainingJacIn = bStream.ReadInt32();
                //Debug.Log("RemainingJacIn:" + RemainingJacIn);
                // �c��JAC�Q�[�������I��
                RemainingJacHits = bStream.ReadInt32();
                //Debug.Log("RemainingJacHits:" + RemainingJacHits);
                // �c��JAC�Q�[����
                RemainingJacGames = bStream.ReadInt32();
                //Debug.Log("RemainingJacGames:" + RemainingJacGames);
                // ���݂̃{�[�i�X�l������
                CurrentBonusPayouts = bStream.ReadInt32();
                //Debug.Log("CurrentBonusPayouts:" + CurrentBonusPayouts);
                // �A�`������Ԓ��̃{�[�i�X����
                CurrentZonePayouts = bStream.ReadInt32();
                //Debug.Log("CurrentZonePayouts:" + CurrentZonePayouts);
                // �A�`������Ԓ���
                HasZone = (bStream.ReadInt32() == 1 ? true : false);
                //Debug.Log("HasZone:" + HasZone);

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //Debug.Log("BonusData end");
            }

            return true;
        }
    }
}

