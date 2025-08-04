using ReelSpinGame_Interface;
using ReelSpinGame_Save.Medal;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Medal
{
    //スロット内部のメダル管理
    public class MedalManager : MonoBehaviour, IHasSave
    {
        // const
        // メダル更新の間隔(ミリ秒)
        public const float MedalUpdateTime = 0.12f;

        // var
        // メダル処理のデータ
        private MedalBehavior data;

        // メダルのUI
        [SerializeField] private MedalPanel medalPanel;

        // クレジットセグメント
        [SerializeField] private MedalSevenSegment creditSegments;
        // 払い出しセグメント
        [SerializeField] private MedalSevenSegment payoutSegments;

        // メダルの更新処理中か
        public bool HasMedalUpdate { get; private set; }
        // メダルが投入されたか
        public delegate void MedalHasInsertEvent();
        public event MedalHasInsertEvent HasMedalInsert;

        void Awake()
        {
            data = new MedalBehavior();
            HasMedalUpdate = false;
        }

        void Start()
        {
            // クレジット更新
            creditSegments.ShowSegmentByNumber(data.system.Credits);
        }

        // タイマー処理の破棄
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // 数値を得る
        public int GetCredits() => data.system.Credits;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetRemainingPayouts() => data.RemainingPayouts;
        public int GetMaxBet() => data.system.MaxBetAmounts;
        public int GetLastBetAmounts() => data.system.LastBetAmounts;
        public int GetLastPayout() => data.LastPayoutAmounts;
        public bool GetBetFinished() => data.FinishedBet;
        public bool GetHasReplay() => data.system.HasReplay;

        // セーブデータにする
        public ISavable MakeSaveData()
        {
            MedalSave save = new MedalSave();
            save.RecordData(data.system);
            return save;
        }

        // セーブを読み込む
        public void LoadSaveData(ISavable loadData)
        {
            if(loadData.GetType() == typeof(MedalSave))
            {
                MedalSave save = loadData as MedalSave;

                data.system.Credits = save.Credits;
                data.system.MaxBetAmounts = save.MaxBetAmounts;
                data.system.LastBetAmounts = save.LastBetAmounts;
                data.system.HasReplay = save.HasReplay;
            }
            else
            {
                throw new Exception("Loaded data is not MedalData");
            }
        }

        // 数値を変える
        public int ChangeMaxBet(int amounts) => data.system.MaxBetAmounts = Math.Clamp(amounts, 0, MaxBetLimit);

        // MAX_BET用の処理
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.system.MaxBetAmounts, false);
        }

        // ベット処理開始
        public void StartBet(int amounts, bool cutCoroutine)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!data.system.HasReplay && !HasMedalUpdate)
            {
                // 枚数を調整
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)
                if (amounts != data.CurrentBet && amounts <= data.system.MaxBetAmounts)
                {
                    // ベット枚数設定
                    data.SetRemainingBet(amounts);

                    // コルーチンを無視する場合
                    if (cutCoroutine)
                    {
                        for(int i = 0; i < data.RemainingBet; i++)
                        {
                            data.InsertOneMedal();
                        }

                        // ランプ、セグメント更新
                        medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmounts);
                        // クレジット更新
                        creditSegments.ShowSegmentByNumber(data.system.Credits);
                        // 払い出しセグメントを消す
                        payoutSegments.TurnOffAllSegments();
                    }
                    else
                    {
                        // メダルの投入を開始する(残りはフレーム処理)
                        StartCoroutine(nameof(UpdateInsert));
                    }
                }
            }
        }

        // 払い出し開始
        public void StartPayout(int amounts, bool cutCoroutine)
        {
            Debug.Log("CutCoroutine:" + cutCoroutine);
            // 払い出しをしていないかチェック
            if (!HasMedalUpdate)
            {
                // メダルの払い出しを開始する(メダルが増えるのは演出で、データ上ではすでに増えている)
                if (amounts > 0)
                {
                    // 払い出し枚数の設定
                    data.RemainingPayouts = Math.Clamp(data.RemainingPayouts + amounts, 0, MaxPayout);
                    // クレジットの増加
                    data.ChangeCredits(amounts);

                    // コルーチンを無視する場合
                    if (cutCoroutine)
                    {
                        Debug.Log("Remaining:" +  data.RemainingPayouts);
                        while(data.RemainingPayouts > 0)
                        {
                            data.PayoutOneMedal();
                        }

                        // クレジットと払い出しセグメント更新
                        creditSegments.ShowSegmentByNumber(data.system.Credits);
                        Debug.Log("Payout:" + data.LastPayoutAmounts);
                        payoutSegments.ShowSegmentByNumber(data.LastPayoutAmounts);

                    }
                    else
                    {
                        // 払い出し開始
                        StartCoroutine(nameof(UpdatePayout));
                    }
                }
            }
        }

        // リプレイ状態にする(前回と同じメダル枚数をかける)
        public void EnableReplay()
        {
            data.system.HasReplay = true;
            data.RemainingBet = data.system.LastBetAmounts;
        }

        // リプレイ状態を消す
        public void DisableReplay() => data.system.HasReplay = false;

        // リプレイ投入を開始
        public void StartReplayInsert(bool hasCoroutineCut)
        {
            // コルーチンを無視する場合
            if(hasCoroutineCut)
            {
                for (int i = 0; i < data.system.LastBetAmounts; i++)
                {
                    data.InsertOneMedal();
                }

                // ランプ、セグメント更新
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmounts);
                // クレジット更新
                creditSegments.ShowSegmentByNumber(data.system.Credits);
                // 払い出しセグメントを消す
                payoutSegments.TurnOffAllSegments();
            }
            else
            {
                StartCoroutine(nameof(UpdateInsert));
            }
        }

        // メダル処理終了
        public void FinishMedalInsert()
        {
            data.CurrentBet = 0;
            data.FinishedBet = false;
        }

        // func
        // コルーチン用
        private IEnumerator UpdateInsert()
        {
            ////Debug.Log("StartBet");
            HasMedalUpdate = true;
            // 残りベット枚数がなくなるまで処理
            while (data.RemainingBet > 0)
            {
                // メダル投入
                data.InsertOneMedal();
                // イベント送信
                HasMedalInsert.Invoke();
                // ランプ、セグメント更新
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmounts);
                // クレジット更新
                creditSegments.ShowSegmentByNumber(data.system.Credits);
                // 払い出しセグメントを消す
                payoutSegments.TurnOffAllSegments();
                // 0.12秒待機
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
            //////Debug.Log("Bet Finished");
            //////Debug.Log("CurrentBet:" + data.CurrentBet);
        }

        private IEnumerator UpdatePayout()
        {
            HasMedalUpdate = true;
            // 払い出し処理
            while (data.RemainingPayouts > 0)
            {
                // メダル払い出し
                data.PayoutOneMedal();
                //HasMedalPayout.Invoke(1);
                // クレジットと払い出しセグメント更新
                creditSegments.ShowSegmentByNumber(data.system.Credits - data.RemainingPayouts);
                ////Debug.Log("LastPayoutAmounts:" + data.LastPayoutAmounts);
                payoutSegments.ShowSegmentByNumber(data.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            HasMedalUpdate = false;
            ////Debug.Log("Payout Finished");
        }
    }
}
