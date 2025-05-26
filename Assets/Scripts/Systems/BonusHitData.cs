using ReelSpinGame_Bonus;
using static ReelSpinGame_Bonus.BonusBehaviour;

namespace ReelSpinGame_Datas
{
    // 当選ボーナス情報
    public class BonusHitData
    {
        // var
        // 当選ボーナスID
        public BonusType BonusID { get; private set; }
        // 成立時ゲーム数(成立時点でのゲーム数)
        public int BonusHitGame { get; private set; }
        // 入賞時ゲーム数(ボーナスを入賞させたゲーム)
        public int BonusStartGame { get; private set; }
        // ボーナス獲得枚数
        public int BonusPayouts { get; private set; }

        // コンストラクタ
        public BonusHitData(BonusType bonusID)
        {
            BonusID = bonusID;
            BonusHitGame = 0;
            BonusStartGame = 0;
            BonusPayouts = 0;
        }

        // セーブから読み込む場合
        public BonusHitData(BonusType bonusID, int bonusHitGame, int bonusStartGame, int bonusPayouts) : this(bonusID)
        {
            BonusHitGame = bonusHitGame;
            BonusStartGame = bonusStartGame;
            BonusPayouts = bonusPayouts;
        }

        // func
        // 成立時ゲーム数セット
        public void SetBonusHitGame(int game) => BonusHitGame = game;
        // 入賞時ゲーム数セット
        public void SetBonusStartGame(int game) => BonusStartGame = game;
        // ボーナス獲得枚数変更
        public void ChangeBonusPayouts(int amounts) => BonusPayouts += amounts;
    }
}