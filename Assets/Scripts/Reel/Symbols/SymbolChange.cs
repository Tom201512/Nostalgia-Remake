using UnityEngine;

namespace ReelSpinGame_Reels.Symbol
{
    // 図柄変更のコンポーネント
    public class SymbolChange : MonoBehaviour
    {
        [SerializeField] private ReelPosID posID;       // リール位置識別ID

        private SpriteRenderer sprite;      // 表示部分

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        // 図柄変更
        public void ChangeSymbol(Sprite symbolSprite) => sprite.sprite = symbolSprite;

        // 位置IDを返す
        public ReelPosID GetPosID() => posID;
    }
}
