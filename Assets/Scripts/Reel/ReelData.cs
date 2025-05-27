using ReelSpinGame_Datas;
using System;
using System.Security.Cryptography;
using UnityEngine;

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
        public enum ReelPosID { Lower3rd = -2, Lower2nd, Lower, Center, Upper, Upper2nd, Upper3rd }

        // var
        // リール識別ID
        public int ReelID{ get; private set; }
        // リールのデータベース
        public ReelDatabase ReelDatabase { get; private set; }
        // 現在の下段リール位置
        private int currentLower;

        // 止まる予定か
        public bool IsStopping { get; private set; }
        // 停止したか
        public bool HasStopped { get; private set; }
        // 最後に止めた位置(下段基準)
        public int LastPressedPos { get; private set; }
        // 将来的に止まる位置(下段基準)
        public int WillStopPos { get; private set; }
        // 最後に止めたときのディレイ数
        public int LastDelay { get; private set; }

        // コンストラクタ
        public ReelData(int reelID, int lowerPos, ReelDatabase reelDatabase) 
        {
            LastPressedPos = 0;
            LastDelay = 0;
            WillStopPos = 0;
            IsStopping = false;
            HasStopped = true;

            ReelID = reelID;
            // 位置設定
            // もし位置が0~20でなければ例外を出す
            if (lowerPos < 0 ||  lowerPos > MaxReelArray - 1)
            {
                throw new Exception("Invalid Position num. Must be within 0 ~ " + (MaxReelArray - 1));
            }
            currentLower = lowerPos;
            ReelDatabase = reelDatabase;

            foreach (byte value in ReelDatabase.Array)
            {
                Debug.Log(value + "Symbol:" + ReturnSymbol(value));
            }

            Debug.Log("ReelGenerated Position at:" + lowerPos);

            for(int i = 0; i < ReelDatabase.Array.Length; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReturnSymbol(ReelDatabase.Array[i]));
            }
        }

        // func
        // リール配列の番号を図柄へ変更
        public static ReelSymbols ReturnSymbol(int reelIndex) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), reelIndex);
        // 指定したリールの位置番号を返す
        public int GetReelPos(int posID) => OffsetReel(currentLower, posID);
        // リールの位置から図柄を返す
        public ReelSymbols GetReelSymbol(int posID) => ReturnSymbol(ReelDatabase.Array[OffsetReel(currentLower, posID)]);
        // 停止予定の位置からリール図柄を返す
        public ReelSymbols GetSymbolFromWillStop(int posID) => ReturnSymbol(ReelDatabase.Array[OffsetReel(WillStopPos, posID)]);
        // リール位置変更 (回転速度の符号に合わせて変更)
        public void ChangeReelPos(float rotateSpeed) => currentLower = OffsetReel(currentLower, (int)Mathf.Sign(rotateSpeed));
        // リール位置を配列要素に置き換える
        public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower3rd * -1;
        // 停止したリール位置を返す(中段)
        public int GetStoppedPos() => GetReelPos((int)ReelPosID.Center);

        // 停止位置になったか
        public bool CheckReachedStop() => currentLower == WillStopPos;

        // 回転を開始させる
        public void BeginStartReel()
        {
            HasStopped = false;
            WillStopPos = 0;
            LastDelay = 0;
            WillStopPos = 0;
        }

        // 停止させる
        public void BeginStopReel(int pushedPos, int delay)
        {
            LastPressedPos = pushedPos;

            if (delay < 0 || delay > MaxDelay)
            {
                throw new Exception("Invalid Delay. Must be within 0~4");
            }
            Debug.Log("Received Stop Delay:" + delay);

            // テーブルから得たディレイを記録し、その分リールの停止を遅らせる。
            WillStopPos = OffsetReel(LastPressedPos, delay);
            Debug.Log("WillStop:" + WillStopPos);
            LastDelay = delay;
            IsStopping = true;
        }

        // 停止処理を終了させる
        public void FinishStopReel()
        {
            IsStopping = false;
            HasStopped = true;
        }

        // リール位置をオーバーフローしない数値で返す
        private int OffsetReel(int reelPos, int offset)
        {
            Debug.Log("ReelPos:" + reelPos);
            if (reelPos + offset < 0)
            {
                Debug.Log("Offset:" + (MaxReelArray + reelPos + offset));
                return MaxReelArray + reelPos + offset;
            }

            else if (reelPos + offset > MaxReelArray - 1)
            {
                Debug.Log("Offset:" + (reelPos + offset - MaxReelArray));
                return reelPos + offset - MaxReelArray;
            }
            // オーバーフローがないならそのまま返す
            Debug.Log("Offset:" + (reelPos + offset));
            return reelPos + offset;
        }
    }
}
