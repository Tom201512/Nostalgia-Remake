using ReelSpinGame_Option.Button;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
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

        // ローカライズ用の画像、テキスト
        [SerializeField] TextMeshProUGUI selectText;    // 選択項目のテキスト
        [SerializeField] Image selectImage;   // 選択項目の画像

        // 設定項目
        public int CurrentSettingID { get; private set; } // 現在選択しているID

        int contentListCount; // コンテンツの数

        // ローカライズ
        LocalizeStringEvent selectTextLocalize;    // 選択項目のテキスト
        LocalizeSpriteEvent selectImageLocalize;   // 選択項目の画像

        // 設定項目が変わったときのイベント
        public delegate void ContentChanged();
        public event ContentChanged ContentChangedEvent;

        void Awake()
        {
            CurrentSettingID = 0;
            selectTextLocalize = selectText.GetComponent<LocalizeStringEvent>();
            selectImageLocalize = selectImage.GetComponent<LocalizeSpriteEvent>();
            nextContentButton.ButtonPushedEvent += OnNextPressed;
            previousContentButton.ButtonPushedEvent += OnPreviousPressed;

            // メニュー数の取得
            contentListCount = textDisplayList.Count;

            // テキストの個数より画像の方が多ければ更新
            if(contentListCount < imageDisplayList.Count)
            {
                contentListCount = imageDisplayList.Count;
            }
        }

        void Start()
        {
            //UpdateScreen();
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

        // 選択ボタンの有効化設定
        public void SetInteractive(bool value)
        {
            nextContentButton.ToggleInteractive(value);
            previousContentButton.ToggleInteractive(value);
        }

        // func(private)

        // 速度選択変更ボタンのイベント
        void OnNextPressed(int signalID)
        {
            CurrentSettingID += 1;
            if (CurrentSettingID == contentListCount)
            {
                CurrentSettingID = 0;
            }

            UpdateScreen();
            ContentChangedEvent?.Invoke();
        }

        void OnPreviousPressed(int signalID)
        {
            CurrentSettingID -= 1;
            if (CurrentSettingID < 0)
            {
                CurrentSettingID = contentListCount - 1;
            }

            UpdateScreen();
            ContentChangedEvent?.Invoke();
        }

        // 画面更新
        void UpdateScreen()
        {
            selectText.gameObject.SetActive(false);
            selectImage.gameObject.SetActive(false);

            // 対応するテキストがあれば表示する
            if(CurrentSettingID < textDisplayList.Count)
            {
                selectText.gameObject.SetActive(true);
                selectTextLocalize.StringReference = textDisplayList[CurrentSettingID];
            }

            if(!selectText.IsActive())
            {
                // 対応する画像があれば表示する
                if (CurrentSettingID < imageDisplayList.Count)
                {
                    selectImage.gameObject.SetActive(true);
                    selectImageLocalize.AssetReference = imageDisplayList[CurrentSettingID];
                }
            }
        }
    }
}
