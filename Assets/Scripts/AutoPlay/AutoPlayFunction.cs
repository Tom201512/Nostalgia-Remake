using ReelSpinGame_AutoPlay.AI;
using System;
using UnityEngine;
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
        public enum AutoStopOrder { First, Second, Third}
        // オート押し順指定用(左:L, 中:M, 右:R)
        public enum AutoStopOrderOptions { LMR, LRM, MLR, MRL, RLM, RML}
        // オート速度(通常、高速、超高速)
        public enum AutoPlaySpeed { Normal, Fast, Quick}
        // 現在のオートモード
        public enum AutoPlaySequence { AutoInsert, AutoStopReel}

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

            // 停止位置の配列作成(テスト用に0で止めるように)
            AutoStopPos = new int[] { 0, 0, 0 };
            HasWaitingCancel = false;
        }

        // func

        // オート名を返す
        public string GetOrderName() => Enum.ToObject(typeof(AutoStopOrderOptions), AutoOrderID).ToString();

        // スピード名を返す
        public string GetSpeedName() => Enum.ToObject(typeof(AutoPlaySpeed), AutoSpeedID).ToString();

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

        // オートスピード番号変更
        public void ChangeAutoSpeed()
        {
            if(!HasAuto)
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

        // オート機能の切り替え
        public void ChangeAutoMode()
        {
            // スピードモードが高速なら払い出しフェーズになるまで実行(処理の不具合を抑えるため)
            if(AutoSpeedID > (int)AutoPlaySpeed.Normal)
            {
                if(!HasAuto)
                {
                    HasAuto = true;
                }
                else
                {
                    Debug.Log("Fast Auto will end when payout is done");
                    HasWaitingCancel = true;
                }
            }
            // 通常スピード時はいつでも切れる
            else
            {
                HasAuto = !HasAuto;
            }

            HasStopPosDecided = false;
        }

        // 高速オート終了チェック
        public bool CheckEndFastAuto()
        {
            if(HasWaitingCancel)
            {
                HasAuto = false;
                HasWaitingCancel = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        // オート停止位置をリセット
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)AutoStopOrder.First] = 0;
            AutoStopOrders[(int)AutoStopOrder.Second] = 0;
            AutoStopOrders[(int)AutoStopOrder.Third] = 0;

            //Debug.Log("AutoPos Reset");
        }

        // オート押し順をフラグ、条件から得る
        public void GetAutoStopPos(FlagId flag, BonusType holdingBonus, int bigChanceGames, int remainingJac)
        {
            //Debug.Log("GetPos");
            SetAutoStopOrder();
            AutoStopPos = autoAI.GetStopPos(flag, AutoStopOrders[(int)AutoStopOrder.First], holdingBonus, bigChanceGames, remainingJac);
            //Debug.Log("Pos created");

            HasStopPosDecided = true;
            //Debug.Log("AutoPos Decided");
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
            else
            {
                //Debug.Log("Failed to change because there are duplicated orders");
            }
        }
    }
}
