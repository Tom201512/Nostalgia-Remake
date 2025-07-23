using ReelSpinGame_Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Medal
{
    //スロット内部のメダル管理
    public class MedalManager : MonoBehaviour, ISavable
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

        // メダル情報のセット

        /*
        public void SetMedalData(MedalSystemSave medalSystemSave)
        {
            data.MedalSave = medalSystemSave;
            Debug.Log("Credits:" + data.Credits);
            Debug.Log("MaxBet:" + data.MaxBetAmounts);
            Debug.Log("LastBet:" + data.LastBetAmounts);
            Debug.Log("HasReplay:" + data.HasReplay);

            if(data.HasReplay)
            {
                EnableReplay();
            }
            creditSegments.ShowSegmentByNumber(data.Credits);
        }*/

        // タイマー処理の破棄
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // 数値を得る
        public int GetCredits() => data.Credits;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetRemainingPayouts() => data.RemainingPayouts;
        public int GetMaxBet() => data.MaxBetAmounts;
        public int GetLastBetAmounts() => data.LastBetAmounts;
        public int GetLastPayout() => data.LastPayoutAmounts;
        public bool GetBetFinished() => data.FinishedBet;
        public bool GetHasReplay() => data.HasReplay;

        // 数値を変える
        public int ChangeMaxBet(int amounts) => data.MaxBetAmounts = Math.Clamp(amounts, 0, MaxBetLimit);

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
                // メダルの払い出しを開始する(メダルが増えるのは演出で、データ上ではすでに増えている)
                if (amounts > 0)
                {
                    // 払い出し枚数の設定
                    data.RemainingPayouts = Math.Clamp(data.RemainingPayouts + amounts, 0, MaxPayout);
                    // クレジットの増加
                    data.ChangeCredits(amounts);

                    // 払い出し開始
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
            Debug.Log("Enable Replay" + data.LastBetAmounts);
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
            while (data.RemainingPayouts > 0)
            {
                // メダル払い出し
                data.PayoutOneMedal();
                //HasMedalPayout.Invoke(1);
                // クレジットと払い出しセグメント更新
                creditSegments.ShowSegmentByNumber(data.Credits - data.RemainingPayouts);
                ////Debug.Log("LastPayoutAmounts:" + data.LastPayoutAmounts);
                payoutSegments.ShowSegmentByNumber(data.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            HasMedalUpdate = false;
            ////Debug.Log("Payout Finished");
        }

        // セーブ
        public List<int> SaveData()
        {
            // メダル情報は(クレジット数、最高で掛けられる枚数、最後に掛けた枚数、リプレイの有無を記録)

            // 変数を格納
            List<int> data = new List<int>();
            data.Add(this.data.Credits);
            data.Add(this.data.MaxBetAmounts);
            data.Add(this.data.LastBetAmounts);
            data.Add(this.data.HasReplay ? 1 : 0);

            return data;
        }

        // 読み込み
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // クレジット枚数
                data.Credits = bStream.ReadInt32();
                Debug.Log("Credits:" + data.Credits);

                // 最大ベット枚数
                data.MaxBetAmounts = bStream.ReadInt32();
                Debug.Log("MaxBetAmounts:" + data.MaxBetAmounts);

                // 最後に掛けた枚数
                data.LastBetAmounts = bStream.ReadInt32();
                Debug.Log("LastBetAmounts:" + data.LastBetAmounts);

                // リプレイの有無
                data.HasReplay = (bStream.ReadInt32() == 1 ? true : false);
                Debug.Log("HasReplay:" + data.HasReplay);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Debug.Log("MedalSystem Loaded");
            }

            return true;
        }
    }
}
