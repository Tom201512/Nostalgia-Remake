using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.Flash.FlashManager;

namespace ReelSpinGame_Effect
{
    public class EffectManager : MonoBehaviour
    {
        // リールフラッシュやサウンドの管理

        // const

        // var
        // フラッシュ機能
        private FlashManager flashManager;
        // リールのオブジェクト
        [SerializeField] private List<ReelObject> reelObjects;

        // func 
        private void Awake()
        {
            flashManager = GetComponent<FlashManager>();
        }

        private void Start()
        {
            // リールオブジェクト割り当て
            flashManager.SetReelObjects(reelObjects);
        }

        // フラッシュで待機中か
        public bool GetHasFlashWait() => flashManager.HasFlashWait;
        // フラッシュ関連
        // リール全点灯
        public void TurnOnAllReels(bool isJacGame)
        {
            // JAC GAME中は中段のみ光らせる
            if (isJacGame)
            {
                flashManager.EnableJacGameLight();
            }
            else
            {
                flashManager.TurnOnAllReels();
            }

            // JAC中のライト処理をする
            foreach (ReelObject reel in reelObjects)
            {
                if (reel.HasJacModeLight != isJacGame)
                {
                    reel.HasJacModeLight = isJacGame;
                }
            }
        }

        // リールライト全消灯
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();
        // リールフラッシュを開始させる
        public void StartReelFlash(FlashID flashID) => flashManager.StartReelFlash(flashID);
        // 払い出しのリールフラッシュを開始させる
        public void StartPayoutReelFlash(float waitSeconds, List<PayoutLineData> lastPayoutLines) => flashManager.StartPayoutFlash(waitSeconds, lastPayoutLines);
        // フラッシュ停止
        public void StopReelFlash() => flashManager.StopFlash();
    }
}
