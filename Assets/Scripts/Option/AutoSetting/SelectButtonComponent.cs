using ReelSpinGame_Option.Button;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using UnityEngine.UI;

namespace ReelSpinGame_Option
{
    // 左右セレクトボタンのコンポーネント
    public class SelectButtonComponent : MonoBehaviour
    {
        // const

        // var
        // 選択ボタン
        [SerializeField] ButtonComponent nextContentButton;       // 次ボタン
        [SerializeField] ButtonComponent previousContentButton;   // 前ボタン

        [SerializeField] List<LocalizedString> textDisplayList; // テキストのローカライズリスト
        [SerializeField] List<LocalizedSprite> imageDisplayList; // 画像のローカライズリスト

        [SerializeField] LocalizedString localText;

        // ローカライズ用の画像、テキスト
        [SerializeField] TextMeshProUGUI selectText;    // 選択項目のテキスト
        [SerializeField] Image selectImage;   // 選択項目の画像

        // 設定項目
        public int CurrentSettingID { get; private set; } // 現在選択しているID

        // ローカライズ
        LocalizeStringEvent selectTextLocalize;    // 選択項目のテキスト
        LocalizeSpriteEvent selectImageLocalize;   // 選択項目の画像

        void Awake()
        {
            CurrentSettingID = 0;
            selectTextLocalize = selectText.GetComponent<LocalizeStringEvent>();
            selectImageLocalize = selectImage.GetComponent<LocalizeSpriteEvent>();
            nextContentButton.ButtonPushedEvent += OnNextPressed;
            previousContentButton.ButtonPushedEvent += OnPreviousPressed;
        }

        void Start()
        {
            UpdateScreen();
        }

        void OnDestroy()
        {
            nextContentButton.ButtonPushedEvent -= OnNextPressed;
            previousContentButton.ButtonPushedEvent -= OnPreviousPressed;
        }

        // func(public)
        // データ読み込み
        public void LoadOptionData(int dataID)
        {
            CurrentSettingID = dataID;
            UpdateScreen();
        }

        // func(private)

        // 速度選択変更ボタンのイベント
        void OnNextPressed(int signalID)
        {
            CurrentSettingID += 1;
            if (CurrentSettingID > (int)AutoPlaySpeed.Quick)
            {
                CurrentSettingID = 0;
            }

            UpdateScreen();
        }

        void OnPreviousPressed(int signalID)
        {
            CurrentSettingID -= 1;
            if (CurrentSettingID < 0)
            {
                CurrentSettingID = (int)AutoPlaySpeed.Quick;
            }

            UpdateScreen();
        }

        // 画面更新
        void UpdateScreen()
        {
            // 対応するテキストがあれば表示する
            if(CurrentSettingID < textDisplayList.Count && textDisplayList[CurrentSettingID] != null)
            {
                selectText.gameObject.SetActive(true);
                selectTextLocalize.StringReference = textDisplayList[CurrentSettingID];
            }
            else
            {
                selectText.gameObject.SetActive(false);
            }

            // 対応する画像があれば表示する
            if (CurrentSettingID < imageDisplayList.Count && imageDisplayList[CurrentSettingID] != null)
            {
                selectImage.gameObject.SetActive(true);
                selectImageLocalize.AssetReference = imageDisplayList[CurrentSettingID];
            }
            else
            {
                selectImage.gameObject.SetActive(false);
            }
        }
    }
}
