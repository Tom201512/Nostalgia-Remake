using ReelSpinGame_Datas;
using UnityEngine;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // �t���O���I
        // var
        // �t���O���I���̃f�[�^
        private FlagBehaviour data;
        // �t���O�f�[�^�x�[�X
        [SerializeField] FlagDatabase flagDatabase;

        // �f�o�b�O�p(������)
        [SerializeField] private bool useInstant;
        [SerializeField] private bool useInfinityInstant;
        [SerializeField] private FlagId instantFlagID;

        // func
        void Awake()
        {
            data = new FlagBehaviour();
        }

        // �e���l�𓾂�
        // ���݂̃t���O
        public FlagId GetCurrentFlag() => data.CurrentFlag;
        // ���݂̃e�[�u��
        public FlagLotMode GetCurrentTable() => data.CurrentTable;
        // �J�E���^
        public int GetCounter() => data.FlagCounter.Counter;

        // ���l�ύX
        // �e�[�u���ύX
        public void ChangeTable(FlagLotMode mode) => data.CurrentTable = mode;

        // �t���O���I������
        public void StartFlagLots(int setting, int betAmounts)
        {
            if (useInstant)
            {
                // �������𔭓�������B���̌�͋�������؂�
                data.CurrentFlag = instantFlagID;

                // �f�o�b�O�p
                if(!useInfinityInstant)
                {
                    useInstant = false;
                }
            }
            else
            {
                data.GetFlagLots(setting, betAmounts, useInstant, instantFlagID, flagDatabase);
            }
        }

        // �����J�E���^����
        public void IncreaseCounter(int payoutAmounts) => data.FlagCounter.IncreaseCounter(payoutAmounts);

        // �����J�E���^����
        public void DecreaseCounter(int setting, int lastBetAmounts) =>data.FlagCounter.DecreaseCounter(setting, lastBetAmounts);

        // �J�E���^���Z�b�g
        public void ResetCounter() => data.FlagCounter.ResetCounter();
    }
}
