using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Datas.Reels
{
    [Serializable]
    public class ReelFirstConditions : ReelBaseData
    {
        // const
        // 第一停止の停止条件読み込み位置
        private const int FirstPushReadPos = ConditionMaxRead + 1;
        // 第一停止のTID読み込み位置
        private const int FirstPushTIDPos = FirstPushReadPos + 1;
        // 第一停止のCID読み込み位置
        private const int FirstPushCIDPos = FirstPushTIDPos + 1;

        // var
        // 第一停止の停止条件
        [SerializeField] private int firstStopPos;

        // コンストラクタ
        public ReelFirstConditions(StreamReader sReader)
        {
            string[] values = GetDataFromStream(sReader);
            int indexNum = 0;

            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // 第一リール停止位置(末端まで読み込む)
                else if (indexNum < FirstPushReadPos)
                {
                    if (value != "ANY")
                    {
                        string[] stopPosData = value.Trim('"').Split(",");
                        foreach (string stop in stopPosData)
                        {
                            firstStopPos += ConvertToArrayBit(Convert.ToInt32(stop));
                        }
                    }
                    else
                    {
                        firstStopPos = 0;
                    }
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

                // 最後の部分は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum += 1;
            }
            //Debug.Log("MainCondition:" + MainConditions);
            //Debug.Log("FirstStopPos:" + firstStopPos);
            //Debug.Log("TID:" + TID);
            //Debug.Log("CID:" + CID);

            //Debug.Log("First Push Load Done");
        }

        // 条件チェック
        public bool CheckFirstReelCondition(int flagID, int bonus, int bet, int random, int pushedPos)
        {
            // メイン条件チェック
            if(CheckMainCondition(flagID, bonus, bet, random))
            {
                // 第一停止の条件が一致するかチェック。0はANY
                // 第一停止の数値をビット演算で比較できるようにする
                //Debug.Log("Pushed:" + pushedPos);
                int checkValue = 1 << pushedPos + 1;
                //Debug.Log("check:" + checkValue);
                //Debug.Log("FirstPos:" + firstStopPos);

                // 停止条件を確認
                if (firstStopPos == 0 || ((checkValue & firstStopPos) != 0))
                {
                    //Debug.Log("Stop Pos has match with condition");
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
        public enum ConditionID { Flag, Bonus, Bet, Random }

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
        protected int GetConditionData(int condition, int conditionID) => ((condition >> ConditionBitOffset * conditionID) & 0xF);

        // メイン条件があっているかチェック
        protected bool CheckMainCondition(int flagID, int bonus, int bet, int random)
        {
            //Debug.Log("MainCondition:" + flagID + "," + bonus + "," + bet + "," + random);
            // データを条件にする
            int[] conditions = new int[]
            {
                flagID,
                bonus,
                bet,
                random,
            };

            //Debug.Log("ConditionData:" + MainConditions);

            // メイン条件チェック
            for (int i = 0; i < ConditionMaxRead; i++)
            {
                int checkData = GetConditionData(MainConditions, i);
                //Debug.Log("checkData:" + checkData);

                // フラグID以外の条件で0があった場合はパスする
                if (i != (int)ConditionID.Flag && checkData == 0)
                {
                    //Debug.Log("No condition");
                    continue;
                }
                // ボーナス条件は3ならいずれかのボーナスが成立していればパス
                else if (i == (int)ConditionID.Bonus && checkData == BonusAnyValueID &&
                    bonus != (int)BonusTypeID.BonusNone)
                {
                    //Debug.Log(bonus + "ANY BONUS");
                    continue;
                }
                // それ以外は受け取ったものと条件が合うか確認する
                else if (conditions[i] != checkData)
                {
                    //Debug.Log("Condition not match");
                    return false;
                }
            }
            //Debug.Log("Condition Passed");
            return true;
        }

        // データをビットにする
        protected int ConvertToArrayBit(int data)
        {
            // 0の時は変換しない
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }

        // データ読み込み
        protected string[] GetDataFromStream(StreamReader sReader)
        {
            // カンマ入りのデータがあるため、独自にパーサーを作成
            // 読み込んだデータのテキスト
            string loadedText = sReader.ReadLine();
            // データに入れるバッファ
            string bufferText = "";
            // ダブルクォーテーションでパースしたデータ
            string parseText = "";
            List<string> dataBuffer = new List<string>();

            // ダブルクオーテーションを発見したか
            bool findDoubleQuartation = false;

            foreach(char c in loadedText)
            {
                // 空白以外読み込む
                if(c != ' ')
                {
                    // ダブルクォーテーションなら別テキストに移す(カンマも取り入れる)
                    if(c == '"')
                    {
                        findDoubleQuartation = !findDoubleQuartation;
                        parseText += c;
                    }
                    else 
                    {
                        if(findDoubleQuartation)
                        {
                            parseText += c;
                        }
                        // ダブルクォーテーションを読み込んだ時はカンマも読まない
                        else
                        {
                            // カンマを読み込んだらバッファテキストを取り込む
                            if(c == ',')
                            {
                                // パースした文章がある場合はその文章をバッファに取り込む
                                if(parseText != "")
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

            string finalData = String.Join(",", dataBuffer);
            return dataBuffer.ToArray();
        }
    }
}
