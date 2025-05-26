using System.Collections;
using UnityEngine;

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
        public MedalBehaviour Data { get; private set; }

        // メダルのUI
        [SerializeField] private MedalPanel medalPanel;

        // クレジットセグメント
        [SerializeField] private MedalSevenSegment creditSegments;
        // 払い出しセグメント
        [SerializeField] private MedalSevenSegment payoutSegments;

        // メダルの更新処理中か
        public bool HasMedalUpdate { get; private set; }
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
            Data = new MedalBehaviour(credits, curretMaxBet, lastBetAmounts, hasReplay);
            Debug.Log("Credits:" + credits);
            creditSegments.ShowSegmentByNumber(credits);
        }

        // タイマー処理の破棄
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // MAX_BET用の処理
        public void StartMAXBet()
        {
            Debug.Log("Received MAX_BET");
            StartBet(Data.MaxBetAmounts);
        }

        // ベット処理開始
        public void StartBet(int amounts)
        {
            // 処理ををしていないか、またはリプレイでないかチェック
            if (!Data.HasReplay && !HasMedalUpdate)
            {
                // 枚数を調整
                // 現在の枚数と違ったらベット(現在のMAX BETを超えていないこと, JAC中:1BET, 通常:3BET)
                if (amounts != Data.CurrentBet && amounts <= Data.MaxBetAmounts)
                {
                    // ベット枚数設定
                    Data.SetRemainingBet(amounts);
                    // メダルの投入を開始する(残りはフレーム処理)
                    StartCoroutine(nameof(UpdateInsert));
                }
                // ベットがすでに終わっている、またはMAXベットの場合(Debug)
                else
                {
                    if (amounts > Data.MaxBetAmounts)
                    {
                        Debug.Log("The MAX Bet is now :" + Data.MaxBetAmounts);
                    }
                    else
                    {
                        Debug.Log("You already Bet:" + amounts);
                    }
                }
            }

            // 処理中でメダルが入れられない場合
            else
            {
                if (Data.HasReplay)
                {
                    Debug.Log("Replay is enabled");
                }
                else
                {
                    Debug.Log("Insert is enabled");
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
                    Data.ChangePayoutAmounts(amounts);

                    StartCoroutine(nameof(UpdatePayout));
                }
                else
                {
                    Debug.Log("No Payouts");
                }
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }

        // リプレイ状態にする(前回と同じメダル枚数をかける)
        public void SetReplay()
        {
            Debug.Log("Enable Replay" + Data.LastBetAmounts);
            Data.EnableReplay();
            StartCoroutine(nameof(UpdateInsert));
        }

        // func
        // コルーチン用
        private IEnumerator UpdateInsert()
        {
            Debug.Log("StartBet");
            HasMedalUpdate = true;
            // 残りベット枚数がなくなるまで処理
            while (Data.RemainingBet > 0)
            {
                // メダル投入
                Data.InsertOneMedal();
                // ランプ、セグメント更新
                medalPanel.UpdateLampByBet(Data.CurrentBet, Data.LastBetAmounts);
                // クレジット更新
                creditSegments.ShowSegmentByNumber(Data.Credits);
                // 払い出しセグメントを消す
                payoutSegments.TurnOffAllSegments();
                // 0.12秒待機
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
            Debug.Log("Bet Finished");
            Debug.Log("CurrentBet:" + Data.CurrentBet);
        }

        private IEnumerator UpdatePayout()
        {
            HasMedalUpdate = true;
            // 払い出し処理
            while (Data.PayoutAmounts > 0)
            {
                // メダル払い出し
                Data.PayoutOneMedal();
                HasMedalPayout.Invoke(1);
                // クレジットと払い出しセグメント更新
                creditSegments.ShowSegmentByNumber(Data.Credits);
                Debug.Log("LastPayoutAmounts:" + Data.LastPayoutAmounts);
                payoutSegments.ShowSegmentByNumber(Data.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // 全て払い出したら処理終了
            HasMedalUpdate = false;
            Debug.Log("Payout Finished");
        }
    }
}
