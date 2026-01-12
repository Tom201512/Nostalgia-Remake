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
            OtherOptionData.MusicVolumeSetting = otherOptionData.MusicVolumeSetting;
            OtherOptionData.SoundVolumeSetting = otherOptionData.SoundVolumeSetting;
            OtherOptionData.ResolutionSetting = otherOptionData.ResolutionSetting;
            OtherOptionData.UseOrthographicCamera = otherOptionData.UseOrthographicCamera;
            OtherOptionData.ShowMiniReelSetting = otherOptionData.ShowMiniReelSetting;
            OtherOptionData.SetMarkerPos(otherOptionData.AssistMarkerPos);
            OtherOptionData.HasWaitCut = otherOptionData.HasWaitCut;
            OtherOptionData.HasDelayDisplay = otherOptionData.HasDelayDisplay;
            OtherOptionData.CurrentLanguage = otherOptionData.CurrentLanguage;
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
            data.Add(OtherOptionData.SoundVolumeSetting);
            data.Add((int)OtherOptionData.ResolutionSetting);
            data.Add(OtherOptionData.UseOrthographicCamera ? 1 : 0);
            data.Add(OtherOptionData.ShowMiniReelSetting ? 1 : 0);

            foreach (int i in OtherOptionData.AssistMarkerPos)
            {
                data.Add(i);
            }

            data.Add(OtherOptionData.HasWaitCut ? 1 : 0);
            data.Add(OtherOptionData.HasDelayDisplay ? 1 : 0);
            data.Add((int)OtherOptionData.CurrentLanguage);

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

                for (int i = 0; i < ReelAmount; i++)
                {
                    AutoOptionData.StopPosLockData[i] = br.ReadInt32();
                }

                // その他設定
                OtherOptionData.MusicVolumeSetting = br.ReadInt32();
                OtherOptionData.SoundVolumeSetting = br.ReadInt32();
                OtherOptionData.ResolutionSetting = (ResolutionOptionID)Enum.ToObject(typeof(ResolutionOptionID), br.ReadInt32());
                OtherOptionData.UseOrthographicCamera = br.ReadInt32() == 1 ? true : false;
                OtherOptionData.ShowMiniReelSetting = br.ReadInt32() == 1 ? true : false;

                for (int i = 0; i < ReelAmount; i++)
                {
                    OtherOptionData.AssistMarkerPos[i] = br.ReadInt32();
                }

                OtherOptionData.HasWaitCut = br.ReadInt32() == 1 ? true : false;
                OtherOptionData.HasDelayDisplay = br.ReadInt32() == 1 ? true : false;
                OtherOptionData.CurrentLanguage = (LanguageOptionID)Enum.ToObject(typeof(LanguageOptionID), br.ReadInt32());
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