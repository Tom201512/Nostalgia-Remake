using ReelSpinGame_Reels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayer : MonoBehaviour
    {
        // リール結果を表示する

        // var
        // 図柄部分
        private SymbolDisplayUI[] reelSymbols;
        // コマ表示用フォント
        [SerializeField] private List<TextMeshProUGUI> reelPosTexts;
        // スベリコマ表示用フォント
        [SerializeField] private TextMeshProUGUI reelDelayText;

        void Awake()
        {
            reelSymbols = GetComponentsInChildren<SymbolDisplayUI>();
        }

        // 指定した下の位置を基準にリール図柄を表示
        public void DisplayReelSymbols(int lowerPos, ReelObjectPresenter reelObjectPresetner)
        {
            foreach(SymbolDisplayUI symbol in reelSymbols)
            {
                Sprite sprite = reelObjectPresetner.GetReelSymbolSprite(ReelObjectPresenter.OffsetReelPos(lowerPos, (int)symbol.GetPosID()));
                symbol.ChangeSymbol(sprite);
            }
        }

        // 停止位置とスベリコマを表示させる
        public void DisplayPos(int lowerPos)
        {
            // 図柄位置のフォント表示
            for (int i = 0; i < reelPosTexts.Count; i++)
            {
                string pos = (ReelObjectPresenter.OffsetReelPos(lowerPos, i) + 1).ToString();
                reelPosTexts[i].text = pos;
            }
        }

        // スベリコマ数
        public void DisplayDelay(int delay)
        {
            reelDelayText.text = "Delay:[" + delay + "]";
        }
    }
}
