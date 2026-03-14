using ReelSpinGame_Bonus;
using ReelSpinGame_Reel.Table;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Scriptable
{
    // 基本データ
    [Serializable]
    public class ReelBaseConditionData
    {
        public const int ConditionMaxRead = 4;          // 条件を読み込むバイト数
        public const int ConditionBitOffset = 4;        // 条件を読み込む際にずらすビット数
        public const int BonusAnyValueID = 3;           // いずれかのボーナスが入っている条件を示す数字
        public const int NoConditionValue = 0;          // 条件なしの数値

        // 条件のシリアライズ
        public enum ConditionID 
        { 
            Flag, 
            Bonus, 
            Bet, 
            Random, 
        }

        [SerializeField] private int mainCondition;      // メイン条件の数値ID
        [SerializeField] private int tableID;              // 使用するTID(テーブルID)
        [SerializeField] private int combinationID;              // 使用するCID(組み合わせID)

        // メイン条件
        public int MainConditions
        {
            get => mainCondition;
            protected set => mainCondition = value;
        }

        // テーブルiD
        public int TableID 
        {
            get => tableID;
            protected set => tableID = value;
        }

        // 組み合わせID
        public int CombinationID
        {
            get => combinationID;
            protected set => combinationID = value;
        }

        // 各条件の数値を返す
        protected int GetConditionData(int condition, int conditionID)
        {
            return condition >> ConditionBitOffset * conditionID & 0xF;
        }

        // メイン条件があっているかチェック
        protected bool CheckMainCondition(ReelMainCondition mainCondition)
        {
            // 条件を見る順番を決める
            int[] conditions = new int[]
            {
                (int)mainCondition.Flag,
                (int)mainCondition.Bonus,
                mainCondition.Bet,
                mainCondition.Random,
            };

            // メイン条件チェック
            for (int i = 0; i < ConditionMaxRead; i++)
            {
                int checkData = GetConditionData(MainConditions, i);

                // フラグID以外の条件で0があった場合はパスする
                if (i != (int)ConditionID.Flag && checkData == NoConditionValue)
                {
                    continue;
                }
                // ボーナス条件は3ならいずれかのボーナスが成立していればパス
                else if (i == (int)ConditionID.Bonus && checkData == BonusAnyValueID &&
                    mainCondition.Bonus != BonusModel.BonusTypeID.BonusNone)
                {
                    continue;
                }
                // それ以外は受け取ったものと条件が合うか確認する
                else if (conditions[i] != checkData)
                {
                    return false;
                }
            }

            return true;
        }

        // データをビットにする
        protected int ConvertToArrayBit(int data) => 1 << data;

        // データ読み込み
        protected string[] GetDataFromStream(StreamReader sReader)
        {
            // カンマ入りのデータがあるため、独自にパーサーを作成

            string loadedText = sReader.ReadLine();
            string bufferText = "";
            string parseText = "";
            List<string> dataBuffer = new List<string>();
            bool findDoubleQuotation = false;

            foreach (char c in loadedText)
            {
                // 空白以外読み込む
                if (c != ' ')
                {
                    // ダブルクォーテーションなら別テキストに移す(カンマも取り入れる)
                    if (c == '"')
                    {
                        findDoubleQuotation = !findDoubleQuotation;
                        parseText += c;
                    }
                    else
                    {
                        if (findDoubleQuotation)
                        {
                            parseText += c;
                        }
                        // ダブルクォーテーションを読み込んだ時はカンマも読まない
                        else
                        {
                            // カンマを読み込んだらバッファテキストを取り込む
                            if (c == ',')
                            {
                                // パースした文章がある場合はその文章をバッファに取り込む
                                if (parseText != "")
                                {
                                    dataBuffer.Add(parseText);
                                    parseText = "";
                                }
                                else
                                {
                                    dataBuffer.Add(bufferText);
                                    bufferText = "";
                                }
                            }
                            // カンマを読み込むまではバッファにテキストを詰める
                            else
                            {
                                bufferText += c;
                            }
                        }
                    }
                }
            }

            // 最後に読み込んだテキストを読み込む
            dataBuffer.Add(bufferText);
            return dataBuffer.ToArray();
        }
    }
}