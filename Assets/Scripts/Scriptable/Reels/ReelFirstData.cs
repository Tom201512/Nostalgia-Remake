using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelFirstData : ReelBaseData
    {
        // const
        // 第一停止のTID読み込み位置
        private const int FirstPushTIDPos = ConditionMaxRead + 1;
        // 第一停止のCID読み込み位置
        private const int FirstPushCIDPos = FirstPushTIDPos + 1;

        // var
        // 第一停止の停止条件
        private int firstStopPos;

        // コンストラクタ
        public ReelFirstData(StreamReader sReader)
        {
            string[] values = GetDataFromStream(sReader);

            int indexNum = 0;
            Debug.Log("Count:" + values.Length);

            foreach (string value in values)
            {
                Debug.Log(value);
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // TID読み込み
                else if (indexNum < FirstPushTIDPos)
                {
                    TID = Convert.ToByte(value);
                }

                // CID読み込み
                else if (indexNum < FirstPushCIDPos)
                {
                    CID = Convert.ToByte(value);
                }

                // 第一リール停止位置(末端まで読み込む)
                else if (indexNum < values.Length - 1)
                {
                    if(value != "ANY")
                    {
                        firstStopPos = ConvertToArrayBit(Convert.ToInt32(value));
                    }
                }

                // 最後の部分は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum += 1;
            }
            Debug.Log("Load Done");
        }

        // 条件チェック
        public bool CheckFirstReelCondition(int flagID, int bet, int bonus, int random, int firstPushPos)
        {
            // メイン条件チェック
            if(CheckMainCondition(flagID, bet, bonus, random))
            {
                // 第一停止の条件が一致するかチェック。0はANY
                // 第一停止の数値をビット演算で比較できるようにする
                int checkValue = 1 << firstPushPos + 1;
                // いずれかの条件が
                if (firstStopPos == 0 || (checkValue & firstStopPos) != 0)
                {
                    // 条件一致
                    return true;
                }
            }
            return false;
        }
    }

    // 基本データ
    [Serializable]
    public class ReelBaseData
    {
        // const
        // 条件を読み込むバイト数
        public const int ConditionMaxRead = 4;
        // 条件を読み込む際にずらすビット数
        public const int ConditionBitOffset = 4;

        // いずれかのボーナスが入っている条件を示す数字
        public const int BonusAnyValueID = 3;

        // 条件のシリアライズ
        public enum ConditionID { Flag, Bet, Bonus, Random }

        // フラグID, ボーナス, ベット枚数, ランダム制御の順で読み込む
        [SerializeField] private int mainConditon;
        // 使用するTID(テーブルID)
        [SerializeField] private byte tid;
        // 使用するCID(組み合わせID)
        [SerializeField] private byte cid;

        public int MainConditions { get { return mainConditon; } protected set { mainConditon = value; } }
        public byte TID { get { return tid; } protected set { tid = value; } }
        public byte CID { get { return cid; } protected set { cid = value; } }

        // 各条件の数値を返す
        public static int GetConditionData(int condition, int conditionID) => ((condition >> ConditionBitOffset * conditionID) & 0xF);

        // メイン条件があっているかチェック
        public bool CheckMainCondition(int flagID, int bet, int bonus, int random)
        {
            // データを条件にする
            int[] conditions = new int[]
            {
                flagID,
                bet,
                bonus,
                random,
            };

            // メイン条件チェック
            for (int i = 0; i < ConditionMaxRead; i++)
            {
                //Debug.Log("Condition1:" + GetConditionData(condition, i));
                //Debug.Log("Condition2:" + GetConditionData(data.MainConditions, i));
                // フラグID以外の条件で0があった場合はパスする
                if (i != (int)ConditionID.Flag && conditions[i] == 0)
                {
                    continue;
                }
                // ボーナス条件は3ならいずれかのボーナスが成立していればパス
                else if (i == (int)ConditionID.Bonus && conditions[i] == BonusAnyValueID &&
                    bonus != (int)BonusTypeID.BonusNone)
                {
                    //Debug.Log(bonus + "ANY BONUS");
                    continue;
                }

                // それ以外は受け取ったものと条件が合うか確認する
                else if (conditions[i] != GetConditionData(MainConditions, i))
                {
                    return false;
                }
            }

            return true;
        }

        // データをビットにする
        public int ConvertToArrayBit(int data)
        {
            // 0の時は変換しない
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }

        // データ読み込み
        public string[] GetDataFromStream(StreamReader sReader)
        {
            // カンマ入りのデータがあるため、独自にパーサーを作成
            string dataText = sReader.ReadLine();
            Debug.Log(dataText);
            string dataBuffer = "";

            // ダブルクオーテーションを発見したか
            bool findDoubleQuartation = false;

            int index = 0;
            foreach(char c in dataText)
            {
                // 空白以外読み込む
                if(c != ' ')
                {
                    dataBuffer += c;
                }

                index += 1;
            }

            Debug.Log("FinalData:" + dataBuffer);
            return dataBuffer.Split(",");
        }
    }
}
