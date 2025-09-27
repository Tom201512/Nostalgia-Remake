using ReelSpinGame_Option.Button;
using System;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    [Serializable]
    public abstract class OptionScreenBase : MonoBehaviour
    {
        // var
        // ���삪�ł��Ȃ���Ԃ�
        public bool CanInteract {  get; private set; }

        // �N���[�Y�{�^��
        [SerializeField] private ButtonComponent closeButton;

        // func

        private void Awake()
        {
            CanInteract = false;
            closeButton.ButtonPushedEvent += CloseScreen;
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
            InitializeScreen();
            Debug.Log(name + " Closed");
        }

        // �������p
        protected abstract void InitializeScreen();
    }
}
