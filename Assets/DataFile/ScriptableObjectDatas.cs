using ReelSpinGame_Reels;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    [Serializable]
    public class ReelConditionsData
    {
        // const
        // 条件を読み込むバイト数
        public const int ConditionMaxRead = 5;
        // 第一停止リール停止位置を読み込むバイト数
        public const int FirstReelPosMaxRead = ConditionMaxRead + 10;
        // テーブルIDの読み込むバイト数
        public const int ReelTableIDMaxRead = FirstReelPosMaxRead + 1;
        // 条件を読み込む際にずらすビット数
        public const int ConditionBitOffset = 4;

        // enum
        // 条件のシリアライズ
        public enum ConditionID { Flag, FirstPush, Bonus, Bet, Random }

        // var
        // テーブル使用条件はint型データで格納し、1バイト(8bit)ごとにデータを分ける
        // フラグID, 第一停止, ボーナス, ベット枚数, ランダム制御の順で読み込む
        [SerializeField] int mainConditions;
        [SerializeField] int firstReelPosition;
        [SerializeField] byte reelTableNumber;

        public int MainConditions { get; private set; }

        // 第一停止したリールの位置(これをもとにテーブルの変更をする)
        public int FirstReelPosition { get; private set; }

        // 使用するテーブル番号
        public byte ReelTableNumber { get; private set; }

        // コンストラクタ
        public ReelConditionsData(StringReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // 読み込み開始(要素番号をもとにデータを読み込む
            foreach (string value in values)
            {
                // メイン条件(16進数で読み込みint型で圧縮)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // 第一リール停止
                else if (indexNum < FirstReelPosMaxRead)
                {
                    FirstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
                }

                // テーブルID読み込み
                else if (indexNum < ReelTableIDMaxRead)
                {
                    ReelTableNumber = Convert.ToByte(value);
                }

                // 最後の一行は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum++;
            }

            // デバッグ用
            string ConditionDebug = "";

            for (int i = 0; i < 5; i++)
            {
                ConditionDebug += GetConditionData(i).ToString() + ",";
            }
            Debug.Log("Condition:" + MainConditions + "Details:" + ConditionDebug + "FirstReel:" + FirstReelPosition + "ReelTableNum" + ReelTableNumber);
        }

        // func
        // 各条件をintにする
        public static int ConvertConditionData(int flagID, int firstPush, int bonus, int bet, int random)
        {
            // 16進数のデータへ変更
            int conditions = 0;
            // 配列にする
            int[] conditionArray = { flagID, firstPush, bonus, bet, random };

            for (int i = 0; i < (int)ConditionID.Random; i++)
            {
                conditions |= conditionArray[i] << ConditionBitOffset * i;
            }
            return conditions;
        }

        // 各条件の数値を返す
        public int GetConditionData(int conditionID) => ((MainConditions >> ConditionBitOffset * conditionID) & 0xF);

        // データをビットにする
        private int ConvertToArrayBit(int data)
        {
            //Do not convert if data is 0
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }
    }

    // リール制御テーブル
    [Serializable]
    public class ReelTableData
    {
        // リール制御テーブル(ディレイを格納する)
        public byte[] TableData { get; private set; } = new byte[ReelData.MaxReelArray];

        public ReelTableData(StringReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // デバッグ用
            string debugBuffer = "";

            // 読み込み開始
            foreach (string value in values)
            {
                // リールデータを読み込む
                if (indexNum < ReelData.MaxReelArray)
                {
                    TableData[indexNum] = Convert.ToByte(value);
                    debugBuffer += TableData[indexNum];
                }

                // 最後の一行は読まない(テーブル名)
                else
                {
                    break;
                }
                indexNum++;
            }

            Debug.Log("Array:" + debugBuffer);
        }
    }

    public class ScriptableGenerator : EditorWindow
    {
        // const

        // var

        // ファイル指定
        private TextAsset arrayData;
        private TextAsset conditionsData;
        private TextAsset delayData;

        // func
        // リール配列の作成
        [MenuItem("ScriptableGen/ReelData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open ReelData Generator");
            ScriptableGenerator window = GetWindow<ScriptableGenerator>();
            window.titleContent = new GUIContent("ReelData Generator");
        }

        private void Awake()
        {
            arrayData = null;
            conditionsData = null;
            delayData = null;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("リール配列ファイル、条件テーブル、ディレイテーブル");
            arrayData = (TextAsset)EditorGUILayout.ObjectField("Array:", arrayData, typeof(TextAsset), true);
            conditionsData = (TextAsset)EditorGUILayout.ObjectField("Conditions:", conditionsData, typeof(TextAsset), true);
            delayData = (TextAsset)EditorGUILayout.ObjectField("Tables:", delayData, typeof(TextAsset), true);

            if (GUILayout.Button("リールデータ作成"))
            {
                Debug.Log("Pressed");
                MakeReelData(arrayData);
            }
        }

        private void MakeReelData(TextAsset arrayFile)
        {
            // ディレクトリの作成
            string path = "Assets/ReelDatas";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();

            // 配列作成
            reelDatabase.Array = MakeReelArray(reelDatabase, arrayFile);

            var fileName = arrayFile.name + ".asset";

            foreach (byte value in reelDatabase.Array)
            {
                Debug.Log(value + "Symbol:" + ReelData.ReturnSymbol(value));
            }

            for (int i = 0; i < ReelData.MaxReelArray; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReelData.ReturnSymbol(reelDatabase.Array[i]));
            }

            // 保存処理
            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }

        private byte[] MakeReelArray(ReelDatabase reelDatabase, TextAsset arrayFile)
        {
            StringReader buffer = new StringReader(arrayFile.text);
            // 配列読み込み
            string[] values = buffer.ReadLine().Split(',');
            // 配列に変換
            return Array.ConvertAll(values, byte.Parse);
        }
    }
}

