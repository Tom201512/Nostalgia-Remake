using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Medal.Payout
{
    public class PayoutChecker : MonoBehaviour
    {
        // 図柄の判定用

        // const
        public enum PayoutCheckMode { PayoutNormal, PayoutBIG, PayoutJAC };

        // var
        // 払い出し結果
        public class PayoutResultBuffer
        {
            public int Payouts { get; private set; }
            public int BonusID { get; private set; }
            public bool IsReplayOrJacIn { get; private set; }

            // 払い出しのあったライン
            public List<PayoutLineData> PayoutLines { get; private set; }

            public PayoutResultBuffer(int payouts, int bonusID, bool isReplayOrJac)
            {
                Payouts = payouts;
                BonusID = bonusID;
                IsReplayOrJacIn = isReplayOrJac;
                PayoutLines = new List<PayoutLineData>();
            }

            public void SetPayout(int payouts) => Payouts = payouts;
            public void SetBonusID(int bonusID) => BonusID = bonusID;
            public void SetReplayStatus(bool isReplayOrJac) => IsReplayOrJacIn = isReplayOrJac;
            public void SetPayoutLines(List<PayoutLineData> PayoutLines) => this.PayoutLines = PayoutLines;
        }

        // 払い出しデータベース
        [SerializeField] private PayoutDatabase payoutDatabase;

        // 最後に当たった結果
        public PayoutResultBuffer LastPayoutResult { get; private set; }
        public PayoutDatabase PayoutDatabase { get {return payoutDatabase; } }

        // 選択中のテーブル
        public PayoutCheckMode CheckMode { get; private set; }

        private void Awake()
        {
            // 最後に判定した時の結果
            CheckMode = PayoutCheckMode.PayoutNormal;
            LastPayoutResult = new PayoutResultBuffer(0, 0, false);
        }

        // func
        //判定モード変更
        public void ChangePayoutCheckMode(PayoutCheckMode checkMode) => CheckMode = checkMode;

        // ライン判定
        public void CheckPayoutLines(int betAmount, List<List<ReelData.ReelSymbols>> lastSymbols)
        {
            // 最終的な払い出し結果
            int finalPayouts = 0;
            int bonusID = 0;
            bool replayStatus = false;
            List<PayoutLineData> finalPayoutLine = new List<PayoutLineData>();

            // 指定したラインごとにデータを得る
            foreach (PayoutLineData lineData in payoutDatabase.PayoutLines)
            {
                // 指定ラインの結果を保管
                List<ReelData.ReelSymbols> lineResult = new List<ReelData.ReelSymbols>();

                // ベット枚数の条件を満たしているかチェック
                if (betAmount >= lineData.BetCondition)
                {
                    // 各リールから指定ラインの位置を得る(枠下2段目を基準に)
                    int reelIndex = 0;
                    foreach (List<ReelData.ReelSymbols> reelResult in lastSymbols)
                    {
                        // マイナス数値を配列番号に変換
                        int lineIndex = ReelData.GetReelArrayIndex(lineData.PayoutLines[reelIndex]);

                        Debug.Log("Symbol:" + reelResult[lineIndex]);
                        lineResult.Add(reelResult[lineIndex]);
                        reelIndex += 1;
                    }

                    // 図柄構成リストと見比べて該当するものがあれば当選。払い出し、ボーナス、リプレイ処理もする。
                    // ボーナスは非当選でもストックされる
                    int foundIndex = CheckPayoutLines(lineResult, GetPayoutResultData(CheckMode));

                    // データを追加(払い出しだけ当たった分追加する)
                    // 当たったデータがあれば記録(-1以外)
                    if (foundIndex != -1)
                    {
                        // 払い出しは常にカウント(15枚を超えても切り捨てられる)
                        finalPayouts += GetPayoutResultData(CheckMode)[foundIndex].Payouts;

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

            Debug.Log("payout:" + finalPayouts);
            Debug.Log("Bonus:" + bonusID);
            Debug.Log("IsReplay:" + replayStatus);

            // デバッグ用
            for(int i = 0; i < finalPayoutLine.Count; i++)
            {
                string buffer = "";
                for(int j = 0; j < finalPayoutLine[i].PayoutLines.Count; j++)
                {
                    buffer += finalPayoutLine[i].PayoutLines[j].ToString();

                    if(j != finalPayoutLine[i].PayoutLines.Count - 1)
                    {
                        buffer += ",";
                    }
                }
                Debug.Log("PayoutLines" + i + ":" + buffer);
            }

            LastPayoutResult.SetPayout(finalPayouts);
            LastPayoutResult.SetBonusID(bonusID);
            LastPayoutResult.SetReplayStatus(replayStatus);
            LastPayoutResult.SetPayoutLines(finalPayoutLine);
        }

        // 図柄の判定(配列を返す)
        private int CheckPayoutLines(List<ReelData.ReelSymbols> lineResult, List<PayoutResultData> payoutResult)
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
                for (int i = 0; i < data.Combinations.Count; i++)
                {
                    // 図柄が合っているかチェック(ANYなら次の図柄へ)
                    if (data.Combinations[i] == PayoutResultData.AnySymbol ||
                        (byte)lineResult[i] == data.Combinations[i])
                    {
                        sameSymbolCount += 1;
                    }
                }

                // 同じ図柄(ANY含め)がリールの数と合えば当選とみなす
                if (sameSymbolCount == ReelManager.ReelAmounts)
                {
                    Debug.Log("HIT!:" + payoutResult[indexNum].Payouts + "Bonus:"
                     + payoutResult[indexNum].BonusType + "Replay:" + payoutResult[indexNum].HasReplayOrJac);

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
        private List<PayoutResultData> GetPayoutResultData(PayoutCheckMode payoutCheckMode)
        {
            Debug.Log(payoutCheckMode.ToString());
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
