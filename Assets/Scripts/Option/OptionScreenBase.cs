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
        public bool CanInteract { get; set; }

        // �N���[�Y�{�^��
        [SerializeField] private ButtonComponent closeButton;

        // ��ʂ�����Ƃ��̃C�x���g
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func

        private void Awake()
        {
            CanInteract = true;
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
            InitializeScreen();
            Debug.Log(name + " Opened");
        }

        // ��ʂ����
        public void CloseScreen()
        {
            Debug.Log("Interact :" + CanInteract);
            if(CanInteract)
            {
                CloseScreenBehavior();
                OnClosedScreenEvent?.Invoke();
                Debug.Log(name + " Closed");
            }
        }

        // �������p
        protected abstract void InitializeScreen();

        // �I���p
        protected abstract void CloseScreenBehavior();
    }
}
