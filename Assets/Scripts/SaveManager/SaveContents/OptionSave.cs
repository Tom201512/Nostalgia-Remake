using ReelSpinGame_Datas;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Save.Database
{
    // オート設定
    public struct AutoOptionData
    {
        public AutoPlaySpeed AutoSpeedID;               // オート速度
        public AutoStopOrderOptions AutoStopOrders;     // オート時の押し順
        public bool HasTechnicalPlay;                   // 技術介入をするか
        public BigColor PlayerSelectedBigColor;         // 揃えるBIG色
        public AutoSpecificConditionID SpecificConditionBinary; // 一定条件のバイナリ値
        public AutoSpinTimeConditionID SpinConditionID; // 回転条件 
    }

    // その他設定
    public struct OtherOptionData
    {
        // 音量設定(0~8)
        public int MusicVolumeSetting;   // BGM音量
        public int SoundVolumeSetting;   // SE音量

        // その他
        public bool ShowMiniReelSetting;    // ミニリール表示設定
        public List<int> AssistMarkerPos;   // 目押しアシスト位置指定
        public bool HasWaitCut;             // ウェイトカット設定
        public bool HasDelayDisplay;        // スベリコマ表示設定
    }

    // オプション用のデータベース
    public class OptionSave
    {
        // const

        // var


        private AutoOptionData autoOptionData;      // オートオプションのデータ
        private OtherOptionData otherOptionData;    // その他設定のデータ

        public OptionSave()
        {
            autoOptionData = new AutoOptionData();

            // オート設定初期化
            autoOptionData.AutoSpeedID = AutoPlaySpeed.Normal;
            autoOptionData.AutoStopOrders = AutoStopOrderOptions.LMR;
            autoOptionData.HasTechnicalPlay = true;
            autoOptionData.PlayerSelectedBigColor = BigColor.None;
            autoOptionData.SpecificConditionBinary = AutoSpecificConditionID.None;
            autoOptionData.SpinConditionID = AutoSpinTimeConditionID.None;

            // その他設定初期化
            otherOptionData = new OtherOptionData();

            // 音量初期化
            otherOptionData.MusicVolumeSetting = 5;
            otherOptionData.SoundVolumeSetting = 5;

            // その他設定
            otherOptionData.ShowMiniReelSetting = false;
            otherOptionData.AssistMarkerPos = new List<int>()
            {
                -1,
                -1,
                -1,
            };
            otherOptionData.HasWaitCut = false;
            otherOptionData.HasDelayDisplay = false;
        }

        // func

        // データ記録
        public void RecordData(AutoOptionData autoOptionData, OtherOptionData otherOptionData)
        {
            this.autoOptionData = autoOptionData;
            this.otherOptionData = otherOptionData;
        }

        // セーブ
        public List<int> SaveData()
        {
            // 変数データをすべて格納
            List<int> data = new List<int>();

            // オート設定
            data.Add((int)autoOptionData.AutoSpeedID);  // 回数
            Debug.Log("AutoSpeed:" + autoOptionData.AutoSpeedID);

            data.Add((int)autoOptionData.AutoStopOrders); // 押し順
            Debug.Log("AutoOrder:" + autoOptionData.AutoStopOrders);

            data.Add(autoOptionData.HasTechnicalPlay ? 1 : 0); // 技術介入
            Debug.Log("AutoTechnical:" + autoOptionData.HasTechnicalPlay);

            data.Add((int)autoOptionData.PlayerSelectedBigColor); // BIG時の色選択
            Debug.Log("AutoBigColor:" + autoOptionData.PlayerSelectedBigColor);

            data.Add((int)autoOptionData.SpecificConditionBinary); // 一定条件のバイナリ
            Debug.Log("AutoSpecific:" + autoOptionData.SpecificConditionBinary);

            data.Add((int)autoOptionData.SpinConditionID); // 回転条件
            Debug.Log("AutoSpin:" + autoOptionData.SpinConditionID);


            // その他設定
            data.Add(otherOptionData.MusicVolumeSetting); // 音量
            Debug.Log("MusicVol:" + otherOptionData.MusicVolumeSetting);

            data.Add(otherOptionData.SoundVolumeSetting); // 効果音
            Debug.Log("SoundVol:" + otherOptionData.SoundVolumeSetting);

            data.Add(otherOptionData.ShowMiniReelSetting ? 1 : 0); // ミニリール表示
            Debug.Log("MiniReel:" + otherOptionData.ShowMiniReelSetting);

            // マーカー位置記録
            foreach (int i in otherOptionData.AssistMarkerPos)
            {
                data.Add(i);
                Debug.Log("Marker:" + i);
            }

            data.Add(otherOptionData.HasWaitCut ? 1 : 0); // ウェイトカット
            Debug.Log("WaitCut:" + otherOptionData.HasWaitCut);

            data.Add(otherOptionData.HasDelayDisplay ? 1 : 0); // スベリコマ表示
            Debug.Log("DelayDisplay:" + otherOptionData.HasDelayDisplay);

            return data;
        }

        // セーブ読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                // オート読み込み

                // 速度
                autoOptionData.AutoSpeedID = (AutoPlaySpeed)Enum.ToObject(typeof(AutoPlaySpeed), br.ReadInt32());
                Debug.Log("AutoSpeed:" + autoOptionData.AutoSpeedID);

                // 押し順
                autoOptionData.AutoStopOrders = (AutoStopOrderOptions)Enum.ToObject(typeof(AutoStopOrderOptions), br.ReadInt32());
                Debug.Log("AutoOrder:" + autoOptionData.AutoStopOrders);

                // 技術介入
                autoOptionData.HasTechnicalPlay = br.ReadInt32() == 1 ? true : false;
                Debug.Log("AutoTechnical:" + autoOptionData.HasTechnicalPlay);

                // BIG時の色
                autoOptionData.PlayerSelectedBigColor = (BigColor)Enum.ToObject(typeof(BigColor), br.ReadInt32());
                Debug.Log("AutoBigColor:" + autoOptionData.PlayerSelectedBigColor);

                // 一定条件のバイナリ
                autoOptionData.SpecificConditionBinary = (AutoSpecificConditionID)Enum.ToObject(typeof(AutoSpecificConditionID), br.ReadInt32());
                Debug.Log("AutoSpecific:" + autoOptionData.SpecificConditionBinary);

                // 回転条件
                autoOptionData.SpinConditionID = (AutoSpinTimeConditionID)Enum.ToObject(typeof(AutoSpinTimeConditionID), br.ReadInt32());
                Debug.Log("AutoSpin:" + autoOptionData.SpinConditionID);

                // その他設定
                // 音楽音量
                otherOptionData.MusicVolumeSetting = br.ReadInt32();
                Debug.Log("MusicVol:" + otherOptionData.MusicVolumeSetting);

                // 効果音
                otherOptionData.SoundVolumeSetting = br.ReadInt32();
                Debug.Log("SoundVol:" + otherOptionData.SoundVolumeSetting);

                // ミニリール表示
                otherOptionData.ShowMiniReelSetting = br.ReadInt32() == 1 ? true : false;
                Debug.Log("MiniReel:" + otherOptionData.ShowMiniReelSetting);

                // マーカー位置記録
                for(int i = 0; i < ReelSpinGame_Reels.ReelManagerModel.ReelAmount; i++)
                {
                    otherOptionData.AssistMarkerPos[i] = br.ReadInt32();
                    Debug.Log("Marker [" + i + "]:" + otherOptionData.AssistMarkerPos[i]);
                }
                // ウェイトカット
                otherOptionData.HasWaitCut = br.ReadInt32() == 1 ? true : false;
                Debug.Log("WaitCut:" + otherOptionData.HasWaitCut);

                // スベリコマ表示
                otherOptionData.HasDelayDisplay = br.ReadInt32() == 1 ? true : false;
                Debug.Log("DelayDisplay:" + otherOptionData.HasDelayDisplay);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {

            }
            Debug.Log("Option Read is done");
            return true;
        }
    }
}