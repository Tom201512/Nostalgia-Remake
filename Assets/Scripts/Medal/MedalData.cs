using UnityEngine;
using System.Collections;

namespace ReelSpinGame_Datas
{
    // メダル情報
    public class MedalData
    {
        // const
        // プレイヤー初期メダル枚数
        public const int DefaultPlayerMedal = 50;

        // var
        // 所有メダル枚数
        public int CurrentPlayerMedal { get; private set; }
        // 投入したメダル枚数(IN)
        public int CurrentInMedal { get; private set; }
        // 獲得メダル枚数(OUT)
        public int CurrentOutMedal { get; private set; }

        // コンストラクタ
        public MedalData()
        {
            CurrentPlayerMedal = DefaultPlayerMedal;
            CurrentInMedal = 0;
            CurrentOutMedal = 0;
        }

        // セーブから読み込む場合
        public MedalData(int currentPlayerMedal, int currentInMedal, int currentOutMedal)
        {
            CurrentPlayerMedal = currentPlayerMedal;
            CurrentInMedal = currentInMedal;
            CurrentOutMedal = currentOutMedal;
        }

        // func

        // プレイヤーメダル増加
        public void IncreasePlayerMedal(int amounts) => CurrentPlayerMedal += amounts;
        // プレイヤーメダル減少
        public void DecreasePlayerMedal(int amounts)
        {
            CurrentPlayerMedal -= amounts;

            // 0になったら50追加
            while (CurrentPlayerMedal < 0)
            {
                CurrentPlayerMedal += DefaultPlayerMedal;
            }
        }

        // IN増加
        public void IncreaseInMedal(int amounts) => CurrentInMedal += amounts;
        // OUT増加
        public void IncreaseOutMedal(int amounts) => CurrentOutMedal += amounts;
    }
}