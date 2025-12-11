using ReelSpinGame_Option.Button;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Option.AutoSetting
{
    // オート設定マネージャー
    public class AutoSettingManager : MonoBehaviour
    {
        // const

        // var
        // 選択ボタン
        [SerializeField] ButtonComponent speedSelectNext; // 速度選択(次)
        [SerializeField] ButtonComponent speedSelectPrevious; // 速度選択(前)
        [SerializeField] ButtonComponent orderSelectNext; // 押し順選択(次)
        [SerializeField] ButtonComponent orderSelectPrevious; // 押し順選択(前)
        [SerializeField] ButtonComponent bigColorSelectNext; // BIG図柄選択(次)
        [SerializeField] ButtonComponent bigColorSelectPrevious; // BIG図柄選択(前) 
        [SerializeField] ButtonComponent technicalPlaySelectNext; // 技術介入選択(次)
        [SerializeField] ButtonComponent technicalPlaySelectPrevious; // 技術介入選択(前)

        [SerializeField] LocalizedString speedSelectLocalize; // スピード表記のローカライズ

        [SerializeField] List<Sprite> bigColorSprites; // 選択時の画像

        // BIG図柄表示画像
        [SerializeField] TextMeshProUGUI bigColorRandomText; // ランダム選択時のテキスト
        [SerializeField] Image bigColorImage; // 各ボーナス選択時のテキスト

        // 設定項目
        public int CurrentAutoSpeed { get; private set; } // 現在のオート速度
        public int CurrentAutoOrder { get; private set; } // 現在のオート速度
        public int CurrentBigColor {  get; private set; } // 現在選択しているBIG色
        public bool HasTechnicalPlay { get; private set; } // 技術介入を所有しているか

        void Awake()
        {
            CurrentAutoSpeed = (int)AutoPlaySpeed.Normal;
            CurrentAutoOrder = (int)AutoStopOrderOptions.LRM;
            CurrentBigColor = (int)BigColor.None;
            HasTechnicalPlay = true;
        }

        // func(public)

        // func(private)
        
        // 速度選択変更ボタンのイベント
        void OnSpeedNextPressed(int signalID)
        {
            CurrentAutoSpeed += 1;
            if(CurrentAutoSpeed > (int)AutoPlaySpeed.Quick)
            {
                CurrentAutoSpeed = 0;
            }
        }

        void OnSpeedPreviousPressed(int signalID)
        {
            CurrentAutoSpeed -= 1;
            if (CurrentAutoSpeed < 0)
            {
                CurrentAutoSpeed = (int)AutoPlaySpeed.Quick;
            }
        }

        // 押し順変更ボタンのイベント
        void OnOrderNextPressed(int signalID)
        {
            CurrentAutoOrder += 1;
            if (CurrentAutoOrder > (int)AutoStopOrderOptions.RML)
            {
                CurrentAutoOrder = 0;
            }
        }

        void OnOrderPreviousPressed(int signalID)
        {
            CurrentAutoOrder -= 1;
            if (CurrentAutoOrder < 0)
            {
                CurrentAutoOrder = (int)AutoStopOrderOptions.RML;
            }
        }

        // BIG時図柄指定ボタンのイベント
        void OnBigColorNextPressed(int signalID)
        {
            CurrentBigColor += 1;
            if (CurrentBigColor > (int)BigColor.Black)
            {
                CurrentBigColor = 0;
            }
        }

        void OnBigColorPreviousPressed(int signalID)
        {
            CurrentBigColor -= 1;
            if (CurrentBigColor < 0)
            {
                CurrentBigColor = (int)BigColor.Black;
            }
        }

        // 技術介入指定ボタンのイベント(両方とも同じボタン)
        void OnTechnicalPlayPressed()
        {
            HasTechnicalPlay = !HasTechnicalPlay;
        }

        // 画面更新
        void UpdateScreen()
        {

        }
    }
}
