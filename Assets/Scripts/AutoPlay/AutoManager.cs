using ReelSpinGame_AutoPlay.AI;
using System;
using TMPro;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay
{
    public enum StopOrderID { First, Second, Third }                    // 停止順の識別化
    public enum StopOrderOptionName { LMR, LRM, MLR, MRL, RLM, RML }    // 停止順番のオプション名(左:L, 中:M, 右:R)
    public enum AutoSpeedName { Normal, Fast, Quick }                   // オート速度

    // 一定条件
    [Flags]
    public enum SpecificConditionFlag
    {
        None = 0,
        WinningPattern = 1 << 0,
        BIG = 1 << 1,
        REG = 1 << 2,
        EndBonus = 1 << 3,
    }

    // 回数条件
    public enum SpinTimeConditionName
    {
        None = 0,
        Spin1000G,
        Spin3000G,
        Spin5000G,
        Spin10000G,
    }

    // オートプレイ機能
    public class AutoManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI autoModeDisplayText;       // オートプレイ中のUI

        public AutoSpeedName CurrentSpeed { get; set; }             // 現在のオート速度
        public StopOrderOptionName CurrentStopOrder { get; set; }   // 現在の停止順
        public BigColor BigLineUpSymbol                             // 揃えるBIG図柄
        {
            get => autoAI.BigLineUpSymbol;
            set => autoAI.BigLineUpSymbol = value;
        }

        public bool HasTechnicalPlay                                // 技術介入の有無
        {
            get => autoAI.HasTechnicalPlay;
            set => autoAI.HasTechnicalPlay = value;
        }

        public byte EndConditionFlag { get; set; }                      // 終了条件
        public SpinTimeConditionName SpinTimeCondition { get; set; }    // 回転数条件

        public bool HasAuto { get; private set; }                       // オートがあるか
        public bool HasWaitingCancel { get; private set; }              // オート終了待機中か
        public bool HasStopPosDecided { get; private set; }             // 停止位置を決めたか
        public int RemainingAutoGames { get; private set; }             // 残りオート数
        public ReelID[] AutoStopOrders { get; private set; }            // 停止させるリールの順番
        public int[] AutoStopPos { get; private set; }                  // 各リールの停止位置

        private AutoPlayAI autoAI;                                      // オートのAI

        void Awake()
        {
            autoAI = new AutoPlayAI();
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoStopPos = new int[] { 0, 0, 0 };
            // オートプレイ中のUIを非表示に
            autoModeDisplayText.gameObject.SetActive(false);
        }

        // オート状態変更
        public void ChangeAutoMode()
        {
            // 速度が高速以上なら払い出しが終わるまではオートを切らない
            if (CurrentSpeed > (int)AutoSpeedName.Normal)
            {
                if (!HasAuto)
                {
                    HasAuto = true;
                }
                else
                {
                    HasWaitingCancel = true;
                }
            }
            // 速度が通常ならいつでも切れるようにする
            else
            {
                HasAuto = !HasAuto;
            }

            HasStopPosDecided = false;
            SetSpinTimes();
            autoModeDisplayText.gameObject.SetActive(HasAuto);
        }

        // 高速オート終了チェック
        public void CheckFastAutoCancelled()
        {
            if(HasWaitingCancel)
            {
                FinishAutoForce();
            }
        }

       // 残りオート回数を減らす
        public void DecreaseAutoSpin()
        {
            if(SpinTimeCondition != SpinTimeConditionName.None)
            {
                RemainingAutoGames -= 1;

                if(RemainingAutoGames == 0)
                {
                    FinishAutoForce();
                }
            }
        }

        // ボーナス条件によるオート終了チェック
        public void CheckAutoEndByBonus(int bonusTypeID)
        {
            // リーチ目出現の場合(ボーナス成立でも終了)
            if ((EndConditionFlag & (byte)SpecificConditionFlag.WinningPattern) == (byte)SpecificConditionFlag.WinningPattern
                && autoAI.HasStoppedWinningPattern)
            {
                FinishAutoForce();
            }
            // BIG当選
            else if ((EndConditionFlag & (byte)SpecificConditionFlag.BIG) == (byte)SpecificConditionFlag.BIG
                && bonusTypeID == (int)BonusTypeID.BonusBIG)
            {
                FinishAutoForce();
            }
            // REG当選
            else if ((EndConditionFlag & (byte)SpecificConditionFlag.REG) == (byte)SpecificConditionFlag.REG
                && bonusTypeID == (int)BonusTypeID.BonusREG)
            {
                FinishAutoForce();
            }
        }

        // ボーナス終了によるオート終了チェック
        public void CheckAutoEndByBonusFinish(int bonusStatusID)
        {
            // 通常時に戻った場合はオート終了
            if ((EndConditionFlag & (byte)SpecificConditionFlag.EndBonus) == (byte)SpecificConditionFlag.EndBonus
                && bonusStatusID == (int)BonusStatus.BonusNone)
            {
                FinishAutoForce();
            }
        }
        
        // オート停止位置リセット
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)StopOrderID.First] = 0;
            AutoStopOrders[(int)StopOrderID.Second] = 0;
            AutoStopOrders[(int)StopOrderID.Third] = 0;
        }

        // オート停止位置の設定
        public void SetAutoStopPos(AutoAIConditionClass condition)
        {
            // BIG中の場合、(JAC回数が残り1回, 残りゲーム数が9G以上)ならJACハズシをする
            if (condition.BonusStatus == BonusStatus.BonusBIGGames &&
                condition.BigChanceGames > 8 && condition.RemainingJacIn == 1)
            {
                CurrentStopOrder = StopOrderOptionName.RML;
            }
            // ボーナス成立後であれば左押しに固定する
            else if (condition.HoldingBonus != BonusTypeID.BonusNone)
            {
                CurrentStopOrder = StopOrderOptionName.LMR;
            }

            // 押し順設定
            SetAutoStopOrder();
            // リーチ目で止めるかの設定
            autoAI.HasWinningPatternStop = ((EndConditionFlag & (byte)SpecificConditionFlag.WinningPattern) 
                == (byte)SpecificConditionFlag.WinningPattern);
            // 停止位置決定
            AutoStopPos = autoAI.GetStopPos(condition);
            HasStopPosDecided = true;
        }

        // オートの強制終了
        void FinishAutoForce()
        {
            HasAuto = false;
            HasWaitingCancel = false;
            HasStopPosDecided = false;
            RemainingAutoGames = 0;
            autoAI.HasStoppedWinningPattern = false;
            autoModeDisplayText.gameObject.SetActive(false);
        }

        // オート押し順の設定反映
        void SetAutoStopOrder()
        {       
            switch(CurrentStopOrder)
            {
                case StopOrderOptionName.LMR:
                    ChangeAutoStopOrder(ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight);
                    break;

                case StopOrderOptionName.LRM:
                    ChangeAutoStopOrder(ReelID.ReelLeft, ReelID.ReelRight, ReelID.ReelMiddle);
                    break;

                case StopOrderOptionName.MLR:
                    ChangeAutoStopOrder(ReelID.ReelMiddle, ReelID.ReelLeft, ReelID.ReelRight);
                    break;

                case StopOrderOptionName.MRL:
                    ChangeAutoStopOrder(ReelID.ReelMiddle, ReelID.ReelRight, ReelID.ReelLeft);
                    break;

                case StopOrderOptionName.RLM:
                    ChangeAutoStopOrder(ReelID.ReelRight, ReelID.ReelLeft, ReelID.ReelMiddle);
                    break;

                case StopOrderOptionName.RML:
                    ChangeAutoStopOrder(ReelID.ReelRight, ReelID.ReelMiddle, ReelID.ReelLeft);
                    break;
            }
        }

        // オート押し順の変更
        void ChangeAutoStopOrder(ReelID first, ReelID second, ReelID third)
        {
            // 同じ押し順がないかチェック
            if(first != second && second != third)
            {
                AutoStopOrders[(int)StopOrderID.First] = first;
                AutoStopOrders[(int)StopOrderID.Second] = second;
                AutoStopOrders[(int)StopOrderID.Third] = third;
            }
        }

        // 回転数の反映
        void SetSpinTimes()
        {
            switch(SpinTimeCondition)
            {
                case SpinTimeConditionName.Spin1000G:
                    RemainingAutoGames = 1000;
                    break;

                case SpinTimeConditionName.Spin3000G:
                    RemainingAutoGames = 3000;
                    break;

                case SpinTimeConditionName.Spin5000G:
                    RemainingAutoGames = 5000;
                    break;

                case SpinTimeConditionName.Spin10000G:
                    RemainingAutoGames = 10000;
                    break;
                default:
                    RemainingAutoGames = 0;
                    break;
            }
        }
    }
}
