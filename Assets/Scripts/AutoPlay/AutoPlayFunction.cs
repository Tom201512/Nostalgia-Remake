using ReelSpinGame_AutoPlay.AI;
using System;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_AutoPlay
{
    public class AutoPlayFunction
    {
        // オートプレイ機能

        // const
        // オート押し順の識別用
        public enum AutoStopOrder { First, Second, Third }
        // オート押し順指定用(左:L, 中:M, 右:R)
        public enum AutoStopOrderOptions { LMR, LRM, MLR, MRL, RLM, RML }
        // オート速度(通常、高速、超高速)
        public enum AutoPlaySpeed { Normal, Fast, Quick }
        // 現在のオートモード
        public enum AutoPlaySequence { AutoInsert, AutoStopReel }
        // オート終了条件で使う条件ID
        // 条件(無制限, BIG成立, REG成立, ボーナス成立, ボーナス終了(どちらか), リーチ目, 指定ゲーム数消費)
        public enum AutoEndConditionID { None, BIG, REG, AnyBonus, EndBonus, RiichiPattern, Games }

        // var
        // オートプレイ中か
        public bool HasAuto { get; private set; }
        // オート速度
        public int AutoSpeedID { get; private set; }
        // 高速オート解除待ちか
        public bool HasWaitingCancel { get; private set; }
        // オート時の押し順
        public ReelID[] AutoStopOrders { get; private set; }
        // オート押し順のオプション数値
        public int AutoOrderID { get; private set; }
        // オート時の停止位置
        public int[] AutoStopPos { get; private set; }
        // オートの停止位置を決めたか
        public bool HasStopPosDecided { get; private set; }

        // 終了条件
        // 残りオート回数
        public int RemainingAutoGames { get; private set; }
        // 終了条件
        public int autoEndConditionID { get; private set; }

        // オート時の停止位置選択AI
        private AutoPlayAI autoAI;

        // コンストラクタ
        public AutoPlayFunction()
        {
            autoAI = new AutoPlayAI();
            HasAuto = false;
            // 停止順番の配列作成(デフォルトは順押し)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoOrderID = (int)AutoStopOrderOptions.LMR;
            AutoSpeedID = (int)AutoPlaySpeed.Normal;
            RemainingAutoGames = 0;
            autoEndConditionID = (int)AutoEndConditionID.None;

            // 停止位置の配列作成
            AutoStopPos = new int[] { 0, 0, 0 };
            HasWaitingCancel = false;
        }

        // func

        // オート名を返す
        public string GetOrderName() => Enum.ToObject(typeof(AutoStopOrderOptions), AutoOrderID).ToString();

        // スピード名を返す
        public string GetSpeedName() => Enum.ToObject(typeof(AutoPlaySpeed), AutoSpeedID).ToString();

        // オート終了条件名を返す
        public string GetConditionName() => Enum.ToObject(typeof(AutoEndConditionID), autoEndConditionID).ToString();

        // 技術介入の有無を返す
        public bool GetHasTechnicalPlay() => autoAI.HasTechnicalPlay;

        // リーチ目優先制御の有無を返す
        public bool GetHasRiichiStop() => autoAI.HasRiichiStop;

        // リーチ目を止めたかを返す
        public bool GetHasStoppedRiichiPtn() => autoAI.HasStoppedRiichiPtn;

        // 1枚掛けボーナス揃えの有無を返す
        public bool GetHasOneBetBonusLineUp() => autoAI.HasOneBetBonusLineUp;

        // オート仕様番号の変更(デバッグ用)
        public void ChangeAutoOrder()
        {
            if (!HasAuto)
            {
                if (AutoOrderID + 1 > (int)AutoStopOrderOptions.RML)
                {
                    AutoOrderID = (int)AutoStopOrderOptions.LMR;
                }
                else
                {
                    AutoOrderID += 1;
                }
            }
        }

        // オートスピード番号変更(デバッグ用)
        public void ChangeAutoSpeed()
        {
            if (!HasAuto)
            {
                if (AutoSpeedID + 1 > (int)AutoPlaySpeed.Quick)
                {
                    AutoSpeedID = (int)AutoPlaySpeed.Normal;
                }
                else
                {
                    AutoSpeedID += 1;
                }
            }
        }

        // オートBIG揃い色選択
        public void ChangeBigLineUpColor(BigColor color) => autoAI.PlayerSelectedBigColor = color;

        // オート機能の切り替え
        public void ChangeAutoMode(AutoEndConditionID conditionID, int conditionGames, bool hasTechnicalPlay, 
            bool hasRiichiStop, bool hasOneBetBonusLineUp, BigColor favoriteBIGColor)
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
            autoEndConditionID = (int)conditionID;
            RemainingAutoGames = conditionGames;

            // AI設定
            // 技術介入
            autoAI.HasTechnicalPlay = hasTechnicalPlay;

            // 技術介入がある場合はリーチ目で止める設定、1枚掛けで揃える設定などをする
            if (hasTechnicalPlay)
            {
                autoAI.HasRiichiStop = (autoEndConditionID == (int)AutoEndConditionID.RiichiPattern || hasRiichiStop);
                autoAI.HasStoppedRiichiPtn = false;
                // ボーナスは1枚掛けで揃えさせるか(リーチ目設定が出てる場合のみ有効)
                autoAI.HasOneBetBonusLineUp = hasRiichiStop && hasOneBetBonusLineUp;
            }
            else
            {
                autoAI.HasRiichiStop = false;
                autoAI.HasStoppedRiichiPtn = false;
                autoAI.HasOneBetBonusLineUp = false;
            }

            // 狙うBIGの色設定
            autoAI.PlayerSelectedBigColor = favoriteBIGColor;
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
            if(autoEndConditionID == (int)AutoEndConditionID.Games)
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
            switch(autoEndConditionID)
            {
                // BIG当選
                case (int)AutoEndConditionID.BIG:

                    if(bonusTypeID == (int)BonusTypeID.BonusBIG)
                    {
                        FinishAutoForce();
                    }
                    break;

                // REG当選
                case (int)AutoEndConditionID.REG:
                    if (bonusTypeID == (int)BonusTypeID.BonusREG)
                    {
                        FinishAutoForce();
                    }
                    break;

                // どちらか当選
                case (int)AutoEndConditionID.AnyBonus:
                    if (bonusTypeID == (int)BonusTypeID.BonusBIG || bonusTypeID == (int)BonusTypeID.BonusREG)
                    {
                        FinishAutoForce();
                    }
                    break;

                // リーチ目
                case (int)AutoEndConditionID.RiichiPattern:
                    if(autoAI.HasStoppedRiichiPtn)
                    {
                        FinishAutoForce();
                    }
                    break;
            }
        }

        // ボーナス終了によるオート終了チェック
        public void CheckAutoEndByBonusFinish(int bonusStatusID)
        {
            // 通常時に戻った場合はオート終了
            if(autoEndConditionID == (int)AutoEndConditionID.EndBonus && bonusStatusID == (int)BonusStatus.BonusNone)
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
            autoAI.HasStoppedRiichiPtn = false;
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
        public void GetAutoStopPos(FlagId flag, BonusTypeID holdingBonus, int bigChanceGames, int remainingJac, int betAmount)
        {
            SetAutoStopOrder();
            AutoStopPos = autoAI.GetStopPos(flag, AutoStopOrders[(int)AutoStopOrder.First], holdingBonus, bigChanceGames, remainingJac, betAmount);
            HasStopPosDecided = true;
        }

        // オート押し順の設定反映
        private void SetAutoStopOrder()
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
        private void ChangeAutoStopOrder(ReelID first, ReelID second, ReelID third)
        {
            // 同じ押し順がないかチェック
            if(first != second && second != third)
            {
                AutoStopOrders[(int)AutoStopOrder.First] = first;
                AutoStopOrders[(int)AutoStopOrder.Second] = second;
                AutoStopOrders[(int)AutoStopOrder.Third] = third;
            }
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
    }
}
