using ReelSpinGame_Interface;
using ReelSpinGame_Lamps;
using ReelSpinGame_Medal.Segment;
using ReelSpinGame_Save.Medal;
using System;
using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Medal
{
    // スロット内部のメダル管理
    public class MedalManager : MonoBehaviour, IHasSave<MedalSave>
    {
        public const float MedalUpdateTime = 0.12f;        // メダル更新の間隔(ミリ秒)

        [SerializeField] private MedalLampPanel medalLamp;              // メダルランプ部分
        [SerializeField] private MedalSevenSegment creditSegments;      // クレジットセグメント
        [SerializeField] private MedalSevenSegment payoutSegments;      // 払い出しセグメント

        // プロパティ部分
        public int Credit { get => model.Data.Credit; }                     // クレジット
        public int MaxBetAmount { get => model.Data.MaxBetAmount; }         // 最大ベット枚数
        public int LastBetAmount { get => model.Data.LastBetAmount; }       // 直前のベット
        public bool HasReplay { get => model.Data.HasReplay; }              // リプレイ

        public int CurrentBet { get => model.CurrentBet; }                  // ベット枚数
        public int RemainingBet { get => model.RemainingBet; }              // 残りベット
        public int RemainingPayout { get => model.RemainingPayout; }        // 残り払い出し
        public int LastPayoutAmount => model.LastPayoutAmount;              // 直前の払い出し
        public bool IsFinishedBet { get => model.IsFinishedBet; }           // 払出が終了したか

        public bool HasMedalUpdate { get; private set; }                // メダルの更新処理中か
        public bool HasSegmentUpdate { get; private set; }              // セグメントを更新中か

        public event Action MedalInsertedEvent;        // メダルが投入された時のイベント
        public event Action MedalPayoutEvent;          // メダルが払い出された時のイベント

        private MedalModel model;           // メダル処理のデータ
        private int previousCredit;         // 最後に投入した時のクレジット枚数

        private void Awake()
        {
            model = new MedalModel();
            HasMedalUpdate = false;
            HasSegmentUpdate = false;
            previousCredit = 0;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // セーブデータの作成
        public MedalSave MakeSaveData()
        {
            MedalSave save = new MedalSave();
            save.RecordData(model.Data);
            return save;
        }

        // セーブを読み込む
        public void LoadSaveData(MedalSave loadData)
        {
            model.Data.Credit = loadData.Credit;
            model.Data.MaxBetAmount = loadData.MaxBetAmount;
            model.Data.LastBetAmount = loadData.LastBetAmount;
            model.Data.HasReplay = loadData.HasReplay;
        }

        // MAXベット枚数変更
        public int ChangeMaxBet(int amount) => model.Data.MaxBetAmount = Math.Clamp(amount, 1, MedalModel.MaxBetLimit);

        // MAX_BET用の処理
        public void StartMAXBet() => StartBet(model.Data.MaxBetAmount, false);

        // クレジットを表示
        public void UpdateCreditSegment() => creditSegments.ShowSegmentByNumber(model.Data.Credit);

        // メダルベット枚数ランプを非表示にする
        public void DisableMedalBetLamp() => medalLamp.UpdateLampByBet(0, 0);

        // ベット処理開始
        public void StartBet(int amount, bool isFastAuto)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!model.Data.HasReplay && !HasMedalUpdate)
            {
                // 枚数を調整
                if (amount != model.CurrentBet && amount <= model.Data.MaxBetAmount)
                {
                    // ベット枚数設定
                    model.SetRemainingBet(amount);

                    // 払い出す前のクレジット枚数を記録
                    previousCredit = model.Data.Credit;

                    // 高速オートならデータ更新とセグメント処理を行う
                    if (isFastAuto)
                    {
                        model.CurrentBet = amount;
                        model.Data.Credit = Math.Clamp(model.Data.Credit -= amount, 0, MedalModel.MaxCredit);
                        model.IsFinishedBet = true;
                        // ランプ、セグメント更新
                        medalLamp.UpdateLampByBet(model.CurrentBet, model.Data.LastBetAmount);
                        creditSegments.ShowSegmentByNumber(model.Data.Credit);
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
                    model.RemainingPayout = Math.Clamp(amount, 0, MedalModel.MaxPayout);
                    // 払い出す前のクレジットを記録th
                    previousCredit = model.Data.Credit;
                    // クレジットの増加
                    model.ChangeCredit(model.RemainingPayout);

                    // 高速オートならデータ更新とセグメント処理を行う
                    if (isFastAuto)
                    {
                        model.LastPayoutAmount = model.RemainingPayout;
                        model.RemainingPayout = 0;
                        // クレジットと払い出しセグメント更新
                        creditSegments.ShowSegmentByNumber(model.Data.Credit);
                        payoutSegments.ShowSegmentByNumber(model.LastPayoutAmount);
                        HasMedalUpdate = false;
                    }
                }
            }
        }

        // リプレイ状態にする(前回と同じメダル枚数をかける)
        public void EnableReplay()
        {
            model.Data.HasReplay = true;
            model.RemainingBet = model.Data.LastBetAmount;
        }

        // リプレイ状態を消す
        public void DisableReplay() => model.Data.HasReplay = false;

        // リプレイ投入を開始
        public void StartReplayInsert(bool hasCoroutineCut)
        {
            // コルーチンを無視する場合
            if (hasCoroutineCut)
            {
                // ベット枚数設定
                model.CurrentBet = model.Data.LastBetAmount;
                model.IsFinishedBet = true;
                // ランプ、セグメント更新
                medalLamp.UpdateLampByBet(model.CurrentBet, model.Data.LastBetAmount);
                creditSegments.ShowSegmentByNumber(model.Data.Credit);
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
            model.CurrentBet = 0;
            model.IsFinishedBet = false;
        }

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
            if (!HasMedalUpdate)
            {
                StartCoroutine(nameof(UpdatePayoutSegment));
            }
        }

        // コルーチン用
        // 投入時のセグメント更新
        private IEnumerator UpdateInsertSegment()
        {
            HasMedalUpdate = true;
            payoutSegments.TurnOffAllSegments();

            // 残りベット枚数がなくなるまで処理
            while (model.RemainingBet > 0)
            {
                // メダル投入
                MedalInsertedEvent?.Invoke();

                model.InsertOneMedal();
                medalLamp.UpdateLampByBet(model.CurrentBet, model.Data.LastBetAmount);

                // リプレイ以外の時のみクレジットセグメントを更新
                if (!HasReplay)
                {
                    creditSegments.ShowSegmentByNumber(model.Data.Credit);
                }

                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
        }

        // 払い出しのセグメント更新
        private IEnumerator UpdatePayoutSegment()
        {
            HasMedalUpdate = true;
            int currentPayout = 0;

            // 払い出し処理
            while (model.RemainingPayout > 0)
            {
                currentPayout += 1;
                MedalPayoutEvent?.Invoke();

                // メダル払い出し
                model.PayoutOneMedal();
                // セグメント更新処理
                creditSegments.ShowSegmentByNumber(Math.Clamp(previousCredit + currentPayout, 0, MedalModel.MaxCredit));
                payoutSegments.ShowSegmentByNumber(currentPayout);
                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            HasMedalUpdate = false;
        }
    }
}
