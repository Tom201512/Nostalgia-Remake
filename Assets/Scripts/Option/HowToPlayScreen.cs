using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Option.MenuContent
{
    public class HowToPlayScreen : MonoBehaviour, IOptionScreenBase
    {
        // �V�ѕ��K�C�h���

        // const

        // ���삪�ł����Ԃ�(�A�j���[�V�������Ȃǂ͂��Ȃ�����)
        public bool CanInteract { get; set; }

        // var
        // �X�N���[��
        [SerializeField] private Image screen;
        // �\��������
        [SerializeField] private List<Sprite> imageList;
        // ���{�^��
        [SerializeField] private ButtonComponent nextButton;
        // �O�{�^��
        [SerializeField] private ButtonComponent previousButton;
        // �N���[�Y�{�^��
        [SerializeField] private ButtonComponent closeButton;
        // �y�[�W�\�L
        [SerializeField] private TextMeshProUGUI pageCount;

        // �\�����̃y�[�W�ԍ�
        private int currentPage = 0;

        // ��ʂ�����Ƃ��̃C�x���g
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func

        private void Awake()
        {
            // �{�^���o�^
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
        }

        private void Update()
        {
            if (CanInteract)
            {
                if (OriginalInput.CheckOneKeyInput(KeyCode.RightArrow))
                {
                    OnNextPushed();
                }
                if (OriginalInput.CheckOneKeyInput(KeyCode.LeftArrow))
                {
                    OnPreviousPushed();
                }
            }
        }

        private void OnDestroy()
        {
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            nextButton.ButtonPushedEvent -= OnNextPushed;
            previousButton.ButtonPushedEvent -= OnPreviousPushed;
        }

        // ��ʕ\��&������
        public void OpenScreen()
        {
            Debug.Log("Initialized How To Play");
            CanInteract = true;
            Debug.Log("Interact :" + CanInteract);
            currentPage = 0;
            UpdateScreen();

            closeButton.ToggleInteractive(true);
            nextButton.ToggleInteractive(true);
            previousButton.ToggleInteractive(true);
        }

        // ��ʂ����
        public void CloseScreen()
        {
            Debug.Log("Interact :" + CanInteract);
            if (CanInteract)
            {
                Debug.Log("Closed How To Play");
                closeButton.ToggleInteractive(false); ;
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
            }
        }

        // ���{�^�����������Ƃ��̋���
        private void OnNextPushed()
        {
            Debug.Log("Next pressed");
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
            Debug.Log("Previous pressed");
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

        // ����{�^�����������Ƃ��̋���
        private void OnClosedPressed() => OnClosedScreenEvent?.Invoke();

        // �摜�̔��f����
        private void UpdateScreen()
        {
            Debug.Log("Page:" + currentPage + 1);
            screen.sprite = imageList[currentPage];
            pageCount.text = (currentPage + 1) + "/" + imageList.Count;
        }
    }
}
