using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class ReelTest : MonoBehaviour
{
    // リール処理のテスト用
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    [SerializeField] private ReelManager manager;

    // 入力があったか
    bool hasInput;

    // Use this for initialization
    void Awake()
    {
        hasInput = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 何も入力が入っていなければ実行

        if(!hasInput)
        {
            // リール回転
            if (OriginalInput.CheckOneKeyInput(KeyCode.UpArrow))
            {
                manager.StartReels();
            }

            // 左停止
            if (OriginalInput.CheckOneKeyInput(keyToStopLeft))
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
            // 入力がないかチェック
            if (Input.anyKey)
            {
                hasInput = true;
            }
        }

        else if (hasInput)
        {
            if(!Input.anyKey)
            {
                //Debug.Log("Input end");
                hasInput = false;

                if(manager.IsFinished && !manager.HasFinishedCheck)
                {
                    manager.StartCheckPayout(3);
                }
            }
            else
            {
                //Debug.Log("You still have inputs");
            }

        }
    }
} 