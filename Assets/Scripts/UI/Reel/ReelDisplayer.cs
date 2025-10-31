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
        // �}������
        private SymbolDisplayUI[] reelSymbols;

        void Awake()
        {
            reelSymbols = GetComponentsInChildren<SymbolDisplayUI>();
        }

        // ���̈ʒu����Ƀ��[���}����\��
        public void DisplayReel(int lowerPos, Sprite sprite)
        {
            foreach(SymbolDisplayUI symbol in reelSymbols)
            {
                //Debug.Log("Pos:" + ReelObjectModel.OffsetReel(lowerPos, (int)symbol.GetPosID()));
                symbol.ChangeSymbol(sprite);
            }
        }
    }
}
