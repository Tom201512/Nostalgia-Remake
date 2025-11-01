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

        // �w�肵�����̈ʒu����Ƀ��[���}����\��
        public void DisplayReel(int lowerPos, ReelObjectPresenter reelObjectPresetner)
        {
            foreach(SymbolDisplayUI symbol in reelSymbols)
            {
                Sprite sprite = reelObjectPresetner.GetReelSymbolSprite(ReelObjectPresenter.OffsetReelPos(lowerPos, (int)symbol.GetPosID()));
                symbol.ChangeSymbol(sprite);
            }
        }
    }
}
