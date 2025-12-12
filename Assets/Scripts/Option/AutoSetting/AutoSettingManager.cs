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
        [SerializeField] SelectButtonComponent speedSelect; // スピード変更

        void Awake()
        {
            speedSelect.LoadOptionData((int)AutoPlaySpeed.Normal);
        }

        // func(public)

        // func(private)
    }
}
