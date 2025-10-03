using ReelSpinGame_Option.Button;
using System;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    [Serializable]
    public abstract class OptionScreenBase : MonoBehaviour
    {
        // var
        // ���삪�ł����Ԃ�(�A�j���[�V�������Ȃǂ͂��Ȃ�����)
        public bool CanInteract {  get; private set; }

        // �N���[�Y�{�^��
        [SerializeField] private ButtonComponent closeButton;

        // ��ʂ������Ƃ��̃C�x���g
        public delegate void OnClosedWindow();
        public event OnClosedWindow OnClosedWindowEvent;

        // func

        private void Awake()
        {
            CanInteract = false;
            closeButton.ButtonPushedEvent += CloseScreen;
        }

        void Start()
        {
            closeButton.ToggleInteractive(true);
        }

        // ����\��Ԃ̃R���g���[��
        public void SetCanInteract(bool value) => CanInteract = value;

        // ��ʕ\��&������
        public void OpenScreen()
        {
            gameObject.SetActive(true);
            InitializeScreen();
            Debug.Log(name + " Opened");
        }

        // ��ʂ����
        public void CloseScreen()
        {
            gameObject.SetActive(false);
            Debug.Log(name + " Closed");
            OnClosedWindowEvent.Invoke();
        }

        // �������p
        protected abstract void InitializeScreen();
    }
}
