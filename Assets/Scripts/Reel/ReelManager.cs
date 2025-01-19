using ReelSpinGame_Reels;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // 目指す目標

    // 1:リールの回転
    // 2:図柄の実装
    // 3:リールの停止
    // 4:スベリ実装
    // 5:テーブル機能搭載

    // マネージャーが持つもの

    // 各リールのデータ(3つ)
    // 全リールへのコントロール

    // const 
    public const int MaxReels = 3;

    public enum ReelID { ReelLeft, ReelMiddle, ReelRight };

    [SerializeField] private SymbolSpin[] symbolSpins;

    // var

    // リール
    private ReelData[] reels;

    void Awake()
    {
        reels = new ReelData[MaxReels];

        reels[(int)ReelID.ReelLeft] = new ReelData(0, ReelSpinGame_Reels.Array.ReelArray.LeftReelArray.ToArray());
        reels[(int)ReelID.ReelMiddle] = new ReelData(1, ReelSpinGame_Reels.Array.ReelArray.MiddleReelArray.ToArray());
        reels[(int)ReelID.ReelRight] = new ReelData(2, ReelSpinGame_Reels.Array.ReelArray.RightReelArray.ToArray());
    }

    // func
}
