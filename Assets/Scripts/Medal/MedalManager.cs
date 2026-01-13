using ReelSpinGame_Interface;
using ReelSpinGame_Lamps;
using ReelSpinGame_Medal.Segment;
using ReelSpinGame_Save.Medal;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Medal
{
    //スロット内部のメダル管理
    public class MedalManager : MonoBehaviour, IHasSave<MedalSave>
    {
        public const float MedalUpdateTime = 0.12f;        // メダル更新の間隔(ミリ秒)

        [SerializeField] private MedalPanel medalPanel;                 // メダルのUI
        [SerializeField] private MedalSevenSegment creditSegments;      // クレジットセグメント
        [SerializeField] private MedalSevenSegment payoutSegments;      // 払い出しセグメント

        public bool HasMedalUpdate { get; private set; }            // メダルの更新処理中か
        public bool HasSegmentUpdate { get; private set; }          // セグメントを更新中か

        // プロパティ部分
        public int Credit { get => data.MedalSystem.Credit; }                // クレジット
        public int CurrentBet { get => data.CurrentBet; }               // ベット枚数
        public int RemainingBet { get => data.RemainingBet; }           // 残りベット
        public int RemainingPayout { get => data.RemainingPayout; }     // 残り払い出し
        public int MaxBetAmount { get => data.MedalSystem.MaxBetAmount; }    // 最大ベット枚数
        public int LastBetAmount { get => data.MedalSystem.LastBetAmount; }  // 直前のベット
        public int LastPayoutAmount => data.LastPayoutAmount;           // 直前の払い出し
        public bool IsFinishedBet { get => data.IsFinishedBet; }        // 払出が終了したか
        public bool HasReplay { get => data.MedalSystem.HasReplay; }         // リプレイ

        // メダルが投入された時のイベント
        public delegate void MedalHasInsertEvent();
        public event MedalHasInsertEvent HasMedalInsertEvent;

        private MedalBehavior data;        // メダル処理のデータ

        private void Awake()
        {
            data = new MedalBehavior();
            HasMedalUpdate = false;
            HasSegmentUpdate = false;
        }

        private void Start()
        {
            creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // セーブデータにする
        public MedalSave MakeSaveData()
        {
            MedalSave save = new MedalSave();
            save.RecordData(data.MedalSystem);
            return save;
        }

        // セーブを読み込む
        public void LoadSaveData(MedalSave loadData)
        {
            data.MedalSystem.Credit = loadData.Credit;
            data.MedalSystem.MaxBetAmount = loadData.MaxBetAmount;
            data.MedalSystem.LastBetAmount = loadData.LastBetAmount;
            data.MedalSystem.HasReplay = loadData.HasReplay;
        }

        // MAXベット枚数変更
        public int ChangeMaxBet(int amount) => data.MedalSystem.MaxBetAmount = Math.Clamp(amount, 0, MaxBetLimit);

        // 払い出しセグメント更新を開始する
        public void ChangeSegmentUpdate(bool value) => HasSegmentUpdate = value;

        // MAX_BET用の処理
        public void StartMAXBet() => StartBet(data.MedalSystem.MaxBetAmount, false);

        // ベット処理開始
        public void StartBet(int amount, bool cutCoroutine)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!data.MedalSystem.HasReplay && !HasMedalUpdate)
            {
                // 枚数を調整
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)
                if (amount != data.CurrentBet && amount <= data.MedalSystem.MaxBetAmount)
                {
                    // ベット枚数設定
                    data.SetRemainingBet(amount);

                    // 払い出す前のクレジット枚数を記録。負数は切り捨て
                    int previousCredit = Math.Clamp(data.MedalSystem.Credit, 0, MaxCredit);

                    // コルーチンを無視する場合
                    if (cutCoroutine)
                    {
                        data.CurrentBet = amount;
                        data.MedalSystem.Credit = Math.Clamp(data.MedalSystem.Credit -= amount, MinCredit, MaxCredit);
                        data.IsFinishedBet = true;

                        // ランプ、セグメント更新
                        medalPanel.UpdateLampByBet(data.CurrentBet, data.MedalSystem.LastBetAmount);
                        creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);
                        payoutSegments.TurnOffAllSegments();
                    }
                    else
                    {
                        StartInsertTween(previousCredit, data.RemainingBet);
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
                    // 払い出す前と払い出し後のクレジット枚数を記録
                    int previousCredit = Math.Clamp(data.MedalSystem.Credit, 0, MaxCredit);
                    int toCredit = Math.Clamp(previousCredit + data.RemainingPayout, 0, MaxCredit);

                    // クレジットの増加
                    data.ChangeCredit(data.RemainingPayout);

                    // コルーチンを無視する場合
                    if (cutCoroutine)
                    {
                        data.LastPayoutAmount = data.RemainingPayout;
                        data.RemainingPayout = 0;
                        // クレジットと払い出しセグメント更新
                        creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);
                        payoutSegments.ShowSegmentByNumber(data.LastPayoutAmount);
                        HasMedalUpdate = false;
                    }
                    else
                    {
                        StartPayoutTween(previousCredit, toCredit, data.RemainingPayout);
                    }
                }
            }
        }

        // リプレイ状態にする(前回と同じメダル枚数をかける)
        public void EnableReplay()
        {
            data.MedalSystem.HasReplay = true;
            data.RemainingBet = data.MedalSystem.LastBetAmount;
        }

        // リプレイ状態を消す
        public void DisableReplay() => data.MedalSystem.HasReplay = false;

        // リプレイ投入を開始
        public void StartReplayInsert(bool hasCoroutineCut)
        {
            // コルーチンを無視する場合
            if (hasCoroutineCut)
            {
                // ベット枚数設定
                data.CurrentBet = data.MedalSystem.LastBetAmount;
                data.IsFinishedBet = true;
                // ランプ、セグメント更新
                medalPanel.UpdateLampByBet(data.CurrentBet, data.MedalSystem.LastBetAmount);
                creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);
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
            data.IsFinishedBet = false;
        }

        // メダル投入のセグメント更新を行う
        void StartInsertTween(int previousCredit, int betAmount)
        {
            // メダルの投入を開始する(残りはフレーム処理)
            StartCoroutine(nameof(UpdateInsert));
            creditSegments.DoSegmentTween(previousCredit, previousCredit - betAmount);
            payoutSegments.TurnOffAllSegments();
        }

        // メダル払い出し時のセグメント更新を行う
        void StartPayoutTween(int previousCredit, int toCredit, int payoutAmount)
        {
            StartCoroutine(nameof(UpdatePayout));
            creditSegments.DoSegmentTween(previousCredit, toCredit);
            payoutSegments.DoSegmentTween(0, payoutAmount);
        }

        // コルーチン用
        private IEnumerator UpdateInsert()
        {
            HasMedalUpdate = true;
            // 残りベット枚数がなくなるまで処理
            while (data.RemainingBet > 0)
            {
                // メダル投入
                data.InsertOneMedal();
                HasMedalInsertEvent?.Invoke();
                medalPanel.UpdateLampByBet(data.CurrentBet, data.MedalSystem.LastBetAmount);
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
        }

        private IEnumerator UpdatePayout()
        {
            HasMedalUpdate = true;

            // セグメント更新が入るまで待機
            while (!HasSegmentUpdate)
            {
                yield return new WaitForEndOfFrame();
            }

            // 払い出し処理
            while (data.RemainingPayout > 0)
            {
                // メダル払い出し
                data.PayoutOneMedal();
                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            HasMedalUpdate = false;
        }
    }
}
