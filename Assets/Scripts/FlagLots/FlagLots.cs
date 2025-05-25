using ReelSpinGame_Datas;
using UnityEngine;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // フラグ抽選
        // var
        // フラグの処理
        public FlagBehaviour FlagBehaviour { get; private set; }

        // フラグデータベース
        [SerializeField] FlagDatabase flagDatabase;


        // デバッグ用(強制役)
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
