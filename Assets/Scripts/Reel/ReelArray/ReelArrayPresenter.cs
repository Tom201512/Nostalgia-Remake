using UnityEngine;

namespace ReelSpinGame_Reels.Array
{
    public class ReelArrayPresenter : MonoBehaviour
    {
        // リール配列プレゼンター

        // var
        // リール配列モデル
        private ReelArrayModel reelArrayModel;

        // 図柄マネージャー
        private SymbolManager symbolManager;

        void Awake()
        {
            reelArrayModel = new ReelArrayModel();
            symbolManager = GetComponentInChildren<SymbolManager>();
        }

        // リール配列プレゼンターの初期化
        public void SetReelArrayPresenter(byte[] reelArray)
        {
            reelArrayModel.ReelArray = reelArray;
        }

        // 指定位置からのリール位置を得る
        public int GetReelPos(int currentLower, sbyte posID) => ReelObjectPresenter.OffsetReelPos(currentLower, posID);

        // 図柄位置の更新
        public void UpdateReelSymbols(int currentLower) => symbolManager.UpdateSymbolsObjects(currentLower, reelArrayModel.ReelArray);
    }
}
