using UnityEngine;

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
        [SerializeField] SelectButtonComponent speedSelect;         // スピード変更
        [SerializeField] SelectButtonComponent orderSelect;         // 押し順変更
        [SerializeField] SelectButtonComponent bigColorSelect;      // BIG時図柄変更
        [SerializeField] SelectButtonComponent technicalSelect;     // 技術介入変更 

        void Awake()
        {
            speedSelect.LoadOptionData((int)AutoPlaySpeed.Normal);
            orderSelect.LoadOptionData((int)AutoStopOrderOptions.LMR);
            bigColorSelect.LoadOptionData((int)BigColor.None);
            technicalSelect.LoadOptionData(true ? 1 : 0);
        }

        // func(public)
        // 各種選択ボタンの有効化設定
        public void SetInteractiveButtons(bool value)
        {
            speedSelect.SetInteractive(value);
            orderSelect.SetInteractive(value);
            bigColorSelect.SetInteractive(value);
            technicalSelect.SetInteractive(value);
        }

        // func(private)
    }
}
