namespace ReelSpinGame_Effect.Data
{
    // 演出データインターフェース
    public interface IDoesEffect<T>
    {
        public bool HasEffect { get; set; }         // 演出処理中か

        public void DoEffect(T effectCondition);    // エフェクト処理をする
    }
}

