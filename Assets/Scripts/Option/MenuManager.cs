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
        // �V�ѕ��̃{�^��
        [SerializeField] private ButtonComponent howToPlayButton;

        // �e����
        // �V�ѕ��\�����
        // �e�X�g�p
        [SerializeField] private HowToPlayScreen howToPlayScreen; 

        // ��������̃{�^�����������Ƃ��̃C�x���g
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // ��������̉�ʂ�����Ƃ��̃C�x���g
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // �I�v�V����

        void Awake()
        {
            howToPlayButton.ButtonPushedEvent += TestOpen;
            howToPlayScreen.OnClosedScreenEvent += TestClose;
        }

        void Start()
        {
            howToPlayButton.ToggleInteractive(true);
        }

        void OnDestroy()
        {
            howToPlayButton.ButtonPushedEvent -= TestOpen;
            howToPlayScreen.OnClosedScreenEvent -= TestClose;
        }

        // func
        // (�e�X�g�p)
        // �����̃{�^�����J�������̊֐�
        public void TestOpen()
        {
            howToPlayScreen.gameObject.SetActive(true);
            SetInteractiveAllButton(false);
            OnPressedMenuEvent?.Invoke();
            Debug.Log("Open Dialog");
        }

        // ��ʂ�����Ƃ��̃C�x���g
        public void TestClose()
        {
            howToPlayScreen.gameObject.SetActive(false);
            SetInteractiveAllButton(true);
            OnClosedScreenEvent?.Invoke();
            Debug.Log("Close");
        }

        // �S���j���[�̃��b�N�ݒ�
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
        }
    }
}
