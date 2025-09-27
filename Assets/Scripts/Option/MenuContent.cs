using ReelSpinGame_Option.Button;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Option.MenuBar
{
    public class MenuContent : MonoBehaviour
    {
        // メニューバー用コンポーネント

        // var
        // 各種ボタン
        // 説明画面表示
        [SerializeField] ButtonComponent howToPlayButton;
        // データ表示
        [SerializeField] ButtonComponent slotDataButton;
        // 強制役セット画面表示
        [SerializeField] ButtonComponent instantFlagButton;
        // オート設定画面表示
        [SerializeField] ButtonComponent autoSettingButton;
        // その他設定画面表示
        [SerializeField] ButtonComponent optionButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
