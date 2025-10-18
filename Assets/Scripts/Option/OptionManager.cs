using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuBar;
using UnityEngine;

namespace ReelSpinGame_Option
{
    public class OptionManager : MonoBehaviour
    {
        // �I�v�V�����}�l�[�W���[

        // const

        // var
        // ���j���[�J�{�^��
        [SerializeField] private ButtonComponent openButton;
        // ���j���[�o�[��UI
        [SerializeField] private MenuManager menuBarUI;

        // �I�v�V������ʂ��J���Ă��邩(UI�{�^���̕\���Ɏg�p����)
        public bool hasOptionScreen { get; private set; }
        // �ݒ�ύX����(�Q�[���̑��삪�ł��Ȃ��Ȃ�)
        public bool hasOptionMode { get; private set; }
        // �ݒ肪�J���Ȃ���Ԃ�(���[����]����I�[�g���s���͐ݒ���J���Ȃ�)
        public bool lockOptionMode { get; private set; }

        private void Awake()
        {
            hasOptionScreen = false;
            hasOptionMode = false;
            lockOptionMode = false;
            openButton.ButtonPushedEvent += ToggleOptionScreen;
            menuBarUI.OnPressedMenuEvent += EnterOptionMode;
            menuBarUI.OnClosedScreenEvent += DisableOptionMode;
        }

        private void Start()
        {
            openButton.ToggleInteractive(true);
        }

        private void OnDestroy()
        {
            openButton.ButtonPushedEvent -= ToggleOptionScreen;
            menuBarUI.OnPressedMenuEvent -= EnterOptionMode;
            menuBarUI.OnClosedScreenEvent -= DisableOptionMode;
        }

        // func
        // �I�v�V������ʂ��J��
        public void ToggleOptionScreen()
        {
            Debug.Log("option clicked");
            menuBarUI.gameObject.SetActive(!menuBarUI.gameObject.activeSelf);
        }

        // ���b�N��Ԃ̐ݒ�
        public void ToggleOptionLock(bool value)
        {
            // �V�Z���̓{�^���������Ȃ��悤�ɂ���(�L���ɂȂ�͉̂�]��)
            lockOptionMode = value;
            menuBarUI.SetInteractiveAllButton(!value);
            Debug.Log("Lock:" + value);
        }

        // �I�v�V�������[�h�ɓ����
        void EnterOptionMode()
        {
            hasOptionMode = true;
            openButton.ToggleInteractive(false);
        }

        // �I�v�V�������[�h����
        void DisableOptionMode()
        {
            hasOptionMode = false;
            openButton.ToggleInteractive(true);
        }
    }
}
