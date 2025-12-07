using ReelSpinGame_Option.Button;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_UI.Bonus
{
    // ボーナス履歴表示オブジェクト
    public class BonusLogDisplay : MonoBehaviour
    {
        // const

        // var
        [SerializeField] Image selection; // 選択カーソル
        [SerializeField] TextMeshProUGUI numberText; // 履歴番号テキスト
        [SerializeField] TextMeshProUGUI bonusTypeText; // ボーナスタイプのテキスト
        [SerializeField] BonusTypeDisplay bonusTypeDisplay; // ボーナスタイプ表示画像
        [SerializeField] TextMeshProUGUI payoutText; // 払い出し枚数テキスト

        public bool HasSelect {  get; private set; } // 選択されているか

        // 登録データ
        public struct BonusDisplayData
        {
            public int BonusLogNumber; // ボーナス履歴番号
            public BigColor BigColor; // ビッグ時の色
            public int Payouts; // 当該ボーナスの払い出し枚数
            public List<int> BonusReelPos; // ボーナス成立時のリール位置
            public List<int> BonusReelPushOrder; // ボーナス成立時の押し順
            public List<int> BonusReelDelay; // ボーナス成立時のスベリコマ数
        }

        void Awake()
        {
            ToggleSelection(false);
        }

        void Start()
        {

        }

        void Update()
        {

        }

        // func (public)
        // 選択状態の切り替え
        public void ToggleSelection(bool isSelected)
        {
            HasSelect = isSelected;
            selection.gameObject.SetActive(isSelected);
        }

        // データ登録
        public void SetData(BonusDisplayData bonusDisplayData)
        {
            // 履歴番号
            numberText.text = "No." + bonusDisplayData.BonusLogNumber;

            // ボーナス種類
            if(bonusDisplayData.BigColor != BigColor.None)
            {
                bonusTypeText.color = Color.red;
                bonusTypeText.text = "BIG";
            }
            else
            {
                bonusTypeText.color = Color.cyan;
                bonusTypeText.text = "REG";
            }

            // ボーナス画像
            bonusTypeDisplay.ShowBonusDisplay(bonusDisplayData.BigColor);

            // 払い出し枚数
            payoutText.text = bonusDisplayData.Payouts.ToString();
        }

        // func (private)
    }
}

