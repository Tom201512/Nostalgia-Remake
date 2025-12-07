using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_UI.Bonus.BonusLogDisplay;

namespace ReelSpinGame_Option.MenuContent
{
    public class BonusRecordDataUI : MonoBehaviour
    {
        // ボーナス履歴を表示するUI

        // const

        // var
        [SerializeField] private BonusLogDisplayList bonusLogDisplayList; // ボーナス履歴リスト

        public List<BonusDisplayData> BonusDisplayDatas { get; private set; } // ボーナス履歴のデータ

        void Awake()
        {
            BonusDisplayDatas = new List<BonusDisplayData>();
        }

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        // func(public)
        // ボーナス履歴をデータから作成する
        public void GenerateBonusResult(List<BonusHitData> bonusHitDatas)
        {
            int index = 0; // 配列要素番号
            foreach(BonusHitData bonusData in bonusHitDatas)
            {
                if(bonusData.BonusStartGame == 0)
                {
                    break;
                }
                GenerateBonusDisplayData(bonusData, index); // データを記録
                bonusLogDisplayList.AddBonusData(BonusDisplayDatas[index]); // 表示用画像を作成
                Debug.Log("Generated Data, Index:" + index);

                index += 1;
            }
        }

        // 画面を閉じる
        public void CloseBonusResult()
        {
            BonusDisplayDatas.Clear();
            bonusLogDisplayList.InitializeData();
        }

        // func(private)
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

            Debug.Log("Log:" + bonusDisplayData.BonusLogNumber);
            Debug.Log("Color:" + bonusDisplayData.BigColor);
            Debug.Log("Payouts:" + bonusDisplayData.Payouts);

            Debug.Log("BonusReelPos:" + 
                bonusDisplayData.BonusReelPos[(int)ReelID.ReelLeft] +
                bonusDisplayData.BonusReelPos[(int)ReelID.ReelMiddle] +
                bonusDisplayData.BonusReelPos[(int)ReelID.ReelRight]);

            Debug.Log("BonusReelPushOrder:" +
                bonusDisplayData.BonusReelPushOrder[(int)ReelID.ReelLeft] +
                bonusDisplayData.BonusReelPushOrder[(int)ReelID.ReelMiddle] +
                bonusDisplayData.BonusReelPushOrder[(int)ReelID.ReelRight]);

            Debug.Log("BonusReelDelay:" +
                bonusDisplayData.BonusReelDelay[(int)ReelID.ReelLeft] +
                bonusDisplayData.BonusReelDelay[(int)ReelID.ReelMiddle] +
                bonusDisplayData.BonusReelDelay[(int)ReelID.ReelRight]);

            BonusDisplayDatas.Add(bonusDisplayData);
        }
    }
}
