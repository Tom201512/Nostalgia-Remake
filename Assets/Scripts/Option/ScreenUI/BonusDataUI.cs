using ReelSpinGame_Reels;
using ReelSpinGame_System;
using ReelSpinGame_UI.Reel;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // ボーナス中の情報
    public class BonusDataUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI dataTextUI;        // データ部分
        [SerializeField] ReelDisplayUI reelDisplay;         // リールディスプレイ(リーチ目用)

        public void UpdateText(PlayerDatabase player)
        {
            float probability = 0.0f;
            float rate = 0.0f;
            string data = "\n";

            // 直近ボーナスゲームの表示(ただし入賞していないものは表示しない)
            // 2回目以降のボーナスは入賞時ゲームが記録していれば表示
            if (player.BonusHitRecord.Count > 1)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    data += player.BonusHitRecord[^1].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^1].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^1].BonusPayout + "\n";
                }
                // そうでなければ2つ前を表示
                else
                {
                    data += player.BonusHitRecord[^2].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^2].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^2].BonusPayout + "\n";
                }
            }
            // 初回ボーナスの場合は入賞していなければ表示しない
            else if (player.BonusHitRecord.Count > 0)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    data += player.BonusHitRecord[^1].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^1].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^1].BonusPayout + "\n";
                }
                else
                {
                    data += "-------\n";
                    data += "-------\n";
                    data += "-------\n";
                }
            }
            // 非成立の場合は表示しない
            else
            {
                data += "-------\n";
                data += "-------\n";
                data += "-------\n";
            }

            data += "\n\n";

            // JACハズシ
            // 成功回数
            data += player.PlayerAnalyticsData.BigJacAvoidTimes + "\n";
            // ビタハズシ回数
            data += player.PlayerAnalyticsData.BigJacPerfectAvoidTimes + "\n";
            // アシストハズシ回数
            data += player.PlayerAnalyticsData.BigJacAssistedAvoidTimes + "\n";

            // 成功確率
            if (player.PlayerAnalyticsData.BigJacAvoidTimes > 0)
            {
                rate = (float)player.PlayerAnalyticsData.BigJacInTimes / player.PlayerAnalyticsData.BigJacAvoidTimes * 100;
                data += rate.ToString("F2") + "%\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ビタハズシ成功確率
            if (player.PlayerAnalyticsData.BigJacPerfectAvoidTimes > 0)
            {
                rate = (float)player.PlayerAnalyticsData.BigJacAvoidTimes / player.PlayerAnalyticsData.BigJacPerfectAvoidTimes * 100;
                data += rate.ToString("F2") + "%\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            data += "\n\n";


            // ボーナスゲーム中ハズレ
            // 回数
            data += player.PlayerAnalyticsData.JacGameNoneTimes + "\n";
            // 確率
            if (player.PlayerAnalyticsData.JacGameNoneTimes > 0)
            {
                probability = (float)player.PlayerAnalyticsData.JacGamesCount / player.PlayerAnalyticsData.JacGameNoneTimes;
                data += "1/" + probability.ToString("F2");
            }
            else
            {
                data += "1/---.--";
            }

            dataTextUI.text = data;
        }

        // 当選後出目を表示する
        public void DisplayWinningPattern(PlayerDatabase player)
        {
            // ボーナス履歴が2つ以上ある場合は条件に合わせて表示
            if (player.BonusHitRecord.Count > 1)
            {
                // ボーナスが2個以上ある場合でまだ当選していなければ2つ目を表示
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    reelDisplay.gameObject.SetActive(true);
                    reelDisplay.DisplayReels(player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelRight]);

                    // リール位置、スベリコマを表示
                    reelDisplay.DisplayPos(player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelRight]);

                    // 押し順を表示
                    reelDisplay.DisplayOrder(player.BonusHitRecord[^1].BonusReelPushOrder[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPushOrder[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPushOrder[(int)ReelID.ReelRight]);

                    // スベリコマを表示
                    reelDisplay.DisplayDelay(player.BonusHitRecord[^1].BonusReelDelay[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelDelay[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelDelay[(int)ReelID.ReelRight]);
                }
                // そうでなければ2つ前を表示
                else
                {
                    reelDisplay.gameObject.SetActive(true);
                    // ボーナスが2個以上ある場合でまだ当選していなければ2つ目を表示
                    reelDisplay.DisplayReels(player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelRight]);

                    // リール位置、スベリコマを表示
                    reelDisplay.DisplayPos(player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelRight]);

                    // 押し順を表示
                    reelDisplay.DisplayOrder(player.BonusHitRecord[^2].BonusReelPushOrder[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^2].BonusReelPushOrder[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^2].BonusReelPushOrder[(int)ReelID.ReelRight]);

                    // スベリコマを表示
                    reelDisplay.DisplayDelay(player.BonusHitRecord[^2].BonusReelDelay[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^2].BonusReelDelay[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^2].BonusReelDelay[(int)ReelID.ReelRight]);
                }
            }
            // 1つある場合は
            else if (player.BonusHitRecord.Count > 0)
            {
                // 入賞ゲーム数がなければ表示しない
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    reelDisplay.DisplayReels(player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelRight]);

                    // リール位置、スベリコマを表示
                    reelDisplay.DisplayPos(player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelRight]);

                    // 押し順を表示
                    reelDisplay.DisplayOrder(player.BonusHitRecord[^1].BonusReelPushOrder[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPushOrder[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPushOrder[(int)ReelID.ReelRight]);

                    // スベリコマを表示
                    reelDisplay.DisplayDelay(player.BonusHitRecord[^1].BonusReelDelay[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelDelay[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelDelay[(int)ReelID.ReelRight]);
                }
                else
                {
                    reelDisplay.gameObject.SetActive(false);
                }
            }
            else
            {
                reelDisplay.gameObject.SetActive(false);
            }
        }
    }
}
