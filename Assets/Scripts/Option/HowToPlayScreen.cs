using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class HowToPlayScreen : OptionScreenBase
{
    // �V�ѕ��K�C�h���

    // const
    // var
    // �X�N���[��
    [SerializeField] private Image screen;
    // �\��������
    [SerializeField] private List<Texture> imageList;
    // ���{�^��
    [SerializeField] private ButtonComponent nextButton;
    // �O�{�^��
    [SerializeField] private ButtonComponent previousButton;

    // �\�����̃y�[�W�ԍ�
    private int currentPage = 0;

    // �������p
    protected override void InitializeScreen()
    {
        Debug.Log("Initialized How To Play");
        CanInteract = true;
        Debug.Log("Interact :" + CanInteract);
        currentPage = 0;

        // �{�^���o�^
        //nextButton.ButtonPushedEvent += OnNextPushed;
        // previousButton.ButtonPushedEvent += OnPreviousPushed;
    }

    // �I���p
    protected override void CloseScreenBehavior()
    {
        Debug.Log("Closed How To Play");
        // �{�^���o�^����
        //nextButton.ButtonPushedEvent -= OnNextPushed;
        //previousButton.ButtonPushedEvent -= OnPreviousPushed;
    }

    // ���{�^�����������Ƃ��̋���
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

    // �O�{�^�����������Ƃ��̋���
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

    // �摜�̔��f����
    private void UpdateScreen()
    {
        Debug.Log("Page:" + currentPage + 1);
        screen.image = imageList[currentPage];
    }
}
