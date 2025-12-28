using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_UI.Bonus
{
    // ボーナス履歴表示オブジェクト
    public class BonusLogDisplay : MonoBehaviour, IPointerDownHandler
    {
        // const

        // var
        [SerializeField] Image selection; // 選択カーソル
        [SerializeField] TextMeshProUGUI numberText; // 履歴番号テキスト
        [SerializeField] TextMeshProUGUI bonusTypeText; // ボーナスタイプのテキスト
        [SerializeField] BonusTypeDisplay bonusTypeDisplay; // ボーナスタイプ表示画像
        [SerializeField] TextMeshProUGUI payoutText; // 払い出し枚数テキスト

        public int BonusIndexNumber { get; private set; } // ボーナスの配列要素番号
        public bool HasSelect { get; private set; } // 選択されているか
        public bool CanInteractable { get; private set; } // ボタンが押せる状態か

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

        // ボタンが押された時のイベント
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexNum">配列番号</param>
        public delegate void OnBonusLogPressed(int indexNum);
        public event OnBonusLogPressed OnBonusLogPressedEvent;

        void Awake()
        {
            BonusIndexNumber = 0;
            ToggleSelection(false);
            CanInteractable = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                if (eventData.button == 0)
                {
                    OnBonusLogPressedEvent?.Invoke(BonusIndexNumber);
                }
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }

        // 選択状態の切り替え
        public void ToggleSelection(bool isSelected)
        {
            HasSelect = isSelected;
            selection.gameObject.SetActive(isSelected);
        }

        // データ登録
        public void SetData(BonusDisplayData bonusDisplayData)
        {
            // 配列要素番号
            BonusIndexNumber = bonusDisplayData.BonusLogNumber - 1;
            // 履歴番号
            numberText.text = "No." + bonusDisplayData.BonusLogNumber;

            // ボーナス種類
            if (bonusDisplayData.BigColor != BigColor.None)
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
    }
}

