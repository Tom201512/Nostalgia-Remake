using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using ReelSpinGame_UI.Reel;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ReelSpinGame_UI.Bonus.BonusLogDisplay;

namespace ReelSpinGame_Option.MenuContent
{        
    // ボーナス履歴を表示するUI
    public class BonusRecordDataUI : MonoBehaviour
    {
        [SerializeField] BonusLogDisplayList bonusLogDisplayList;   // ボーナス履歴リスト
        [SerializeField] ReelDisplayUI reelDisplayUI;               // 当選時出目表示機能
        [SerializeField] TextMeshProUGUI noDataText;                // データがないときに表示するテキスト

        public List<BonusDisplayData> BonusDisplayDatas { get; private set; } // ボーナス履歴のデータ

        void Awake()
        {
            BonusDisplayDatas = new List<BonusDisplayData>();
            reelDisplayUI.gameObject.SetActive(false);
        }

        // ボーナス履歴をデータから作成する
        public void GenerateBonusResult(List<BonusHitData> bonusHitDatas)
        {
            int index = 0; // 配列要素番号
            if (bonusHitDatas.Count == 0)
            {
                noDataText.gameObject.SetActive(true);
            }
            else
            {
                noDataText.gameObject.SetActive(false);
            }

            foreach (BonusHitData bonusData in bonusHitDatas)
            {
                if (bonusData.BonusStartGame == 0)
                {
                    break;
                }
                GenerateBonusDisplayData(bonusData, index); // データを記録
                bonusLogDisplayList.AddBonusData(BonusDisplayDatas[index]); // 表示用画像を作成

                index += 1;
            }

            // ボタンが押されたときに成立時出目を表示
            bonusLogDisplayList.OnBonusLogSelectedEvent += DisplayHitReelPattern;
        }

        // 画面を閉じる
        public void CloseBonusResult()
        {
            BonusDisplayDatas.Clear();
            bonusLogDisplayList.InitializeData();
            bonusLogDisplayList.OnBonusLogSelectedEvent -= DisplayHitReelPattern;
            reelDisplayUI.gameObject.SetActive(false);
        }

        // ボーナス表示データの作成
        void GenerateBonusDisplayData(BonusHitData bonusHitData, int index)
        {
            BonusDisplayData bonusDisplayData = new BonusDisplayData();
            bonusDisplayData.BonusLogNumber = index + 1;
            bonusDisplayData.BigColor = bonusHitData.BigColor;
            bonusDisplayData.Payouts = bonusHitData.BonusPayout;
            bonusDisplayData.BonusReelPos = new List<int>(bonusHitData.BonusReelPos);
            bonusDisplayData.BonusReelPushOrder = new List<int>(bonusHitData.BonusReelPushOrder);
            bonusDisplayData.BonusReelDelay = new List<int>(bonusHitData.BonusReelDelay);
            BonusDisplayDatas.Add(bonusDisplayData);
        }

        // ボーナス履歴データが押されたら当選時の出目を表示
        void DisplayHitReelPattern(int indexNum)
        {
            reelDisplayUI.gameObject.SetActive(true);

            reelDisplayUI.DisplayReels(
                BonusDisplayDatas[indexNum].BonusReelPos[(int)ReelID.ReelLeft],
                BonusDisplayDatas[indexNum].BonusReelPos[(int)ReelID.ReelMiddle],
                BonusDisplayDatas[indexNum].BonusReelPos[(int)ReelID.ReelRight]);

            reelDisplayUI.DisplayPos(
                BonusDisplayDatas[indexNum].BonusReelPos[(int)ReelID.ReelLeft],
                BonusDisplayDatas[indexNum].BonusReelPos[(int)ReelID.ReelMiddle],
                BonusDisplayDatas[indexNum].BonusReelPos[(int)ReelID.ReelRight]);

            reelDisplayUI.DisplayOrder(
                BonusDisplayDatas[indexNum].BonusReelPushOrder[(int)ReelID.ReelLeft],
                BonusDisplayDatas[indexNum].BonusReelPushOrder[(int)ReelID.ReelMiddle],
                BonusDisplayDatas[indexNum].BonusReelPushOrder[(int)ReelID.ReelRight]);

            reelDisplayUI.DisplayDelay(
                BonusDisplayDatas[indexNum].BonusReelDelay[(int)ReelID.ReelLeft],
                BonusDisplayDatas[indexNum].BonusReelDelay[(int)ReelID.ReelMiddle],
                BonusDisplayDatas[indexNum].BonusReelDelay[(int)ReelID.ReelRight]);
        }
    }
}
