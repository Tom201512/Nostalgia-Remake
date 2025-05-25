using ReelSpinGame_Datas;
using UnityEngine;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // �t���O���I
        // var
        // �t���O�̏���
        public FlagBehaviour FlagBehaviour { get; private set; }

        // �t���O�f�[�^�x�[�X
        [SerializeField] FlagDatabase flagDatabase;


        // �f�o�b�O�p(������)
        [SerializeField] private bool useInstant;
        [SerializeField] private FlagId instantFlagID;

        void Awake()
        {
            FlagBehaviour = new FlagBehaviour();
        }

        public void StartFlagLots(int setting, int betAmounts)
        {
            FlagBehaviour.GetFlagLots(setting, betAmounts, useInstant, instantFlagID, flagDatabase);
        }
    }
}
