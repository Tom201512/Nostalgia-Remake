using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehaviour;

namespace ReelSpinGame_Medal
{
    //スロット内部のメダル管理
    public class MedalManager : MonoBehaviour
    {
        // const
        // メダル更新の間隔(ミリ秒)
        public const float MedalUpdateTime = 0.12f;

        // var
        // メダル処理のデータ
        private MedalBehaviour data;

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
        // メダルが払い出されたか
        public delegate void MedalHasPayoutEvent(int payout);
        public event MedalHasPayoutEvent HasMedalPayout;

        void Awake()
        {
            HasMedalUpdate = false;
        }

        // コンストラクタ
        public void SetMedalData(int credits, int curretMaxBet, int lastBetAmounts, bool hasReplay)
        {
            data = new MedalBehaviour(credits, curretMaxBet, lastBetAmounts, hasReplay);
            //////Debug.Log("Credits:" + credits);
            creditSegments.ShowSegmentByNumber(credits);
        }

        // タイマー処理の破棄
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // 数値を得る
        public int GetCredits() => data.Credits;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetPayoutAmounts() => data.PayoutAmounts;
        public int GetMaxBet() => data.MaxBetAmounts;
        public int GetLastBetAmounts() => data.LastBetAmounts;
        public int GetLastPayout() => data.LastPayoutAmounts;
        public bool GetBetFinished() => data.FinishedBet;
        public bool GetHasReplay() => data.HasReplay;

        // 数値を変える
        public int ChangeMaxBet(int amounts) => data.MaxBetAmounts = Math.Clamp(data.MaxBetAmounts + amounts, 0, MaxBetLimit);

        // MAX_BET用の処理
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.MaxBetAmounts);
        }

        // ベット処理開始
        public void StartBet(int amounts)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!data.HasReplay && !HasMedalUpdate)
            {
                // 枚数を調整
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)
                if (amounts != data.CurrentBet && amounts <= data.MaxBetAmounts)
                {
                    // ベット枚数設定
                    data.SetRemainingBet(amounts);
                    // メダルの投入を開始する(残りはフレーム処理)
                    StartCoroutine(nameof(UpdateInsert));
                }
                // ベットがすでに終わっている、またはMAXベットの場合(////Debug)
                else
                {
                    if (amounts > data.MaxBetAmounts)
                    {
                        ////Debug.Log("The MAX Bet is now :" + data.MaxBetAmounts);
                    }
                    else
                    {
                        ////Debug.Log("You already Bet:" + amounts);
                    }
                }
            }

            // 処理中でメダルが入れられない場合
            else
            {
                if (data.HasReplay)
                {
                    ////Debug.Log("Replay is enabled");
                }
                else
                {
                    ////Debug.Log("Insert is enabled");
                }
            }
        }

        // 払い出し開始
        public void StartPayout(int amounts)
        {
            // 払い出しをしていないかチェック
            if (!HasMedalUpdate)
            {
                // メダルの払い出しを開始する(残りはフレーム処理
                if (amounts > 0)
                {
                    data.PayoutAmounts = Math.Clamp(data.PayoutAmounts + amounts, 0, MaxPayout);
                    StartCoroutine(nameof(UpdatePayout));
                }
                else
                {
                    ////Debug.Log("No Payouts");
                }
            }
            else
            {
                ////Debug.Log("Payout is enabled");
            }
        }

        // リプレイ状態にする(前回と同じメダル枚数をかける)
        public void EnableReplay()
        {
            ////Debug.Log("Enable Replay" + data.LastBetAmounts);
            data.HasReplay = true;
            data.RemainingBet = data.LastBetAmounts;
        }

        // リプレイ状態を消す
        public void DisableReplay()
        {
            data.HasReplay = false;
            data.LastBetAmounts = 0;
        }

        // リプレイ投入を開始
        public void StartReplayInsert()
        {
            StartCoroutine(nameof(UpdateInsert));
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
                medalPanel.UpdateLampByBet(data.CurrentBet, data.LastBetAmounts);
                // クレジット更新
                creditSegments.ShowSegmentByNumber(data.Credits);
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
            while (data.PayoutAmounts > 0)
            {
                // メダル払い出し
                data.PayoutOneMedal();
                HasMedalPayout.Invoke(1);
                // クレジットと払い出しセグメント更新
                creditSegments.ShowSegmentByNumber(data.Credits);
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
