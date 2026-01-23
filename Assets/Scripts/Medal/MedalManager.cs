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

        private MedalBehavior data;         // メダル処理のデータ
        private int tweenFromCredit;        // 補完処理前のクレジット数値
        private int tweenToCredit;          // 補完処理後のクレジット数値

        private void Awake()
        {
            data = new MedalBehavior();
            HasMedalUpdate = false;
            HasSegmentUpdate = false;

            tweenFromCredit = 0;
            tweenToCredit = 0;
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

        // 投入時のセグメント更新を開始する
        public void StartInsertSegmentUpdate()
        {
            if (!HasMedalUpdate)
            {
                StartCoroutine(nameof(UpdateInsertSegment));
            }
        }

        // 払い出しセグメント更新を開始する
        public void StartPayoutSegmentUpdate()
        {
            if(!HasMedalUpdate)
            {
                StartCoroutine(nameof(UpdatePayoutSegment));
            }
        }

        // MAX_BET用の処理
        public void StartMAXBet() => StartBet(data.MedalSystem.MaxBetAmount, false);

        // クレジットを表示
        public void UpdateCreditSegment() => creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);

        // ベット処理開始
        public void StartBet(int amount, bool isFastAuto)
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
                    tweenFromCredit = Math.Clamp(data.MedalSystem.Credit, 0, MaxCredit);
                    tweenToCredit = tweenFromCredit - data.RemainingBet;

                    // 高速オートならセグメント処理を行う
                    if (isFastAuto)
                    {
                        data.CurrentBet = amount;
                        data.MedalSystem.Credit = Math.Clamp(data.MedalSystem.Credit -= amount, MinCredit, MaxCredit);
                        data.IsFinishedBet = true;

                        // ランプ、セグメント更新
                        medalPanel.UpdateLampByBet(data.CurrentBet, data.MedalSystem.LastBetAmount);
                        creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);
                        payoutSegments.TurnOffAllSegments();
                    }
                }
            }
        }

        // 払い出し開始
        public void StartPayout(int amount, bool isFastAuto)
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
                    tweenFromCredit = Math.Clamp(data.MedalSystem.Credit, 0, MaxCredit);
                    tweenToCredit = Math.Clamp(tweenFromCredit + data.RemainingPayout, 0, MaxCredit);

                    // クレジットの増加
                    data.ChangeCredit(data.RemainingPayout);

                    // 高速オートならセグメント処理を行う
                    if (isFastAuto)
                    {
                        data.LastPayoutAmount = data.RemainingPayout;
                        data.RemainingPayout = 0;
                        // クレジットと払い出しセグメント更新
                        creditSegments.ShowSegmentByNumber(data.MedalSystem.Credit);
                        payoutSegments.ShowSegmentByNumber(data.LastPayoutAmount);
                        HasMedalUpdate = false;
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
                StartCoroutine(nameof(UpdateInsertSegment));
            }
        }

        // メダル処理終了
        public void FinishMedalInsert()
        {
            data.CurrentBet = 0;
            data.IsFinishedBet = false;
        }

        // コルーチン用
        // 投入時のセグメント更新
        private IEnumerator UpdateInsertSegment()
        {
            HasMedalUpdate = true;
            payoutSegments.TurnOffAllSegments();
            // リプレイ以外の時のみクレジットを更新
            if (!HasReplay)
            {
                creditSegments.DoSegmentTween(tweenFromCredit, tweenToCredit);
            }

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

        // 払い出しのセグメント更新
        private IEnumerator UpdatePayoutSegment()
        {
            HasMedalUpdate = true;

            // セグメント更新処理
            creditSegments.DoSegmentTween(tweenFromCredit, tweenToCredit);
            payoutSegments.DoSegmentTween(0, data.RemainingPayout);

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
