using ReelSpinGame_Reels;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_UI.Reel
{
    public class SymbolDisplayUI : MonoBehaviour
    {
        [SerializeField] private ReelPosID posID;        // リール位置識別ID

        private Image symbolImage;                       // 表示部分

        void Awake()
        {
            symbolImage = GetComponent<Image>();
        }

        // 図柄変更
        public void ChangeSymbol(Sprite symbolSprite) => symbolImage.sprite = symbolSprite;

        // 位置IDを返す
        public ReelPosID GetPosID() => posID;
    }
}
