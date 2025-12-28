using ReelSpinGame_Option.Button;
using UnityEngine;

namespace ReelSpinGame_Option.MenuBar
{
    // メニューバー用コンポーネント
    public class MenuContent : MonoBehaviour
    {
        // 各種ボタン
        [SerializeField] ButtonComponent howToPlayButton;       // 説明画面表示
        [SerializeField] ButtonComponent slotDataButton;        // データ表示
        [SerializeField] ButtonComponent instantFlagButton;     // 強制役セット画面表示
        [SerializeField] ButtonComponent autoSettingButton;     // オート設定画面表示
        [SerializeField] ButtonComponent optionButton;          // その他設定画面表示
    }
}
