using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_AutoPlay
{
    public class AutoPlayFunction
    {
        // オートプレイ機能

        // const
        // オート速度
        public enum AutoPlaySpeed { Normal, Fast, Quick}

        // var
        // オートプレイ中か
        public bool HasAuto { get; private set; }
        public AutoPlaySpeed AutoSpeed { get; private set; }
        public ReelID[] AutoStopOrders { get; private set; }
        // オートの速度


        // コンストラクタ
        public AutoPlayFunction()
        {
            Debug.Log("Base Constructor");
            HasAuto = false;
            // 停止位置の配列作成(デフォルトは順押し)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoSpeed = AutoPlaySpeed.Normal;

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
        }

        // オート押し順の変更
        public void ChangeAutoStopOrder(ReelID first, ReelID second, ReelID third)
        {
            // 同じ押し順がないかチェック
            if(first != second && second != third)
            {
                AutoStopOrders[0] = first;
                AutoStopOrders[1] = second;
                AutoStopOrders[2] = third;
            }
            else
            {
                Debug.Log("Failed to change because there are duplicated orders");
            }
        }
    }
}
