using ReelSpinGame_Datas;
using System;
using UnityEngine;
using ReelSpinGame_Reels.Symbol;

namespace ReelSpinGame_Reels
{
    public class ReelData
    {
        // リールのデータ

        // const 
        // リール配列要素数
        public const int MaxReelArray = 21;
        // 最高ディレイ(スベリコマ数 4)
        public const int MaxDelay = 4;

        // 図柄
        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay }
        // リール位置識別用
        public enum ReelPosID { Lower2nd = -1, Lower, Center, Upper, Upper2nd }
        // リールの状態
        public enum ReelStatus { WaitForStop, Stopping, Stopped}

        // var
        // リール識別ID
        public int ReelID{ get; private set; }
        // リールのデータベース
        public ReelDatabase ReelDatabase { get; private set; }
        // 現在の下段リール位置
        private int currentLower;

        // 現在のリールステータス
        public ReelStatus CurrentReelStatus { get; private set; }
        // 最後に止めた位置(下段基準)
        public int LastPushedPos { get; private set; }
        // 将来的に止まる位置(下段基準)
        public int WillStopPos { get; private set; }
        // 最後に止めたときのディレイ数
        public int LastDelay { get; private set; }

        // コンストラクタ
        public ReelData(int reelID, int lowerPos, ReelDatabase reelDatabase) 
        {
            CurrentReelStatus = ReelStatus.WaitForStop;
            LastPushedPos = 0;
            LastDelay = 0;
            WillStopPos = 0;

            ReelID = reelID;
            // 位置設定
            // もし位置が0~20でなければ例外を出す
            if (lowerPos < 0 ||  lowerPos > MaxReelArray - 1)
            {
                throw new Exception("Invalid Position num. Must be within 0 ~ " + (MaxReelArray - 1));
            }
            currentLower = lowerPos;
            ReelDatabase = reelDatabase;
        }

        // func

        // 指定したリール位置にする
        public void SetReelPos(int lowerPos) => currentLower = lowerPos;
        // 指定したリールの位置番号を返す
        public int GetReelPos(int posID) => OffsetReel(currentLower, posID);
        // リール位置変更 (回転速度の符号に合わせて変更)
        public void ChangeReelPos(float rotateSpeed) => currentLower = OffsetReel(currentLower, (int)Mathf.Sign(rotateSpeed));
        // 停止位置になったか
        public bool CheckReachedStop() => currentLower == WillStopPos;

        // 回転を開始させる
        public void BeginStartReel()
        {
            CurrentReelStatus = ReelStatus.WaitForStop;
            WillStopPos = 0;
            LastDelay = 0;
        }

        // 停止させる(強制的に止めることも可能)
        public void BeginStopReel(int pushedPos, int delay, bool stopImmediately)
        {
            if (delay < 0 || delay > MaxDelay)
            {
                throw new Exception("Invalid Delay. Must be within 0~4");
            }

            // 停止位置記録
            LastPushedPos = pushedPos;
            // テーブルから得たディレイを記録し、その分リールの停止を遅らせる。
            WillStopPos = OffsetReel(LastPushedPos, delay);
            LastDelay = delay;

            // 強制停止をオンにした場合は即座にリール位置を変更する。
            if(stopImmediately)
            {
                currentLower = WillStopPos;
                CurrentReelStatus = ReelStatus.Stopped;
            }
            else
            {
                CurrentReelStatus = ReelStatus.Stopping;
            }
        }

        // 停止処理を終了させる
        public void FinishStopReel() => CurrentReelStatus = ReelStatus.Stopped;

        // リール位置をオーバーフローしない数値で返す
        public static int OffsetReel(int reelPos, int offset)
        {
            if (reelPos + offset < 0)
            {
                return MaxReelArray + reelPos + offset;
            }

            else if (reelPos + offset > MaxReelArray - 1)
            {
                return reelPos + offset - MaxReelArray;
            }
            // オーバーフローがないならそのまま返す
            return reelPos + offset;
        }
    }
}
