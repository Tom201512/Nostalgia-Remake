using ReelSpinGame_UI.Bonus;
using System.Collections.Generic;
using UnityEngine;

using static ReelSpinGame_UI.Bonus.BonusLogDisplay;

public class BonusLogDisplayList : MonoBehaviour
{
    // const

    // var
    [SerializeField] private BonusLogDisplay bonusDataPrefab; // ボーナスデータのプレハブ

    void Awake()
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnDisable()
    {
        // 無効になったらオブジェクトリストを削除
        foreach (BonusLogDisplay bonus in GetComponentsInChildren<BonusLogDisplay>())
        {
            Destroy(bonus);
        }
    }

    // func(public)
    // データ追加
    public void AddBonusData(BonusDisplayData bonusDisplayData)
    {
        BonusLogDisplay bonusData = Instantiate(bonusDataPrefab);
        bonusData.SetData(bonusDisplayData);
        // 追加する
        bonusData.transform.SetParent(transform);
    }

    // func(private)
}
