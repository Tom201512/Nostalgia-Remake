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
        public FlagBehaviour Data { get; private set; }

        // �t���O�f�[�^�x�[�X
        [SerializeField] FlagDatabase flagDatabase;


        // �f�o�b�O�p(������)
        [SerializeField] private bool useInstant;
        [SerializeField] private FlagId instantFlagID;

        void Awake()
        {
            Data = new FlagBehaviour();
        }

        public void StartFlagLots(int setting, int betAmounts)
        {
            Data.GetFlagLots(setting, betAmounts, useInstant, instantFlagID, flagDatabase);
        }
    }
}
