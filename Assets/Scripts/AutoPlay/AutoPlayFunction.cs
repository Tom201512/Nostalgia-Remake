using ReelSpinGame_AutoPlay.AI;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
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
        // オート速度
        public enum AutoPlaySpeed { Normal, Fast, Quick}
        // 現在のオートモード
        public enum AutoPlaySequence { AutoInsert, AutoStopReel}

        // var
        // オートプレイ中か
        public bool HasAuto { get; private set; }
        // オート速度
        public AutoPlaySpeed AutoSpeed { get; private set; }
        // オート時の押し順
        public ReelID[] AutoStopOrders { get; private set; }
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
            Debug.Log("Base Constructor");
            HasAuto = false;
            // 停止順番の配列作成(デフォルトは順押し)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoSpeed = AutoPlaySpeed.Normal;

            // 停止位置の配列作成(テスト用に0で止めるように)
            AutoStopPos = new int[] { 0, 0, 0 };

            Debug.Log("Speed:" + AutoSpeed);
            foreach(ReelID order in AutoStopOrders)
            {
                Debug.Log("Order:" + order.ToString());
            }
        }

        // 設定から読み込む場合
        public AutoPlayFunction(AutoPlaySpeed speed, ReelID[] order):this()
        {
            AutoSpeed = speed;
            AutoStopOrders = order;

            Debug.Log("Speed:" + AutoSpeed);
            foreach (ReelID newOrder in AutoStopOrders)
            {
                Debug.Log("Order:" + newOrder.ToString());
            }
        }

        // func

        // オート機能の切り替え
        public void ChangeAutoMode()
        {
            HasAuto = !HasAuto;
            Debug.Log("AutoMode:" + HasAuto);
            HasStopPosDecided = false;
        }
        
        // オート停止位置をリセット
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)AutoStopOrder.First] = 0;
            AutoStopOrders[(int)AutoStopOrder.Second] = 0;
            AutoStopOrders[(int)AutoStopOrder.Third] = 0;

            Debug.Log("AutoPos Reset");
        }

        // オート押し順をフラグ、条件から得る
        public void GetAutoStopPos(FlagId flag, BonusType holdingBonus, int bigChanceGames, int remainingJac)
        {
            Debug.Log("GetPos");

            AutoStopPos = autoAI.GetStopPos(flag, AutoStopOrders[(int)AutoStopOrder.First], holdingBonus, bigChanceGames, remainingJac);
            Debug.Log("Pos created");

            HasStopPosDecided = true;
            Debug.Log("AutoPos Decided");
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
            else
            {
                Debug.Log("Failed to change because there are duplicated orders");
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
                Debug.Log("Failed to change because there are duplicated orders");
            }
        }
    }
}
