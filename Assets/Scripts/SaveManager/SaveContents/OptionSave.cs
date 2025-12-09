using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using ReelSpinGame_Save.Bonus;
using ReelSpinGame_Save.Medal;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using ReelSpinGame_System;
using System;
using System.Collections.Generic;
using System.IO;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_Save.Database
{
    // オプション用のデータベース
    public class OptionSave
    {
        // オート設定
        public struct AutoOptionData
        {
            public int AutoSpeedID;                 // オート速度
            public ReelID[] AutoStopOrders;         // オート時の押し順
            public int AutoOrderID;                 // オート押し順のオプション数値
            public bool HasTechnicalPlay;           // 技術介入をするか
            public bool HasRiichiStop;              // ボーナス成立時、リーチ目を狙うか
            public bool HasStoppedRiichiPtn;        // リーチ目を止めたか
            public bool HasOneBetBonusLineUp;       // ボーナスを1枚掛けで揃えさせるか
            public BigColor PlayerSelectedBigColor; // 揃えるBIG色
        }

        // 音量設定
        
        // ミニリール表示設定

        // 目押しアシスト位置指定

        // ウェイトカット設定

        // スベリコマ表示設定

        // 

        public AutoOptionData AutoOptionSave { get; private set; } // オートオプションのデータ


        public OptionSave()
        {

        }

        // func

        /*
        // データ記録
        public void RecordData(PlayerDatabase playerData)
        {

        }

        // セーブ
        public List<int> SaveData()
        {

        }

        // セーブ読み込み
        public bool LoadData(BinaryReader br)
        {

        }*/
    }
}