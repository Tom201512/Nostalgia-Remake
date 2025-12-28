using System;
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

        // リール配列の番号を図柄へ変更
        public static ReelSymbols ReturnSymbol(byte symbolID) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), symbolID);
        // リール位置を配列要素に置き換える
        public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
    }
}
