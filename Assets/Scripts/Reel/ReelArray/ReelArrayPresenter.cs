using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

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

        void Start()
        {
            symbolManager.UpdateSymbolsObjects(reelArrayModel.CurrentLower, reelArrayModel.ReelArray);
        }

        void Update()
        {

        }

        // リール配列プレゼンターの初期化
        public void SetReelArrayPresenter(int currentLower, byte[] reelArray)
        {
            reelArrayModel.CurrentLower = currentLower;
            reelArrayModel.ReelArray = reelArray;
        }

        // 下段位置を動かす
        public void SetCurrentLower(int currentLower) => reelArrayModel.CurrentLower = currentLower;
    }
}
