using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Option.MenuBar
{
    public class MenuManager : MonoBehaviour
    {
        // ���j���[���X�g�̊Ǘ��}�l�[�W���[

        // const

        // var

        // �e��{�^��
        // �V�ѕ��K�C�h���J���{�^��
        [SerializeField] private ButtonComponent howToPlayButton;
        // �X���b�g����ʂ��J���{�^��
        [SerializeField] private ButtonComponent slotDataButton;

        // �e����
        // �V�ѕ��K�C�h�̉��
        [SerializeField] private HowToPlayScreen howToPlayScreen;
        // �X���b�g�����
        [SerializeField] private SlotDataScreen slotDataScreen;

        // ��������̃{�^�����������Ƃ��̃C�x���g
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // ��������̉�ʂ�����Ƃ��̃C�x���g
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // �I�v�V����

        void Awake()
        {
            howToPlayButton.ButtonPushedEvent += HowToPlayOpen;
            howToPlayScreen.OnClosedScreenEvent += HowToPlayClose;
            slotDataButton.ButtonPushedEvent += SlotDataOpen;
            slotDataScreen.OnClosedScreenEvent += SlotDataClose;
        }

        void Start()
        {
            SetInteractiveAllButton(true);
        }

        void OnDestroy()
        {
            howToPlayButton.ButtonPushedEvent -= HowToPlayOpen;
            howToPlayScreen.OnClosedScreenEvent -= HowToPlayClose;
            slotDataButton.ButtonPushedEvent += SlotDataOpen;
            slotDataScreen.OnClosedScreenEvent += SlotDataClose;
        }

        // func
        // �V�ѕ��K�C�h���J�������̏���
        public void HowToPlayOpen()
        {
            howToPlayScreen.gameObject.SetActive(true);
            howToPlayScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open HowToPlay");
        }

        // �V�ѕ��K�C�h��������̏���
        public void HowToPlayClose()
        {
            howToPlayScreen.CloseScreen();
            howToPlayScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close HowToPlay");
        }

        // �X���b�g�����J�������̏���
        public void SlotDataOpen()
        {
            slotDataScreen.gameObject.SetActive(true);
            slotDataScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open SlotData");
        }

        // �X���b�g����������̏���
        public void SlotDataClose()
        {
            slotDataScreen.CloseScreen();
            slotDataScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close SlotData");
        }

        // ��ʂ��J�����Ƃ��̏���
        private void OnPressedBehaviour()
        {
            SetInteractiveAllButton(false);
            OnPressedMenuEvent?.Invoke();
        }

        // ��ʂ�����Ƃ��̏���
        private void OnClosedBehaviour()
        {
            SetInteractiveAllButton(true);
            OnClosedScreenEvent?.Invoke();
        }

        // �S���j���[�̃��b�N�ݒ�
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
            slotDataButton.ToggleInteractive(value);
        }
    }
}
