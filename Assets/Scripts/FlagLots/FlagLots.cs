using ReelSpinGame_Datass;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // フラグ抽選

        // const
        // 最大フラグ数
        const int MaxFlagLots = 16384;

        // enum
        // フラグID
        public enum FlagId { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJACin, FlagJAC }
        // フラグテーブル
        public enum FlagLotMode { NormalA, NormalB, BigBonus, JacGame };

        // var
        // フラグデータベース
        [SerializeField] FlagDatabase flagDatabase;
        // 現在フラグ(プロパティ)
        public FlagId CurrentFlag { get; private set; } = FlagId.FlagNone;
        // 参照するテーブルID
        public FlagLotMode CurrentTable { get; private set; } = FlagLotMode.NormalA;

        // 抽選順番(最終的に当選したフラグを参照するのに使う)
        private FlagId[] lotResultNormal = new FlagId[] 
        {
            FlagId.FlagBig,
            FlagId.FlagReg,
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin
        };

        // BIG CHANCE中
        private FlagId[] lotResultBig = new FlagId[] 
        {
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin
        };

        // func
        // テーブル変更
        public void ChangeTable(FlagLotMode mode)
        {
            Debug.Log("Changed mode:" + mode);
            CurrentTable = mode;
        }

        // フラグ抽選の開始
        public void GetFlagLots(int setting)
        {
            // 現在の参照テーブルをもとに抽選
            switch (CurrentTable)
            {
                case FlagLotMode.NormalA:
                    CurrentFlag = CheckResultByTable(setting, flagDatabase.NormalATable, lotResultNormal);
                    break;

                case FlagLotMode.NormalB:
                    CurrentFlag = CheckResultByTable(setting, flagDatabase.NormalBTable, lotResultNormal);
                    break;

                case FlagLotMode.BigBonus:
                    CurrentFlag = CheckResultByTable(setting, flagDatabase.BigTable, lotResultBig);
                    break;

                case FlagLotMode.JacGame:
                    CurrentFlag = BonusGameLots(flagDatabase.JacNonePoss);
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + CurrentFlag);
        }

        // 選択したフラグにする 強制役などでの使用
        public void SelectFlag(FlagId flagID)
        {
            Debug.Log("Flag:" + CurrentFlag);
            CurrentFlag = flagID;
        }

        // テーブルからフラグ判定
        private FlagId CheckResultByTable(int setting, FlagDataSets flagTable, FlagId[] lotResult)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;
            int flag = GetFlag();

            int index = 0;
            foreach(float f in flagTable.FlagDataBySettings[setting - 1].FlagTable)
            {
                //各役ごとに抽選
                flagCheckNum += Mathf.FloorToInt((float)MaxFlagLots / f);

                if (flag < flagCheckNum)
                {
                    return lotResult[index];
                }
                index += 1;
            }

            // 何も当たらなければ"はずれ"を返す
            return FlagId.FlagNone;
        }

        // BONUS GAME中の抽選
        private FlagId BonusGameLots(float nonePoss)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選。端数切捨て)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // はずれ抽選
            flagCheckNum = Mathf.FloorToInt((float)MaxFlagLots / nonePoss);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // 何も当たらなければ"JAC役"を返す
            return FlagId.FlagJAC;
        }

        // フラグ抽選
        private int GetFlag()
        {
            // 16384フラグを得る(0~16383)
            int flag = Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);
            return flag;
        }
    }
}
