using ReelSpinGame_Option.Button;
using ReelSpinGame_Reels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Option.MenuContent
{
    // 強制フラグ設定画面
    public class ForceFlagScreen : MonoBehaviour, IOptionScreenBase
    {
        // 操作ができる状態か(アニメーション中などはつけないこと)
        public bool CanInteract { get; set; }

        // var
        [SerializeField] List<ButtonComponent> flagButtons; // フラグボタン
        [SerializeField] List<Sprite> flagImages; // フラグ画像リスト
        [SerializeField] Image flagDisplayImage; // 選択中のフラグを表示するための画像

        [SerializeField] TextMeshProUGUI randomValueText; // ランダム数値
        [SerializeField] ButtonComponent closeButton; // クローズボタン
        [SerializeField] ButtonComponent randomNextButton; // ランダム数値変更ボタン(右)
        [SerializeField] ButtonComponent randomPreviousButton; // ランダム数値変更ボタン(左)
        [SerializeField] ButtonComponent resetButton; // フラグ設定リセットボタン


        public int CurrentSelectFlagID { get; private set; } // 選択中のフラグ番号 (-1はフラグなしとする)
        public int CurrentSelectRandomID { get; private set; } // 選択中のランダム数値 (0はランダムとする)

        // 設定が変更された時のイベント
        public delegate void OnSomethingChanged();
        public event OnSomethingChanged OnSomethingChangedEvent;

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func
        private void Awake()
        {
            // ボタン登録
            foreach(ButtonComponent flagButton in flagButtons)
            {
                flagButton.ButtonPushedEvent += SetSelectedFlag;
            }

            randomNextButton.ButtonPushedEvent += IncreaseRandomValue;
            randomPreviousButton.ButtonPushedEvent += DecreaseRandomValue;
            resetButton.ButtonPushedEvent += ResetFlagSetting;
            closeButton.ButtonPushedEvent += OnClosedPressed;

            // 最初のみ初期化
            CurrentSelectFlagID = -1;
            CurrentSelectRandomID = 0;
        }

        private void OnDestroy()
        {
            // 登録解除
            foreach (ButtonComponent flagButton in flagButtons)
            {
                flagButton.ButtonPushedEvent -= SetSelectedFlag;
            }

            randomNextButton.ButtonPushedEvent -= IncreaseRandomValue;
            randomPreviousButton.ButtonPushedEvent -= DecreaseRandomValue;
            resetButton.ButtonPushedEvent -= ResetFlagSetting;
            closeButton.ButtonPushedEvent -= OnClosedPressed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            Debug.Log("Initialized ForceFlag");
            CanInteract = true;
            Debug.Log("Interact :" + CanInteract);
            // ボタン有効化
            randomNextButton.ToggleInteractive(true);
            randomPreviousButton.ToggleInteractive(true);
            resetButton.ToggleInteractive(true);
            closeButton.ToggleInteractive(true);

            UpdateFlagSelectImage();
            UpdateRandomValueText();
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            Debug.Log("Interact :" + CanInteract);
            if (CanInteract)
            {
                Debug.Log("Closed ForceFlag");
                SetBonusFlagButtonInteractive(false);
                SetBonusFlagButtonInteractive(false);
                randomNextButton.ToggleInteractive(false);
                randomPreviousButton.ToggleInteractive(false);
                resetButton.ToggleInteractive(false);
                closeButton.ToggleInteractive(false);
            }
        }

        // 各種フラグボタンの有効化設定をボーナス状態に合わせて変更する
        public void SetFlagButtonsInteractive(BonusStatus currentBonusStatus, BonusTypeID holdingBonusID)
        {
            // 通常時にいない場合はボーナスフラグボタンは無効にする
            if (currentBonusStatus != BonusStatus.BonusNone)
            {
                SetBonusFlagButtonInteractive(false);
                // JAC中なら小役(ハズレ含む)フラグボタンは無効にする
                if (currentBonusStatus == BonusStatus.BonusJACGames)
                {
                    SetSymbolFlagButtonInteractive(false);
                }
                else
                {
                    SetSymbolFlagButtonInteractive(true);
                }
            }

            // 通常時にいる場合はボーナスが成立していればボーナスフラグボタンは無効にする
            else
            {
                if (holdingBonusID != BonusTypeID.BonusNone)
                {
                    SetBonusFlagButtonInteractive(false);
                }
                else
                {
                    SetBonusFlagButtonInteractive(true);
                }

                SetSymbolFlagButtonInteractive(true);
            }
        }

        // フラグ設定をリセットする
        public void ResetFlagSetting()
        {
            CurrentSelectFlagID = -1;
            CurrentSelectRandomID = 0;
            OnSomethingChangedEvent?.Invoke();
        }

        // ボーナスフラグ設定ボタンの有効化設定
        void SetBonusFlagButtonInteractive(bool value)
        {
            flagButtons[(int)FlagID.FlagBig].ToggleInteractive(value);
            flagButtons[(int)FlagID.FlagReg].ToggleInteractive(value);
        }

        // 小役フラグ設定ボタンの有効化設定
        void SetSymbolFlagButtonInteractive(bool value)
        {
            flagButtons[(int)FlagID.FlagCherry2].ToggleInteractive(value);
            flagButtons[(int)FlagID.FlagCherry4].ToggleInteractive(value);
            flagButtons[(int)FlagID.FlagMelon].ToggleInteractive(value);
            flagButtons[(int)FlagID.FlagBell].ToggleInteractive(value);
            flagButtons[(int)FlagID.FlagReplayJacIn].ToggleInteractive(value);
            flagButtons[(int)FlagID.FlagNone].ToggleInteractive(value);
        }

        // 閉じるボタンを押したときの挙動
        void OnClosedPressed(int signalID) => OnClosedScreenEvent?.Invoke();

        // 選択したフラグを割り当てる
        void SetSelectedFlag(int signalID)
        {
            CurrentSelectFlagID = signalID;
            UpdateFlagSelectImage();
        }

        // ランダム数値の増加
        void IncreaseRandomValue(int signalID)
        {
            CurrentSelectRandomID += 1;
            // 数値が6を超えたらランダム(0)に戻す
            if(CurrentSelectRandomID > ReelManagerModel.MaxRandomLots)
            {
                CurrentSelectRandomID = 0;
            }

            // 数値テキストの変更
            UpdateRandomValueText();
        }

        // ランダム数値の減少
        void DecreaseRandomValue(int signalID)
        {
            CurrentSelectRandomID -= 1;
            // 数値が-1を超えたら6に戻す
            if (CurrentSelectRandomID < 0)
            {
                CurrentSelectRandomID = 6;
            }

            // 数値テキストの変更
            UpdateRandomValueText();
        }

        // リセットフラグ
        void ResetFlagSetting(int signalID)
        {
            CurrentSelectFlagID = -1;
            CurrentSelectRandomID = 0;

            UpdateFlagSelectImage();
            UpdateRandomValueText();
        }

        // フラグ選択中画像変更
        void UpdateFlagSelectImage()
        {
            // フラグ選択がされていれば更新
            if(CurrentSelectFlagID > -1)
            {
                flagDisplayImage.gameObject.SetActive(true);
                flagDisplayImage.sprite = flagImages[CurrentSelectFlagID];
            }
            else
            {
                flagDisplayImage.gameObject.SetActive(false);
            }
        }

        // 数値テキスト変更
        void UpdateRandomValueText()
        {
            if(CurrentSelectRandomID == 0)
            {
                randomValueText.text = "?";
            }
            else
            {
                randomValueText.text = CurrentSelectRandomID.ToString();
            }
        }
    }
}

