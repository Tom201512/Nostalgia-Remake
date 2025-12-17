using System.Collections.Generic;
using ReelSpinGame_Reels.Table;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_Reels
{
    // リール回転マネージャーのデータ
    public class ReelSpinManagerModel
    {
        // const 
        public const int MaxRandomLots = 6;             // 最大 ランダムテーブル数(1~6)

        // var
        public bool IsReelWorking { get; set; }         // 全リールが動作中か
        public bool IsReelFinished { get; set; }        // 動作完了したか
        public bool CanStopReels { get; set; }          // 停止可能か
        public bool HasForceStop { get; set; }          // 強制停止が発動しているか
        public bool IsFirstReelPushed { get; set; }     // 第一停止をしたか
        public ReelID FirstPushReel { get; set; }       // 最初に止めたリール番号
        public int FirstPushPos { get; set; }           // 第一停止リールの停止位置
        public int StoppedReelCount { get; set; }       // 停止したリール数
        public int RandomValue { get; set; }            // リールテーブルのランダム数値

        public LastStoppedReelData LastStoppedReelData { get; private set; }    // 最後に止めたリールのデータ

        // コンストラクタ
        public ReelSpinManagerModel()
        {
            IsReelFinished = true;
            IsReelWorking = false;
            CanStopReels = false;
            HasForceStop = false;

            IsFirstReelPushed = false;
            FirstPushReel = ReelID.ReelLeft;
            FirstPushPos = 0;
            RandomValue = 0;
            StoppedReelCount = 0;

            LastStoppedReelData = new LastStoppedReelData();
        }

        // 最後に止めたリールデータを作る
        public void GenerateLastStopped(List<ReelObjectPresenter> reelObjects) => LastStoppedReelData.GenerateLastStopped(reelObjects);
    }
}