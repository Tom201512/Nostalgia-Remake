using ReelSpinGame_Datas;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Save.Bonus;
using ReelSpinGame_Save.Medal;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using ReelSpinGame_Save.Database.Option;
using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelLogicManager;

namespace ReelSpinGame_Save.Database
{
    // オプション用のデータベース
    public class OptionSave : ISavable
    {
        // const

        // var
        public AutoOptionData AutoOptionData { get; private set; }          // オートオプションのデータ
        public OtherOptionData OtherOptionData { get; private set; }       // その他オプションの設定

        public OptionSave()
        {
            AutoOptionData = new AutoOptionData();
            OtherOptionData = new OtherOptionData();
        }

        // func

        // データを初期化
        public void InitializeSave()
        {
            AutoOptionData.InitializeData();
            OtherOptionData.InitializeData();
        }

        // データ記録
        // オート設定
        public void RecordAutoData(AutoOptionData autoOptionData)
        {
            AutoOptionData.SetAutoSpeed(autoOptionData.AutoSpeedID);
            AutoOptionData.SetAutoStopOrder(autoOptionData.AutoStopOrdersID);
            AutoOptionData.SetBigColor(autoOptionData.BigColorLineUpID);
            AutoOptionData.SetTechnicalPlay(autoOptionData.HasTechnicalPlay);
            AutoOptionData.SetSpecificCondition(autoOptionData.SpecificConditionBinary);
            AutoOptionData.SetSpinCondition(autoOptionData.SpinConditionID);
        }

        // その他設定
        public void RecordOtherData(OtherOptionData otherOptionData)
        {
            OtherOptionData = otherOptionData;
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数データをすべて格納
            List<int> data = new List<int>();

            // オート設定
            data.Add((int)AutoOptionData.AutoSpeedID);  // 回数
            data.Add((int)AutoOptionData.AutoStopOrdersID); // 押し順
            data.Add(AutoOptionData.HasTechnicalPlay ? 1 : 0); // 技術介入
            data.Add((int)AutoOptionData.BigColorLineUpID); // BIG時の色選択
            data.Add((int)AutoOptionData.SpecificConditionBinary); // 一定条件のバイナリ
            data.Add((int)AutoOptionData.SpinConditionID); // 回転条件

            // その他設定
            data.Add(OtherOptionData.MusicVolumeSetting); // 音量
            Debug.Log("MusicVol:" + OtherOptionData.MusicVolumeSetting);

            data.Add(OtherOptionData.SoundVolumeSetting); // 効果音
            Debug.Log("SoundVol:" + OtherOptionData.SoundVolumeSetting);

            data.Add(OtherOptionData.ShowMiniReelSetting ? 1 : 0); // ミニリール表示
            Debug.Log("MiniReel:" + OtherOptionData.ShowMiniReelSetting);

            // マーカー位置記録
            foreach (int i in OtherOptionData.AssistMarkerPos)
            {
                data.Add(i);
                Debug.Log("Marker:" + i);
            }

            data.Add(OtherOptionData.HasWaitCut ? 1 : 0); // ウェイトカット
            Debug.Log("WaitCut:" + OtherOptionData.HasWaitCut);

            data.Add(OtherOptionData.HasDelayDisplay ? 1 : 0); // スベリコマ表示
            Debug.Log("DelayDisplay:" + OtherOptionData.HasDelayDisplay);

            return data;
        }

        // セーブ読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                // オート読み込み
                // 速度
                AutoOptionData.SetAutoSpeed((AutoPlaySpeed)Enum.ToObject(typeof(AutoPlaySpeed), br.ReadInt32()));
                // 押し順
                AutoOptionData.SetAutoStopOrder((AutoStopOrderOptions)Enum.ToObject(typeof(AutoStopOrderOptions), br.ReadInt32()));
                // 技術介入
                AutoOptionData.SetTechnicalPlay(br.ReadInt32() == 1 ? true : false);
                // BIG時の色
                AutoOptionData.SetBigColor((BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32()));
                // 一定条件のバイナリ
                AutoOptionData.SetSpecificCondition((byte)br.ReadInt32());
                // 回転条件
                AutoOptionData.SetSpinCondition((AutoSpinTimeConditionID)Enum.ToObject(typeof(AutoSpinTimeConditionID), br.ReadInt32()));

                // その他設定
                // 音楽音量
                OtherOptionData.SetMusicVolume(br.ReadInt32());
                Debug.Log("MusicVol:" + OtherOptionData.MusicVolumeSetting);

                // 効果音
                OtherOptionData.SetSoundVolume(br.ReadInt32());
                Debug.Log("SoundVol:" + OtherOptionData.SoundVolumeSetting);

                // ミニリール表示
                OtherOptionData.SetMiniReel(br.ReadInt32() == 1 ? true : false);
                Debug.Log("MiniReel:" + OtherOptionData.ShowMiniReelSetting);

                // マーカー位置記録
                for(int i = 0; i < ReelAmount; i++)
                {
                    OtherOptionData.AssistMarkerPos[i] = br.ReadInt32();
                    Debug.Log("Marker [" + i + "]:" + OtherOptionData.AssistMarkerPos[i]);
                }
                // ウェイトカット
                OtherOptionData.SetHasWaitCut(br.ReadInt32() == 1 ? true : false);
                Debug.Log("WaitCut:" + OtherOptionData.HasWaitCut);

                // スベリコマ表示
                OtherOptionData.SetHasDelayDisplay(br.ReadInt32() == 1 ? true : false);
                Debug.Log("DelayDisplay:" + OtherOptionData.HasDelayDisplay);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {

            }

            return true;
        }
    }
}