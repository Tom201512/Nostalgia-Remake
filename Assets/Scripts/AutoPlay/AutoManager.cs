using ReelSpinGame_AutoPlay.AI;
using UnityEngine;
using System;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using TMPro;

namespace ReelSpinGame_AutoPlay
{
    // オートプレイ機能
    public class AutoManager : MonoBehaviour
    {
        // const
        public enum AutoStopOrder { First, Second, Third }                          // オート押し順の識別用
        public enum AutoStopOrderOptions { LMR, LRM, MLR, MRL, RLM, RML }           // オート押し順指定用(左:L, 中:M, 右:R))
        public enum AutoPlaySpeed { Normal, Fast, Quick }                           // オート速度(通常、高速、超高速
        public enum AutoPlaySequence { AutoInsert, AutoStopReel }                   // 現在のオートモード
        // オート終了条件で使う条件ID

        // 一定条件のフラグ
        [Flags]
        public enum AutoSpecificConditionID
        {
            None = 0,
            WinningPattern = 1 << 0,
            BIG = 1 << 1,
            REG = 1 << 2,
            EndBonus = 1 << 3,
        }

        // 回数条件のフラグ
        public enum AutoSpinTimeConditionID
        {
            None = 0,
            Spin1000G,
            Spin3000G,
            Spin5000G,
            Spin10000G,
        }

        // var
        [SerializeField] TextMeshProUGUI autoModeText;              // オート中に表示するテキスト

        public bool HasAuto { get; private set; }                   // オートプレイ中か
        public int AutoSpeedID { get; private set; }                // オート速度
        public bool HasWaitingCancel { get; private set; }          // 高速オート解除待ちか
        public ReelID[] AutoStopOrders { get; private set; }        // オート時の押し順
        public int AutoOrderID { get; private set; }                // オート押し順のオプション数値
        public int[] AutoStopPos { get; private set; }              // オート時の停止位置
        public bool HasStopPosDecided { get; private set; }         // オートの停止位置を決めたか

        // 終了条件
        public byte CurrentEndCondition { get; private set; }                       // 終了条件のフラグ値
        public AutoSpinTimeConditionID SpinTimeConditionID { get; private set; }    // 回転終了条件のID
        public int RemainingAutoGames { get; private set; }                         // 残りオート回数

        private AutoPlayAI autoAI;      // オート時の停止位置選択AI

        void Awake()
        {
            autoAI = new AutoPlayAI();
            HasAuto = false;
            // 停止順番の配列作成(デフォルトは順押し)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoOrderID = (int)AutoStopOrderOptions.LMR;
            AutoSpeedID = (int)AutoPlaySpeed.Normal;
            RemainingAutoGames = 0;
            CurrentEndCondition = (int)AutoSpecificConditionID.None;
            SpinTimeConditionID = AutoSpinTimeConditionID.None;

            // 停止位置の配列作成
            AutoStopPos = new int[] { 0, 0, 0 };
            HasWaitingCancel = false;

            autoModeText.gameObject.SetActive(false);
        }


        // func
        // 各種数値取得
        public string GetOrderName() => Enum.ToObject(typeof(AutoStopOrderOptions), AutoOrderID).ToString();            // オート名を
        public string GetSpeedName() => Enum.ToObject(typeof(AutoPlaySpeed), AutoSpeedID).ToString();                   // スピード名

        // オート終了条件のバイナリ
        public string GetConditionID() => Convert.ToString(CurrentEndCondition, 16).ToUpper();

        public bool GetHasTechnicalPlay() => autoAI.HasTechnicalPlay;                                                   // 技術介入の有無
        public bool GetHasRiichiStop() => autoAI.HasWinningPatternStop;                                                 // リーチ目優先制御の有無
        public BigColor GetBigColorLineUP() => autoAI.PlayerSelectedBigColor;                                           // BIG CHANCE図柄の色
        public bool GetHasStoppedRiichiPtn() => autoAI.HasStoppedWinningPattern;                                        // リーチ目を止めたか

        // 設定値変更
        public void SetAutoSpeed(AutoPlaySpeed autoSpeed) => AutoSpeedID = (int)autoSpeed;                              // オート速度設定
        public void SetAutoOrder(AutoStopOrderOptions autoOrder) => AutoOrderID = (int)autoOrder;                       // オート押し順設定
        public void SetBigColorLineUp(BigColor color) => autoAI.PlayerSelectedBigColor = color;                         // オートBIG揃い色設定
        public void SetTechnicalPlay(bool isEnabled) => autoAI.HasTechnicalPlay = isEnabled;                            // 技術介入設定変更
        public void SetSpecificCondition(byte conditionBinary) => CurrentEndCondition = conditionBinary;                // 終了タイミング
        public void SetSpinTimes(AutoSpinTimeConditionID spinCondition) => SpinTimeConditionID = spinCondition;         // 回転数

        // オート機能の切り替え
        public void ChangeAutoMode()
        {
            // スピードモードが高速なら払い出しフェーズになるまで実行(処理の不具合を抑えるため)
            if (AutoSpeedID > (int)AutoPlaySpeed.Normal)
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
            // 通常スピード時はいつでも切れる
            else
            {
                HasAuto = !HasAuto;

            }

            // 停止位置リセット
            HasStopPosDecided = false;

            // 指定したオート終了条件を付与
            SetSpinTimes();
            // テキスト表示
            autoModeText.gameObject.SetActive(HasAuto);
        }

