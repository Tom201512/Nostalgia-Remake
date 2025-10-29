using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayer : MonoBehaviour
    {
        // ���[�����ʂ�\������

        // var
        // �\�������郊�[��
        [SerializeField] ReelObject reel;
        // �}������
        private SymbolDisplayUI[] reelSymbols;

        void Awake()
        {
            reelSymbols = GetComponentsInChildren<SymbolDisplayUI>();
        }

        // ���̈ʒu����Ƀ��[���}����\��
        public void DisplayReel(int lowerPos)
        {
            foreach(SymbolDisplayUI symbol in reelSymbols)
            {
                Debug.Log("Pos:" + ReelData.OffsetReel(lowerPos, (int)symbol.GetPosID()));
                symbol.ChangeSymbol(reel.GetSymbolImageAtPos(ReelData.OffsetReel(lowerPos, (int)symbol.GetPosID())));
            }
        }
    }
}
