using ReelSpinGame_Datas;
using ReelSpinGame_Medal;
using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Symbol;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelLogicManager;

namespace ReelSpinGame_Payout
{
    // 払い出しマネージャー
    public class PayoutManager : MonoBehaviour
    {
        // 払い出しモード
        public enum PayoutCheckMode { PayoutNormal, PayoutBIG, PayoutJAC };

        // 払い出しデータベース
        [SerializeField] private PayoutDatabase payoutDatabase;

        public PayoutResultBuffer LastPayoutResult { get; private set; }    // 最後に当たった結果
        public PayoutCheckMode CheckMode { get; set; }                      // 選択中のテーブル

        void Awake()
        {
            CheckMode = PayoutCheckMode.PayoutNormal;
            LastPayoutResult = new PayoutResultBuffer(0, 0, false);
        }

        // 払い出しラインを返す
        public List<PayoutLineData> GetPayoutLines() => payoutDatabase.PayoutLines;

        // 払い出しテーブルの変更
        public void ChangePayoutCheckMode(PayoutCheckMode checkMode) => CheckMode = checkMode;

        // 払い出しがあるかチェックする
        public void CheckPayouts(int betAmount, LastStoppedReelData lastStoppedData)
        {
            // 最終的な払い出し結果
            int finalPayout = 0;
            int bonusID = 0;
            bool replayStatus = false;
            List<PayoutLineData> finalPayoutLine = new List<PayoutLineData>();

            // 指定したラインごとにデータを得る
            foreach (PayoutLineData lineData in payoutDatabase.PayoutLines)
            {
                // 指定ラインの結果を保管
                List<ReelSymbols> lineResult = new List<ReelSymbols>();

                // ベット枚数の条件を満たしているかチェック
                if (betAmount >= lineData.BetCondition)
                {
                    // 各リールから指定ラインの位置を得る(枠下2段目を基準に)
                    int reelIndex = 0;
                    foreach (List<ReelSymbols> reelResult in lastStoppedData.LastSymbols)
                    {
                        // マイナス数値を配列番号に変換
                        int lineIndex = SymbolChange.GetReelArrayIndex(lineData.PayoutLines[reelIndex]);
                        lineResult.Add(reelResult[lineIndex]);
                        reelIndex += 1;
                    }

                    // 図柄構成リストと見比べて該当するものがあれば当選。払い出し、ボーナス、リプレイ処理もする。
                    // ボーナスは非当選でもストックされる
                    int foundIndex = CheckHasPayout(lineResult, GetPayoutResultData(CheckMode));

                    // データを追加(払い出しだけ当たった分追加する)
                    // 当たったデータがあれば記録(-1以外)
                    if (foundIndex != -1)
                    {
                        // 払い出しは常にカウント(15枚を超えても切り捨てられる)
                        finalPayout += GetPayoutResultData(CheckMode)[foundIndex].Payout;

                        // ボーナス未成立なら当たった時に変更
                        if (bonusID == 0)
                        {
                            bonusID = GetPayoutResultData(CheckMode)[foundIndex].BonusType;
                        }
                        // リプレイでなければ当たった時に変更
                        if (replayStatus == false)
                        {
                            replayStatus = GetPayoutResultData(CheckMode)[foundIndex].HasReplayOrJac;
                        }

                        // 当たったラインを記録
                        finalPayoutLine.Add(lineData);
                    }
                }
                // 条件を満たさない場合は終了
                else
                {
                    break;
                }
            }
            // 最終的な払い出し枚数をイベントに送る

            // デバッグ用
            for (int i = 0; i < finalPayoutLine.Count; i++)
            {
                string buffer = "";
                for (int j = 0; j < finalPayoutLine[i].PayoutLines.Count; j++)
                {
                    buffer += finalPayoutLine[i].PayoutLines[j].ToString();

                    if (j != finalPayoutLine[i].PayoutLines.Count - 1)
                    {
                        buffer += ",";
                    }
                }
            }

            // 最大払い出しを超える枚数だった場合は切り捨てる
            if (finalPayout > MedalBehavior.MaxPayout)
            {
                finalPayout = MedalBehavior.MaxPayout;
            }

            LastPayoutResult.Payout = finalPayout;
            LastPayoutResult.BonusID = bonusID;
            LastPayoutResult.IsReplayOrJacIn = replayStatus;
            LastPayoutResult.PayoutLines = finalPayoutLine;
        }

        // 図柄の判定(配列を返す)
        int CheckHasPayout(List<ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
        {
            // 全て同じ図柄が揃っていたらHITを返す
            // ANY(10番)は無視

            // 見つかったデータの位置
            int indexNum = 0;

            // 払い出し結果の判定
            foreach (PayoutResultData data in payoutResult)
            {
                // 同じ図柄をカウントする
                int sameSymbolCount = 0;

                // 図柄のチェック
                for (int i = 0; i < data.Combination.Count; i++)
                {
                    // 図柄が合っているかチェック(ANYなら次の図柄へ)
                    if (data.Combination[i] == PayoutResultData.AnySymbol ||
                        (byte)lineResult[i] == data.Combination[i])
                    {
                        sameSymbolCount += 1;
                    }
                }

                // 同じ図柄(ANY含め)がリールの数と合えば当選とみなす
                if (sameSymbolCount == ReelAmount)
                {
                    // 配列番号を送る
                    return indexNum;
                }
                // なかった場合は次の番号へ
                indexNum += 1;
            }
            // 見つからない場合は-1を返す(はずれとなる)
            return -1;
        }

        // 払い出し結果をテーブルごとに得る
        List<PayoutResultData> GetPayoutResultData(PayoutCheckMode payoutCheckMode)
        {
            switch (payoutCheckMode)
            {
                case PayoutCheckMode.PayoutNormal:
                    return payoutDatabase.NormalPayoutDatas;

                case PayoutCheckMode.PayoutBIG:
                    return payoutDatabase.BigPayoutDatas;

                case PayoutCheckMode.PayoutJAC:
                    return payoutDatabase.JacPayoutDatas;
            }
            return null;
        }
    }
}
