using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class ReelTest : MonoBehaviour
{
    // リール処理のテスト用
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    [SerializeField] private ReelManager manager;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // リール回転
        if (OriginalInput.CheckOneKeyInput(KeyCode.UpArrow))
        {
            manager.StartReels();
        }

        // 左停止
        if(OriginalInput.CheckOneKeyInput(keyToStopLeft))
        {
            manager.StopSelectedReel(ReelManager.ReelID.ReelLeft);
        }

        // 中停止
        if (OriginalInput.CheckOneKeyInput(keyToStopMiddle))
        {
            manager.StopSelectedReel(ReelManager.ReelID.ReelMiddle);
        }

        // 右停止
        if (OriginalInput.CheckOneKeyInput(keyToStopRight))
        {
            manager.StopSelectedReel(ReelManager.ReelID.ReelRight);
        }
    }
} 