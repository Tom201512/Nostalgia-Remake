using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_AutoPlay.AI
{
	// オートプレイ時のAI
	public class AutoPlayAI
	{
		// const

		// var

		public AutoPlayAI()
		{

		}

		// func

		// 停止位置を小役や現在の状態に合わせて返す
		public int[] GetStopPos(FlagId flag, BonusType holdingBonus)
		{
			// 小役ごとに挙動を変える予定
            return AINoneBehaviour(BonusType.BonusNone); ;
		}

		// はずれ時
		private int[] AINoneBehaviour(BonusType holdingBonus)
		{
			int[] stopPos = new int[] { 0, 0, 0 };
            // ボーナスがある場合はそのボーナスを狙うように

            if (holdingBonus == BonusType.BonusBIG)
			{
				// 1/3で赤7, 青7, BB7のいずれかを選択(左制御のみ)
				switch (Random.Range((int)BigColor.Red, (int)BigColor.Black))
				{
					case (int)BigColor.Red:

						// 1/2でチェリーのない赤7を選択
						if(Random.Range(0,1) == 1)
						{
                            stopPos[(int)AutoStopOrder.First] = 12;
                        }
						else
						{
                            stopPos[(int)AutoStopOrder.First] = 18;
                        }
						stopPos[(int)AutoStopOrder.Second] = 18;
						stopPos[(int)AutoStopOrder.Third] = 18;
						break;
					case (int)BigColor.Blue:
                        stopPos[(int)AutoStopOrder.First] = 3;

                        // 1/2でBAR上の青7を停止させる
                        if (Random.Range(0, 1) == 1)
                        {
                            stopPos[(int)AutoStopOrder.Second] = 6;
                        }
                        else
                        {
                            stopPos[(int)AutoStopOrder.Second] = 13;
                        }


                        stopPos[(int)AutoStopOrder.Third] = 9;
                        break;
					case (int)BigColor.Black:

                        // 1/2で上にチェリーのあるBARを停止
                        if (Random.Range(0, 1) == 1)
                        {
                            stopPos[(int)AutoStopOrder.First] = 9;
                        }
                        else
                        {
                            stopPos[(int)AutoStopOrder.First] = 16;
                        }

                        stopPos[(int)AutoStopOrder.Second] = 9;
                        stopPos[(int)AutoStopOrder.Third] = 18;
                        break;
				}
			}

			// 
			else
			{
				stopPos[(int)AutoStopOrder.First] = Random.Range(0, MaxReelArray - 1);
                stopPos[(int)AutoStopOrder.Second] = Random.Range(0, MaxReelArray - 1);
                stopPos[(int)AutoStopOrder.Third] = Random.Range(0, MaxReelArray - 1);
            }

			Debug.Log("StopPos generated:" + stopPos[(int)AutoStopOrder.First] +
				"," + stopPos[(int)AutoStopOrder.Second] + "," + stopPos[(int)AutoStopOrder.Third]);
			return stopPos;
        }
	}
}