        // 高速オート終了チェック
        public void CheckFastAutoCancelled()
        {
            if(HasWaitingCancel)
            {
                FinishAutoForce();
            }
        }

       // 残りオートゲーム数チェック
        public void CheckRemainingAuto()
        {
            if(SpinTimeConditionID != AutoSpinTimeConditionID.None)
            {
                RemainingAutoGames -= 1;

                // 残りオートがなくなった場合は終了
                if(RemainingAutoGames == 0)
                {
                    FinishAutoForce();
                }
            }
        }

        // ボーナス条件によるオート終了チェック
        public void CheckAutoEndByBonus(int bonusTypeID)
        {
            // リーチ目出現時
            if ((CurrentEndCondition & (byte)AutoSpecificConditionID.WinningPattern) == (byte)AutoSpecificConditionID.WinningPattern
                && autoAI.HasStoppedWinningPattern)
            {
                FinishAutoForce();
            }
            // BIG当選
            else if ((CurrentEndCondition & (byte)AutoSpecificConditionID.BIG) == (byte)AutoSpecificConditionID.BIG
                && bonusTypeID == (int)BonusTypeID.BonusBIG)
            {
                FinishAutoForce();
            }
            // REG当選
            else if ((CurrentEndCondition & (byte)AutoSpecificConditionID.REG) == (byte)AutoSpecificConditionID.REG
                && bonusTypeID == (int)BonusTypeID.BonusREG)
            {
                FinishAutoForce();
            }
        }

        // ボーナス終了によるオート終了チェック
        public void CheckAutoEndByBonusFinish(int bonusStatusID)
        {
            // 通常時に戻った場合はオート終了
            if ((CurrentEndCondition & (byte)AutoSpecificConditionID.EndBonus) == (byte)AutoSpecificConditionID.EndBonus
                && bonusStatusID == (int)BonusStatus.BonusNone)
            {
                FinishAutoForce();
            }
        }

        // オートの強制終了
        public void FinishAutoForce()
        {
            HasAuto = false;
            HasWaitingCancel = false;
            HasStopPosDecided = false;
            RemainingAutoGames = 0;
            autoAI.HasStoppedWinningPattern = false;
            autoModeText.gameObject.SetActive(false);
        }
        
        // オート停止位置をリセット
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)AutoStopOrder.First] = 0;
            AutoStopOrders[(int)AutoStopOrder.Second] = 0;
            AutoStopOrders[(int)AutoStopOrder.Third] = 0;
        }

        // オート押し順をフラグ、条件から得る
        public void GetAutoStopPos(AutoAIConditionClass autoAICondition)
        {
            SetAutoStopOrder();
            AutoStopPos = autoAI.GetStopPos(autoAICondition);
            HasStopPosDecided = true;
        }

        // オート押し位置の変更
        public void ChangeAutoStopPos(int left, int middle, int right)
        {
            // 停止位置がオーバーフローしていないかチェック(0~20)
            if ((left < MaxReelArray && middle < MaxReelArray && right < MaxReelArray) &&
                (left >= 0 && middle >= 0 && right >= 0))
            {
                AutoStopPos[(int)ReelID.ReelLeft] = left;
                AutoStopPos[(int)ReelID.ReelMiddle] = middle;
                AutoStopPos[(int)ReelID.ReelRight] = right;
            }
        }

        // オート押し順の設定反映
        void SetAutoStopOrder()
        {       
            switch(AutoOrderID)
            {
                case (int)AutoStopOrderOptions.LMR:
                    ChangeAutoStopOrder(ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight);
                    break;

                case (int)AutoStopOrderOptions.LRM:
                    ChangeAutoStopOrder(ReelID.ReelLeft, ReelID.ReelRight, ReelID.ReelMiddle);
                    break;

                case (int)AutoStopOrderOptions.MLR:
                    ChangeAutoStopOrder(ReelID.ReelMiddle, ReelID.ReelLeft, ReelID.ReelRight);
                    break;

                case (int)AutoStopOrderOptions.MRL:
                    ChangeAutoStopOrder(ReelID.ReelMiddle, ReelID.ReelRight, ReelID.ReelLeft);
                    break;

                case (int)AutoStopOrderOptions.RLM:
                    ChangeAutoStopOrder(ReelID.ReelRight, ReelID.ReelLeft, ReelID.ReelMiddle);
                    break;

                case (int)AutoStopOrderOptions.RML:
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
                AutoStopOrders[(int)AutoStopOrder.First] = first;
                AutoStopOrders[(int)AutoStopOrder.Second] = second;
                AutoStopOrders[(int)AutoStopOrder.Third] = third;
            }
        }

        // 回転数の反映
        void SetSpinTimes()
        {
            switch(SpinTimeConditionID)
            {
                case AutoSpinTimeConditionID.Spin1000G:
                    RemainingAutoGames = 1000;
                    break;

                case AutoSpinTimeConditionID.Spin3000G:
                    RemainingAutoGames = 3000;
                    break;

                case AutoSpinTimeConditionID.Spin5000G:
                    RemainingAutoGames = 5000;
                    break;

                case AutoSpinTimeConditionID.Spin10000G:
                    RemainingAutoGames = 10000;
                    break;

                default:
                    RemainingAutoGames = 0;
                    break;
            }
        }
    }
}
