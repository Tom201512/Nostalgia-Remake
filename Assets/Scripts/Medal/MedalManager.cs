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
        // セグメントを更新中か
        public bool HasSegmentUpdate { get; private set; }
        // メダルが投入されたか
        public delegate void MedalHasInsertEvent();
        public event MedalHasInsertEvent HasMedalInsert;

        void Awake()
        {
            data = new MedalBehavior();
            HasMedalUpdate = false;
            HasSegmentUpdate = false;
        }

        void Start()
        {
            // クレジット更新
            creditSegments.ShowSegmentByNumber(data.system.Credit);
        }

        // タイマー処理の破棄
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // 数値を得る
        public int GetCredit() => data.system.Credit;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetRemainingPayout() => data.RemainingPayout;
        public int GetMaxBet() => data.system.MaxBetAmount;
        public int GetLastBetAmount() => data.system.LastBetAmount;
        public int GetLastPayout() => data.LastPayoutAmount;
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

                data.system.Credit = save.Credit;
                data.system.MaxBetAmount = save.MaxBetAmount;
                data.system.LastBetAmount = save.LastBetAmount;
                data.system.HasReplay = save.HasReplay;
            }
            else
            {
                throw new Exception("Loaded data is not MedalData");
            }
        }

        // MAXベット枚数変更
        public int ChangeMaxBet(int amount) => data.system.MaxBetAmount = Math.Clamp(amount, 0, MaxBetLimit);

        // 払い出しセグメント更新を開始する
        public void StartSegmentUpdate() => HasSegmentUpdate = true;

        // MAX_BET用の処理
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.system.MaxBetAmount, false);
        }

        // ベット処理開始
        public void StartBet(int amount, bool cutCoroutine)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!data.system.HasReplay && !HasMedalUpdate)
            {
                // 枚数を調整
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)
                if (amount != data.CurrentBet && amount <= data.system.MaxBetAmount)
                {
                    // ベット枚数設定
                    data.SetRemainingBet(amount);

                    // コルーチンを無視する場合
                    if (cutCoroutine)
                    {
                        data.CurrentBet = amount;
                        data.system.Credit = Math.Clamp(data.system.Credit -= amount, MinCredit, MaxCredit);
                        data.FinishedBet = true;

                        // ランプ、セグメント更新
                        medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmount);
                        // クレジット更新
                        creditSegments.ShowSegmentByNumber(data.system.Credit);
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
        public void StartPayout(int amount, bool cutCoroutine)
        {
            // 払い出しをしていないかチェック
            if (!HasMedalUpdate)
            {
                // メダルの払い出しを開始する(メダルが増えるのは演出で、データ上ではすでに増えている)
                if (amount > 0)
                {
                    // 払い出し枚数の設定
                    data.RemainingPayout = Math.Clamp(amount, 0, MaxPayout);
                    // クレジットの増加
                    data.ChangeCredit(data.RemainingPayout);

                    // コルーチンを無視する場合
                    if (cutCoroutine)
                    {
                        data.LastPayoutAmount = data.RemainingPayout;
                        data.RemainingPayout = 0;
                        // クレジットと払い出しセグメント更新
                        creditSegments.ShowSegmentByNumber(data.system.Credit);
                        payoutSegments.ShowSegmentByNumber(data.LastPayoutAmount);
                        HasMedalUpdate = false;
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
            data.RemainingBet = data.system.LastBetAmount;
        }

        // リプレイ状態を消す
        public void DisableReplay() => data.system.HasReplay = false;

        // リプレイ投入を開始
        public void StartReplayInsert(bool hasCoroutineCut)
        {
            // コルーチンを無視する場合
            if(hasCoroutineCut)
            {
                // ベット枚数設定
                data.CurrentBet = data.system.LastBetAmount;
                data.FinishedBet = true;
                // ランプ、セグメント更新
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmount);
                // クレジット更新
                creditSegments.ShowSegmentByNumber(data.system.Credit);
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
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmount);
                // クレジット更新
                creditSegments.ShowSegmentByNumber(data.system.Credit);
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

            // セグメント更新が入るまで待機
            while(!HasSegmentUpdate)
            {
                yield return new WaitForEndOfFrame();
            }

            // 払い出し処理
            while (data.RemainingPayout > 0)
            {
                // メダル払い出し
                data.PayoutOneMedal();
                //HasMedalPayout.Invoke(1);
                // クレジットと払い出しセグメント更新
                creditSegments.ShowSegmentByNumber(data.system.Credit - data.RemainingPayout);
                ////Debug.Log("LastPayoutAmount:" + data.LastPayoutAmount);
                payoutSegments.ShowSegmentByNumber(data.LastPayoutAmount);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            HasMedalUpdate = false;
            ////Debug.Log("Payout Finished");
        }
    }
}
