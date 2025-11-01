using ReelSpinGame_Reels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayer : MonoBehaviour
    {
        // ���[�����ʂ�\������

        // var
        // �}������
        private SymbolDisplayUI[] reelSymbols;
        // �R�}�\���p�t�H���g
        [SerializeField] private List<TextMeshProUGUI> reelPosTexts;
        // �X�x���R�}�\���p�t�H���g
        [SerializeField] private TextMeshProUGUI reelDelayText;

        void Awake()
        {
            reelSymbols = GetComponentsInChildren<SymbolDisplayUI>();
        }

        // �w�肵�����̈ʒu����Ƀ��[���}����\��
        public void DisplayReelSymbols(int lowerPos, ReelObjectPresenter reelObjectPresetner)
        {
            foreach(SymbolDisplayUI symbol in reelSymbols)
            {
                Sprite sprite = reelObjectPresetner.GetReelSymbolSprite(ReelObjectPresenter.OffsetReelPos(lowerPos, (int)symbol.GetPosID()));
                symbol.ChangeSymbol(sprite);
            }
        }

        // ��~�ʒu�ƃX�x���R�}��\��������
        public void DisplayPos(int lowerPos)
        {
            // �}���ʒu�̃t�H���g�\��
            for (int i = 0; i < reelPosTexts.Count; i++)
            {
                string pos = (ReelObjectPresenter.OffsetReelPos(lowerPos, i) + 1).ToString();
                reelPosTexts[i].text = pos;
            }
        }

        // �X�x���R�}��
        public void DisplayDelay(int delay)
        {
            reelDelayText.text = "Delay:[" + delay + "]";
        }
    }
}
