namespace ReelSpinGame_Reel.Table
{
    // 条件検索で見つかったテーブルの情報
    public class FindTableData
    {
        public const int TableNotFoundValue = -1;       // エラー時の数値

        public int TableID { get; set; }        // テーブルID(TID)
        public int CombinationID { get; set; }  // 組み合わせID(CID)

        public FindTableData()
        {
            TableID = TableNotFoundValue;
            CombinationID = TableNotFoundValue;
        }

        // テーブルが見つかっているかを返す
        public bool IsTableFound() => TableID != TableNotFoundValue && CombinationID != TableNotFoundValue;
    }
}