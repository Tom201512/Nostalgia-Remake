using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_UI.Bonus;
using System.Collections.Generic;
using UnityEngine;

using static ReelSpinGame_UI.Bonus.BonusLogDisplay;

public class BonusLogDisplayList : MonoBehaviour
{
    // const

    // var
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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // func(public)
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
        // ボタンを押したときの挙動を登録
        bonusData.OnBonusLogPressedEvent += OnBonusLogPressed;
        // 追加する
        bonusData.transform.SetParent(transform);
        bonusLogDisplays.Add(bonusData);
    }

    // func(private)
    // 選択リストの全解除
    void ResetSelection()
    {
        foreach (BonusLogDisplay bonus in bonusLogDisplays)
        {
            bonus.ToggleSelection(false);
            Debug.Log("Deselected:" + bonus.BonusIndexNumber);
        }
    }
    
    // ボーナス履歴を押したときのイベント
    void OnBonusLogPressed(int indexNum)
    {
        ResetSelection();
        Debug.Log("Index selected:" + indexNum);
        bonusLogDisplays[indexNum].ToggleSelection(true);
        OnBonusLogSelectedEvent?.Invoke(indexNum);
    }
}
