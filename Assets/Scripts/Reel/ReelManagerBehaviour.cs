using System.Collections.Generic;
using ReelSpinGame_Reels.Table;

namespace ReelSpinGame_Reels
{
	public class ReelManagerBehaviour
	{
        // リールマネージャーのデータ

        // const 
        // リール数
        public const int ReelAmount = 3;
        // リール識別用ID
        public enum ReelID { ReelLeft, ReelMiddle, ReelRight };
        // 最大 ランダムテーブル数(1~6)
        public const int MaxRandomLots = 6;

        // var
        // 全リールが動作中か
        public bool IsReelWorking { get; set; }
        // 動作完了したか
        public bool IsReelFinished { get; set; }
        // 停止可能になったか(リール速度が一定になって0.5秒後)
        public bool CanStopReels { get; set; }
        // 強制停止が発動しているか
        public bool HasForceStop { get; set; }
        // 第一停止をしたか
        public bool IsFirstReelPushed { get; set; }
        // 最初に止めたリール番号
        public ReelID FirstPushReel { get; set; }
        // 第一停止リールの停止位置
        public int FirstPushPos { get; set; }
        // 停止したリール数
        public int StoppedReelCount { get; set; }

        // リール制御
        public ReelTableManager ReelTableManager { get; private set; }

        // 最後に止めたリールのデータ
        public LastStoppedReelData LastStopped { get; private set; }

        // リールテーブルのランダム数値
        public int RandomValue { get; set; }

        // コンストラクタ
        public ReelManagerBehaviour()
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

            LastStopped = new LastStoppedReelData();
            ReelTableManager = new ReelTableManager();
        }

        // 最後に止めたリールデータを作る
        public void GenerateLastStopped(List<ReelObject> reelObjects) => LastStopped.GenerateLastStopped(reelObjects);
    }
}