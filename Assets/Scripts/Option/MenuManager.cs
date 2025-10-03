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
        [SerializeField] private OptionScreenBase screenBase; 

        // ��������̃{�^�����������Ƃ��̃C�x���g
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // ��������̃E�B���h�E������Ƃ��̃C�x���g
        public delegate void OnClosedWindow();
        public event OnClosedWindow OnClosedWindowEvent;

        // �I�v�V����

        void Awake()
        {
            howToPlayButton.ButtonPushedEvent += TestOpen;
            screenBase.OnClosedWindowEvent += TestClose;
        }

        void Start()
        {
            howToPlayButton.ToggleInteractive(true);
        }

        // func
        // (�e�X�g�p)
        // �����̃{�^�����J�������̊֐�
        public void TestOpen()
        {
            screenBase.gameObject.SetActive(true);
            SetInteractiveAllButton(false);
            OnPressedMenuEvent?.Invoke();
            Debug.Log("Open Dialog");
        }

        // ��ʂ�����Ƃ��̃C�x���g
        public void TestClose()
        {
            screenBase.gameObject.SetActive(false);
            SetInteractiveAllButton(true);
            OnClosedWindowEvent?.Invoke();
            Debug.Log("Close");
        }

        // �S���j���[�̃��b�N�ݒ�
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
        }
    }
}
