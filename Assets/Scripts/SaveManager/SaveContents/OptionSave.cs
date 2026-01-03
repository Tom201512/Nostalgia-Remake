using ReelSpinGame_AutoPlay;
using ReelSpinGame_Interface;
using ReelSpinGame_Reels;
using ReelSpinGame_Save.Database.Option;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelLogicManager;

namespace ReelSpinGame_Save.Database
{
    // オプション用のデータベース
    public class OptionSave : ISavable
    {
        public AutoOptionData AutoOptionData { get; private set; }          // オートオプションのデータ
        public OtherOptionData OtherOptionData { get; private set; }        // その他オプションの設定

        public OptionSave()
        {
            AutoOptionData = new AutoOptionData();
            OtherOptionData = new OtherOptionData();
        }

        // データを初期化
        public void InitializeSave()
        {
            AutoOptionData.InitializeData();
            OtherOptionData.InitializeData();
        }

        // オート設定データの設定
        public void RecordAutoData(AutoOptionData autoOptionData)
        {
            AutoOptionData.CurrentSpeed = autoOptionData.CurrentSpeed;
            AutoOptionData.CurrentStopOrder = autoOptionData.CurrentStopOrder;
            AutoOptionData.BigLineUpSymbol = autoOptionData.BigLineUpSymbol;
            AutoOptionData.HasTechnicalPlay = autoOptionData.HasTechnicalPlay;
            AutoOptionData.EndConditionFlag = autoOptionData.EndConditionFlag;
            AutoOptionData.SpinConditionID = autoOptionData.SpinConditionID;
            AutoOptionData.StopPosLockData[(int)ReelID.ReelLeft] = autoOptionData.StopPosLockData[(int)ReelID.ReelLeft];
            AutoOptionData.StopPosLockData[(int)ReelID.ReelMiddle] = autoOptionData.StopPosLockData[(int)ReelID.ReelMiddle];
            AutoOptionData.StopPosLockData[(int)ReelID.ReelRight] = autoOptionData.StopPosLockData[(int)ReelID.ReelRight];
        }

        // その他設定
        public void RecordOtherData(OtherOptionData otherOptionData)
        {
            OtherOptionData = otherOptionData;
        }

        // セーブ
        public List<int> SaveData()
        {
            List<int> data = new List<int>();

            data.Add((int)AutoOptionData.CurrentSpeed);
            data.Add((int)AutoOptionData.CurrentStopOrder);
            data.Add(AutoOptionData.HasTechnicalPlay ? 1 : 0);
            data.Add((int)AutoOptionData.BigLineUpSymbol);
            data.Add((int)AutoOptionData.EndConditionFlag);
            data.Add((int)AutoOptionData.SpinConditionID);

            for (int i = 0; i < AutoOptionData.StopPosLockData.Count; i++)
            {
                data.Add(AutoOptionData.StopPosLockData[i]);
            }

            // その他設定
            data.Add(OtherOptionData.MusicVolumeSetting);
            Debug.Log("MusicVol:" + OtherOptionData.MusicVolumeSetting);

            data.Add(OtherOptionData.SoundVolumeSetting);
            Debug.Log("SoundVol:" + OtherOptionData.SoundVolumeSetting);

            data.Add(OtherOptionData.UseOrtographCamera ? 1 : 0);
            Debug.Log("2Dmode:" + OtherOptionData.UseOrtographCamera);

            data.Add(OtherOptionData.ShowMiniReelSetting ? 1 : 0);
            Debug.Log("MiniReel:" + OtherOptionData.ShowMiniReelSetting);

            // マーカー位置記録
            foreach (int i in OtherOptionData.AssistMarkerPos)
            {
                data.Add(i);
                Debug.Log("Marker:" + i);
            }

            data.Add(OtherOptionData.HasWaitCut ? 1 : 0);
            Debug.Log("WaitCut:" + OtherOptionData.HasWaitCut);

            data.Add(OtherOptionData.HasDelayDisplay ? 1 : 0);
            Debug.Log("DelayDisplay:" + OtherOptionData.HasDelayDisplay);

            return data;
        }

        // セーブ読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                // オート読み込み
                AutoOptionData.CurrentSpeed = (AutoSpeedName)Enum.ToObject(typeof(AutoSpeedName), br.ReadInt32());
                AutoOptionData.CurrentStopOrder = (StopOrderOptionName)Enum.ToObject(typeof(StopOrderOptionName), br.ReadInt32());
                AutoOptionData.HasTechnicalPlay = br.ReadInt32() == 1 ? true : false;
                AutoOptionData.BigLineUpSymbol = (BigType)Enum.ToObject(typeof(BigType), br.ReadInt32());
                AutoOptionData.EndConditionFlag = (byte)br.ReadInt32();
                AutoOptionData.SpinConditionID = (SpinTimeConditionName)Enum.ToObject(typeof(SpinTimeConditionName), br.ReadInt32());

                for(int i = 0; i < ReelAmount; i++)
                {
                    AutoOptionData.StopPosLockData[i] = br.ReadInt32();
                }

                // その他設定
                OtherOptionData.MusicVolumeSetting = br.ReadInt32();
                Debug.Log("MusicVol:" + OtherOptionData.MusicVolumeSetting);

                OtherOptionData.SoundVolumeSetting = br.ReadInt32();
                Debug.Log("SoundVol:" + OtherOptionData.SoundVolumeSetting);

                OtherOptionData.UseOrtographCamera = br.ReadInt32() == 1 ? true : false;

                OtherOptionData.ShowMiniReelSetting = br.ReadInt32() == 1 ? true : false;
                Debug.Log("MiniReel:" + OtherOptionData.ShowMiniReelSetting);

                for (int i = 0; i < ReelAmount; i++)
                {
                    OtherOptionData.AssistMarkerPos[i] = br.ReadInt32();
                    Debug.Log("Marker [" + i + "]:" + OtherOptionData.AssistMarkerPos[i]);
                }

                OtherOptionData.HasWaitCut = br.ReadInt32() == 1 ? true : false;
                Debug.Log("WaitCut:" + OtherOptionData.HasWaitCut);

                OtherOptionData.HasDelayDisplay = br.ReadInt32() == 1 ? true : false;
                Debug.Log("DelayDisplay:" + OtherOptionData.HasDelayDisplay);
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