using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class HowToPlayScreen : OptionScreenBase
{
    // 遊び方ガイド画面

    // const
    // var
    // スクリーン
    [SerializeField] private Image screen;
    // 表示する画面
    [SerializeField] private List<Texture> imageList;
    // 次ボタン
    [SerializeField] private ButtonComponent nextButton;
    // 前ボタン
    [SerializeField] private ButtonComponent previousButton;

    // 表示中のページ番号
    private int currentPage = 0;

    // 初期化用
    protected override void InitializeScreen()
    {
        Debug.Log("Initialized How To Play");
        CanInteract = true;
        Debug.Log("Interact :" + CanInteract);
        currentPage = 0;

        // ボタン登録
        //nextButton.ButtonPushedEvent += OnNextPushed;
        // previousButton.ButtonPushedEvent += OnPreviousPushed;
    }

    // 終了用
    protected override void CloseScreenBehavior()
    {
        Debug.Log("Closed How To Play");
        // ボタン登録解除
        //nextButton.ButtonPushedEvent -= OnNextPushed;
        //previousButton.ButtonPushedEvent -= OnPreviousPushed;
    }

    // 次ボタンを押したときの挙動
    private void OnNextPushed()
    {
        if (currentPage + 1 == imageList.Count)
        {
            currentPage = 0;
        }
        else
        {
            currentPage += 1;
        }

        UpdateScreen();
    }

    // 前ボタンを押したときの挙動
    private void OnPreviousPushed()
    {
        if (currentPage - 1 < 0)
        {
            currentPage = imageList.Count - 1;
        }
        else
        {
            currentPage -= 1;
        }

        UpdateScreen();
    }

    // 画像の反映処理
    private void UpdateScreen()
    {
        Debug.Log("Page:" + currentPage + 1);
        screen.image = imageList[currentPage];
    }
}
