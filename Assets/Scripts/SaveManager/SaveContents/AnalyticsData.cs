using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Datas.Analytics
{
    // 解析情報
    public class AnalyticsData : ISavable
    {
        // var
        public int TotalAllGamesCount { get; private set; }         // 小役、JAC、通常時を含めた全ゲーム回転数
        public int BigGamesCount { get; private set; }              // BIG小役ゲーム中のゲーム数
        public int JacGamesCount { get; private set; }              // JACゲーム中のゲーム数

        // 通常時各小役の入賞回数(リプレイを除く)

        // 成立
        public int NormalBellHitCount { get; private set; }         // ベル
        public int NormalMelonHitCount { get; private set; }        // スイカ
        public int NormalCherry2HitCount { get; private set; }      // 2枚チェリー
        public int NormalCherry4HitCount { get; private set; }      // 4枚チェリー

        // 入賞
        public int NormalBellLineUpCount { get; private set; }      // ベル
        public int NormalMelonLineUpCount { get; private set; }     // スイカ
        public int NormalCherry2LineUpCount { get; private set; }   // 2枚チェリー
        public int NormalCherry4LineUpCount { get; private set; }   // 4枚チェリー

        // BIG中成立
        public int BigBellHitCount { get; private set; }        // ベル
        public int BigMelonHitCount { get; private set; }       // スイカ
        public int BigCherry2HitCount { get; private set; }     // 2枚チェリー
        public int BigCherry4HitCount { get; private set; }     // 4枚チェリー

        // BIG中入賞
        public int BigBellLineUpCount { get; private set; }         // ベル
        public int BigMelonLineUpCount { get; private set; }        // スイカ
        public int BigCherry2LineUpCount { get; private set; }      // 2枚チェリー
        public int BigCherry4LineUpCount { get; private set; }      // 4枚チェリー

        // ビタ押し精度用
        public int BigJacInTimes { get; private set; }              // JAC-IN発生回数
        public int BigJacAvoidTimes { get; private set; }           // JACはずし成功回数
        public int BigJacPerfectAvoidTimes { get; private set; }    // JACビタはずし成功回数
        public int BigJacAssistedAvoidTimes { get; private set; }   // アシストはずし成功回数

        public int JacGameNoneTimes { get; private set; }           // JAC中はずれ回数

        // コンストラクタ
        public AnalyticsData()
        {
            TotalAllGamesCount = 0;
            BigGamesCount = 0;
            JacGamesCount = 0;

            NormalBellHitCount = 0;
            NormalMelonHitCount = 0;
            NormalCherry2HitCount = 0;
            NormalCherry4HitCount = 0;

            NormalBellLineUpCount = 0;
            NormalMelonLineUpCount = 0;
            NormalCherry2LineUpCount = 0;
            NormalCherry4LineUpCount = 0;

            BigBellHitCount = 0;
            BigMelonHitCount = 0;
            BigCherry2HitCount = 0;
            BigCherry4HitCount = 0;


            BigBellLineUpCount = 0;
            BigMelonLineUpCount = 0;
            BigCherry2LineUpCount = 0;
            BigCherry4LineUpCount = 0;

            BigJacInTimes = 0;
            BigJacAvoidTimes = 0;
            BigJacPerfectAvoidTimes = 0;
            BigJacAssistedAvoidTimes = 0;

            JacGameNoneTimes = 0;
        }

        // func
        // トータルゲーム数加算
        public void IncreaseTotalAllGameCounts(BonusStatus bonusStatus)
        {
            TotalAllGamesCount += 1;
            if(bonusStatus == BonusStatus.BonusBIGGames)
            {
                BigGamesCount += 1;
            }
            else if(bonusStatus == BonusStatus.BonusJACGames)
            {
                JacGamesCount += 1;
            }
        }

        // 小役成立カウント増加
        public void IncreaseHitCountByFlag(FlagID flagID, BonusStatus bonusStatus)
        {
            if(bonusStatus == BonusStatus.BonusBIGGames)
            {
                switch (flagID)
                {
                    case FlagID.FlagCherry2:
                        BigCherry2HitCount += 1;
                        break;
                    case FlagID.FlagCherry4:
                        BigCherry4HitCount += 1;
                        break;
                    case FlagID.FlagMelon:
                        BigMelonHitCount += 1;
                        break;
                    case FlagID.FlagBell:
                        BigBellHitCount += 1;
                        break;

                    // BIG中はJAC-IN成立も含める(入賞はカウントしない)
                    case FlagID.FlagReplayJacIn:
                        BigJacInTimes += 1;
                        break;
                }
            }
            else if(bonusStatus == BonusStatus.BonusJACGames)
            {
                if(flagID == FlagID.FlagNone)
                {
                    JacGameNoneTimes += 1;
                }
            }
            else
            {
                switch (flagID)
                {
                    case FlagID.FlagCherry2:
                        NormalCherry2HitCount += 1;
                        break;
                    case FlagID.FlagCherry4:
                        NormalCherry4HitCount += 1;
                        break;
                    case FlagID.FlagMelon:
                        NormalMelonHitCount += 1;
                        break;
                    case FlagID.FlagBell:
                        NormalBellHitCount += 1;
                        break;
                }
            }
        }

        // 小役入賞カウント増加
        public void IncreaseLineUpCountByFlag(FlagID flagID, BonusStatus bonusStatus)
        {
            if (bonusStatus == BonusStatus.BonusBIGGames)
            {
                switch (flagID)
                {
                    case FlagID.FlagCherry2:
                        BigCherry2LineUpCount += 1;
                        break;
                    case FlagID.FlagCherry4:
                        BigCherry4LineUpCount += 1;
                        break;
                    case FlagID.FlagMelon:
                        BigMelonLineUpCount += 1;
                        break;
                    case FlagID.FlagBell:
                        BigBellLineUpCount += 1;
                        break;
                }
            }

            else
            {
                switch (flagID)
                {
                    case FlagID.FlagCherry2:
                        NormalCherry2LineUpCount += 1;
                        break;
                    case FlagID.FlagCherry4:
                        NormalCherry4LineUpCount += 1;
                        break;
                    case FlagID.FlagMelon:
                        NormalMelonLineUpCount += 1;
                        break;
                    case FlagID.FlagBell:
                        NormalBellLineUpCount += 1;
                        break;
                }
            }
        }

        // ビタハズシ記録
        public void CountJacAvoidCounts(int leftPushedPos, int random)
        {
            // ビタ押しでの判定
            if(leftPushedPos == 10 || leftPushedPos == 16)
            {
                BigJacAvoidTimes += 1;
                BigJacPerfectAvoidTimes += 1;
            }
            // アシスト入りで外したかのチェック(乱数が偶数であること)
            else if((leftPushedPos == 9 || leftPushedPos == 15) &&
                random % 2 == 0)
            {
                BigJacAvoidTimes += 1;
                BigJacAssistedAvoidTimes += 1;
            }
        }

        // データ書き込み
        public List<int> SaveData()
        {
            // 変数を格納
            List<int> data = new List<int>();
            data.Add(TotalAllGamesCount);
            data.Add(BigGamesCount);
            data.Add(JacGamesCount);

            data.Add(NormalBellHitCount);
            data.Add(NormalMelonHitCount);
            data.Add(NormalCherry2HitCount);
            data.Add(NormalCherry4HitCount);

            data.Add(NormalBellLineUpCount);
            data.Add(NormalMelonLineUpCount);
            data.Add(NormalCherry2LineUpCount);
            data.Add(NormalCherry4LineUpCount);

            data.Add(BigBellHitCount);
            data.Add(BigMelonHitCount);
            data.Add(BigCherry2HitCount);
            data.Add(BigCherry4HitCount);

            data.Add(BigBellLineUpCount);
            data.Add(BigMelonLineUpCount);
            data.Add(BigCherry2LineUpCount);
            data.Add(BigCherry4LineUpCount);

            data.Add(BigJacInTimes);
            data.Add(BigJacAvoidTimes);
            data.Add(BigJacPerfectAvoidTimes);
            data.Add(BigJacAssistedAvoidTimes);

            data.Add(JacGameNoneTimes);

            return data;
        }

        // データ読み込み
        public bool LoadData(BinaryReader br)
        {
            try
            {
                TotalAllGamesCount = br.ReadInt32();
                BigGamesCount = br.ReadInt32();
                JacGamesCount = br.ReadInt32();

                NormalBellHitCount = br.ReadInt32();
                NormalMelonHitCount = br.ReadInt32();
                NormalCherry2HitCount = br.ReadInt32();
                NormalCherry4HitCount = br.ReadInt32();

                NormalBellLineUpCount = br.ReadInt32();
                NormalMelonLineUpCount = br.ReadInt32();
                NormalCherry2LineUpCount = br.ReadInt32();
                NormalCherry4LineUpCount = br.ReadInt32();

                BigBellHitCount = br.ReadInt32();
                BigMelonHitCount = br.ReadInt32();
                BigCherry2HitCount = br.ReadInt32();
                BigCherry4HitCount = br.ReadInt32();

                BigBellLineUpCount = br.ReadInt32();
                BigMelonLineUpCount = br.ReadInt32();
                BigCherry2LineUpCount = br.ReadInt32();
                BigCherry4LineUpCount = br.ReadInt32();

                BigJacInTimes = br.ReadInt32();
                BigJacAvoidTimes = br.ReadInt32();
                BigJacPerfectAvoidTimes = br.ReadInt32();
                BigJacAssistedAvoidTimes = br.ReadInt32();

                JacGameNoneTimes = br.ReadInt32();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }
    }
}