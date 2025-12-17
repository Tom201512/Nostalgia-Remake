namespace ReelSpinGame_Effect.Data
{
    // 演出のデータ
    public interface IDoesEffect<T>
    {
        // var
        public bool HasEffect { get; set; }         // 演出処理中か

        // func
        public void DoEffect(T effectCondition);    // エフェクト処理をする
    }
}

