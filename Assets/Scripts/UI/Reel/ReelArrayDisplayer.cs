using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Util;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_UI.Reel
{
    // リール配列の表示
    public class ReelArrayDisplayer : MonoBehaviour
    {
        [SerializeField] ReelObjectPresenter reelObject;    // リールオブジェクト

        private Image[] symbolImages;                       // 図柄部分

        void Awake()
        {
            // 図柄を読み込む
            symbolImages = GetComponentsInChildren<Image>();
        }

        void Start()
        {
            int lowerPos = 0;

            foreach (Image symbol in symbolImages)
            {
                Sprite sprite = reelObject.GetReelSymbolSprite(ReelSymbolPosCalc.OffsetReelPos(lowerPos, (int)ReelPosID.Lower));
                symbol.sprite = sprite;
                lowerPos += 1;
            }
        }
    }
}
