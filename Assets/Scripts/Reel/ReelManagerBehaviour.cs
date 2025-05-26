using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_Reels
{
	public class ReelManagerBehaviour
	{
        // リールマネージャーのデータ

        // const 
        // リール数
        public const int ReelAmounts = 3;
        // リール識別用ID
        public enum ReelID { ReelLeft, ReelMiddle, ReelRight };
        // 最大 ランダムテーブル数(1~6)
        const int MaxRandomLots = 6;

        // var
        // 全リールが動作中か
        public bool IsWorking { get; private set; }
        // 動作完了したか
        public bool IsFinished { get; private set; }
        // 停止可能になったか(リール速度が一定になって0.5秒後)
        public bool CanStop { get; private set; }
        // 第一停止をしたか
        public bool IsFirstReelPushed { get; private set; }
        // 最初に止めたリール番号
        public ReelID FirstPushReel { get; private set; }
        // 第一停止リールの停止位置
        public int FirstPushPos { get; private set; }

        // リール制御
        public ReelTableManager ReelTableManager { get; private set; }

        // 最後に止めた位置
        public List<int> LastPos { get; private set; }
        // 最後に止まった出目
        public List<List<ReelSymbols>> LastSymbols { get; private set; }

        // リールテーブルのランダム数値
        public int RandomValue { get; private set; }

        // コンストラクタ
        public ReelManagerBehaviour()
        {
            IsFinished = true;
            IsWorking = false;
            CanStop = false;

            IsFirstReelPushed = false;
            FirstPushReel = ReelID.ReelLeft;
            FirstPushPos = 0;
            RandomValue = 0;

            LastPos = new List<int>();
            LastSymbols = new List<List<ReelSymbols>>();

            ReelTableManager = new ReelTableManager();
        }

        // func
        // プロパティ設定

        // 動作終了したか
        public void SetIsFinished(bool value) => IsFinished = value;
        // リールが動いているか
        public void SetIsWorking(bool value) => IsWorking = value;
        // 第一停止があったか
        public void SetIsFirstReelPushed(bool value) => IsFirstReelPushed = value;
        // 停止可能状態か
        public void SetCanStop(bool value) => CanStop = value;
        // 第一停止の設定
        public void SetFirstPushReel(ReelID value) => FirstPushReel = value;
        // 第一停止位置の設定
        public void SetFirstPushPos(int value) => FirstPushPos = value;

        // ランダム数値の決定
        public void SetRandomValue(bool hasInstantMode, int instantRandomValue)
        {
            RandomValue = Random.Range(1, MaxRandomLots);
            if (hasInstantMode)
            {
                RandomValue = instantRandomValue;
            }
        }

        // 最後に止めた結果を作る
        public void GenerateLastStopped(ReelObject[] reelObjects)
        {
            // 初期化
            LastPos.Clear();
            LastSymbols.Clear();

            string posBuffer = "";

            // リール図柄を作成する
            for (int i = 0; i < reelObjects.Length; i++)
            {
                LastPos.Add(reelObjects[i].ReelData.GetReelPos((int)ReelPosID.Lower));
                posBuffer += LastPos[i];

                Debug.Log("Position:" + LastPos[i]);

                // データ作成
                LastSymbols.Add(new List<ReelSymbols>());

                // 各位置の図柄を得る(枠下2段目から枠上2段目まで)
                for (int j = (int)ReelData.ReelPosID.Lower3rd; j < (int)ReelData.ReelPosID.Upper3rd; j++)
                {
                    LastSymbols[i].Add(reelObjects[i].ReelData.GetReelSymbol(j));
                    Debug.Log("Symbol:" + reelObjects[i].ReelData.GetReelSymbol(j));
                }
            }
            Debug.Log("Final ReelPosition" + posBuffer);

            // 各リールごとに表示(デバッグ)
            foreach (List<ReelSymbols> reelResult in LastSymbols)
            {
                Debug.Log("Reel:");
                for (int i = 0; i < reelResult.Count; i++)
                {
                    Debug.Log(reelResult[i]);
                }
            }
        }
    }
}