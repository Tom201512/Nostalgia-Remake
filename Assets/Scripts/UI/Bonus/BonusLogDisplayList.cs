using ReelSpinGame_UI.Bonus;
using System.Collections.Generic;
using UnityEngine;

using static ReelSpinGame_UI.Bonus.BonusLogDisplay;

public class BonusLogDisplayList : MonoBehaviour
{
    [SerializeField] private BonusLogDisplay bonusDataPrefab; // ボーナスデータのプレハブ

    // ボーナス履歴のリスト
    List<BonusLogDisplay> bonusLogDisplays;

    // 履歴が選択された時のイベント
    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexNum">配列番号</param>
    public delegate void OnBonusLogSelected(int indexNum);
    public event OnBonusLogSelected OnBonusLogSelectedEvent;

    void Awake()
    {
        bonusLogDisplays = new List<BonusLogDisplay>();
    }

    // 初期化
    public void InitializeData()
    {
        foreach (BonusLogDisplay bonus in bonusLogDisplays)
        {
            bonus.OnBonusLogPressedEvent -= OnBonusLogPressed;
            Destroy(bonus.gameObject);
        }
        bonusLogDisplays.Clear();
    }

    // データ追加
    public void AddBonusData(BonusDisplayData bonusDisplayData)
    {
        BonusLogDisplay bonusData = Instantiate(bonusDataPrefab);
        bonusData.SetData(bonusDisplayData);
        bonusData.OnBonusLogPressedEvent += OnBonusLogPressed;
        bonusData.transform.SetParent(transform, false);
        bonusLogDisplays.Add(bonusData);
    }

    // 選択リストの全解除
    void ResetSelection()
    {
        foreach (BonusLogDisplay bonus in bonusLogDisplays)
        {
            bonus.ToggleSelection(false);
        }
    }

    // ボーナス履歴を押したときのイベント
    void OnBonusLogPressed(int indexNum)
    {
        ResetSelection();
        bonusLogDisplays[indexNum].ToggleSelection(true);
        OnBonusLogSelectedEvent?.Invoke(indexNum);
    }
}
