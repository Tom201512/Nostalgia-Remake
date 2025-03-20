using UnityEngine;
using System.Collections;

namespace ReelSpinGame_Main.File
{
    public class FileManager
    {
        // ファイルの管理

        // const
        // ファイルのアドレス

        // フラグ確率テーブル
        public const string FlagTableAPath = "Assets/DataFile/LotsTable/FlagTableA.csv";
        public const string FlagTableBPath = "Assets/DataFile/LotsTable/FlagTableB.csv";
        public const string FlagTableBIGPath = "Assets/DataFile/LotsTable/FlagTableBIG.csv";

        // リール配列
        public const string ReelArrayPath = "Assets/DataFile/ArrayData.csv";

        // 停止条件テーブル
        public const string ReelLeftCondtiion = "Assets/DataFile/Conditions/ReelLConditions.csv";
        public const string ReelMiddleCondtiion = "Assets/DataFile/Conditions/ReelMConditions.csv";
        public const string ReelRightCondtiion = "Assets/DataFile/Conditions/ReelRConditions.csv";

        // スベリコマテーブル
        public const string ReelLeftTable = "Assets/DataFile/ReelTables/ReelLTable.csv";
        public const string ReelMiddleTable = "Assets/DataFile/ReelTables/ReelMTable.csv";
        public const string ReelRightTable = "Assets/DataFile/ReelTables/ReelRTable.csv";

        // 払い出し表と成立ラインデータ
        public const string NormalPayoutPath = "Assets/DataFile/Payouts/NormalPayoutData.csv";
        public const string BigPayoutPath = "Assets/DataFile/Payouts/BigPayoutData.csv";
        public const string JacPayoutPath = "Assets/DataFile/Payouts/JACPayout.csv";
        public const string PayoutLinePath = "Assets/DataFile/Payouts/PayoutLine.csv";
    }
}