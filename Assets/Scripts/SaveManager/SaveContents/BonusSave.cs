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
        public BonusTypeID HoldingBonusID { get; set; }
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
        public int CurrentBonusPayout { get; set; }
        // �A�`������Ԓ��̃{�[�i�X����
        public int CurrentZonePayout { get; set; }
        // �A�`������Ԃɂ��邩
        public bool HasZone { get; set; }
        // �Ō�Ɋl�������A�`������Ԃ̖���
        public int LastZonePayout { get; set; }

        public BonusSave()
        {
            HoldingBonusID = BonusTypeID.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigChanceColor = BigColor.None;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
            CurrentBonusPayout = 0;
            CurrentZonePayout = 0;
            HasZone = false;
            LastZonePayout = 0;
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
            CurrentBonusPayout = bonus.CurrentBonusPayout;
            CurrentZonePayout = bonus.CurrentZonePayout;
            LastZonePayout = bonus.LastZonePayout;
            HasZone = bonus.HasZone;
        }

        // �Z�[�u
        public List<int> SaveData()
        {
            // �ϐ����i�[
            List<int> data = new List<int>();

            data.Add((int)HoldingBonusID);
            Debug.Log("HoldingBonusID:" + HoldingBonusID);
            data.Add((int)CurrentBonusStatus);
            Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
            data.Add((int)BigChanceColor);
            Debug.Log("BigChanceColor:" + BigChanceColor);
            data.Add(RemainingBigGames);
            Debug.Log("RemainingBigGames:" + RemainingBigGames);
            data.Add(RemainingJacIn);
            Debug.Log("RemainingJacIn:" + RemainingJacIn);
            data.Add(RemainingJacHits);
            Debug.Log("RemainingJacHits:" + RemainingJacHits);
            data.Add(RemainingJacGames);
            Debug.Log("RemainingJacGames:" + RemainingJacGames);
            data.Add(CurrentBonusPayout);
            Debug.Log("CurrentBonusPayout:" + CurrentBonusPayout);
            data.Add(CurrentZonePayout);
            Debug.Log("CurrentZonePayout:" + CurrentZonePayout);
            data.Add(LastZonePayout);
            Debug.Log("LastZonePayout:" + LastZonePayout);
            data.Add(HasZone ? 1 : 0);
            Debug.Log("HasZone:" + HasZone);

            return data;
        }

        // �ǂݍ���
        public bool LoadData(BinaryReader br)
        {
            try
            {
                HoldingBonusID = (BonusTypeID)Enum.ToObject(typeof(BonusTypeID), br.ReadInt32());
                Debug.Log("HoldingBonusID:" + HoldingBonusID);
                CurrentBonusStatus = (BonusStatus)Enum.ToObject(typeof(BonusStatus), br.ReadInt32());
                Debug.Log("CurrentBonusStatus:" + CurrentBonusStatus);
                BigChanceColor = (BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32());
                Debug.Log("BigChanceColor:" + BigChanceColor);
                RemainingBigGames = br.ReadInt32();
                Debug.Log("RemainingBigGames:" + RemainingBigGames);
                RemainingJacIn = br.ReadInt32();
                Debug.Log("RemainingJacIn:" + RemainingJacIn);
                RemainingJacHits = br.ReadInt32();
                Debug.Log("RemainingJacHits:" + RemainingJacHits);
                RemainingJacGames = br.ReadInt32();
                Debug.Log("RemainingJacGames:" + RemainingJacGames);
                CurrentBonusPayout = br.ReadInt32();
                Debug.Log("CurrentBonusPayout:" + CurrentBonusPayout);
                CurrentZonePayout = br.ReadInt32();
                Debug.Log("CurrentZonePayout:" + CurrentZonePayout);
                LastZonePayout = br.ReadInt32();
                Debug.Log("LastZonePayout:" + LastZonePayout);
                HasZone = (br.ReadInt32() == 1 ? true : false);
                Debug.Log("HasZone:" + HasZone);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {

            }

            Debug.Log("BonusData end");
            return true;
        }
    }
}

