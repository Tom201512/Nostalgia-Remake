using ReelSpinGame_Datas;
using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_System;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotDataScreen : MonoBehaviour, IOptionScreenBase
    {
        // �X���b�g�����

        // const
        private const int maxPage = 3;

        // var

        // �e����
        // ���C�����
        [SerializeField] private SlotMainDataUI slotMainDataUI;
        // �ʏ펞�����m��
        [SerializeField] private ProbabilityDataUI probabilityDataUI;
        // ���߃{�[�i�X���AJAC�n�Y�V�m���Ȃ�
        [SerializeField] private BonusDataUI bonusDataUI;

        // ���{�^��
        [SerializeField] private ButtonComponent nextButton;
        // �O�{�^��
        [SerializeField] private ButtonComponent previousButton;
        // �N���[�Y�{�^��
        [SerializeField] private ButtonComponent closeButton;
        // �y�[�W�\�L
        [SerializeField] private TextMeshProUGUI pageCount;

        // ���삪�ł����Ԃ�(�A�j���[�V�������Ȃǂ͂��Ȃ�����)
        public bool CanInteract { get; set; }

        // ��ʂ�����Ƃ��̃C�x���g
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // �\�����̃y�[�W�ԍ�
        private int currentPage = 0;

        // �v���C���[�f�[�^�̃A�h���X
        private PlayerDatabase playerData;

        // func
        private void Awake()
        {
            // �{�^���o�^
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
        }

        private void Start()
        {
            UpdateScreen();
        }

        private void Update()
        {
            if(CanInteract)
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
            Debug.Log("Initialized SlotData");
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
                Debug.Log("Closed SlotData");
                closeButton.ToggleInteractive(false); ;
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
            }
        }

        // �f�[�^���󂯓n��
        public void SendData(PlayerDatabase player)
        {
            playerData = player;
        }

        // ���{�^�����������Ƃ��̋���
        private void OnNextPushed()
        {
            Debug.Log("Next pressed");
            if (currentPage + 1 == maxPage)
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
                currentPage = maxPage - 1;
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
            DisactivateAllScreen();

            Debug.Log("Page:" + currentPage + 1);
            // �y�[�W���Ƃɏ������s��

            switch (currentPage)
            {
                case 0:
                    slotMainDataUI.gameObject.SetActive(true);
                    slotMainDataUI.UpdateText(playerData);
                    break;

                case 1:
                    probabilityDataUI.gameObject.SetActive(true);
                    probabilityDataUI.UpdateText(playerData);
                    break;

                case 2:
                    bonusDataUI.gameObject.SetActive(true);
                    bonusDataUI.UpdateText(playerData);
                    break;

                default:
                    break;
            }

            pageCount.text = (currentPage + 1) + "/" + maxPage;
        }

        private void DisactivateAllScreen()
        {
            slotMainDataUI.gameObject.SetActive(false);
            probabilityDataUI.gameObject.SetActive(false);
            bonusDataUI.gameObject.SetActive(false);
        }
    }
}
