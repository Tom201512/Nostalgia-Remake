using ReelSpinGame_Option.Button;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Option
{
    public class OptionManager : MonoBehaviour
    {
        // �I�v�V�����}�l�[�W���[

        // const

        // var

        // �I�v�V�������j���[�J�{�^��
        [SerializeField] private ButtonComponent openButton;
        // �I�v�V�������j���[UI
        [SerializeField] private GameObject optionUIScreen;

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
        }

        private void Start()
        {
            openButton.buttonPushedEvent += ToggleOptionScreen;
        }

        // func
        // �I�v�V������ʂ��J��
        public void ToggleOptionScreen()
        {
            if (!lockOptionMode)
            {
                hasOptionScreen = !hasOptionScreen;
                hasOptionMode = !hasOptionMode;
                optionUIScreen.SetActive(hasOptionScreen);

                Debug.Log("Option:" + hasOptionMode);
            }
            else
            {
                Debug.LogWarning("Can't activate option because option mode is locked");
            }
        }

        // �I�v�V�������b�N�ݒ�
        public void LockOptionButton(bool value)
        {
            lockOptionMode = value;
            openButton.ToggleInteractive(lockOptionMode);

            Debug.Log("Lock:" + lockOptionMode);
        }
    }
}
