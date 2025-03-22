using TMPro;
using UnityEngine;

public class ReelTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    ReelManager reel;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Reels-" + "\n";

        // Œ»Ý‚ÌƒŠ[ƒ‹ˆÊ’u
        buffer += "LeftPos:" + reel.GetCurrentReelPos((int)ReelManager.ReelID.ReelLeft);
        buffer += "MiddlePos:" + reel.GetCurrentReelPos((int)ReelManager.ReelID.ReelMiddle);
        buffer += "RightPos:" + reel.GetCurrentReelPos((int)ReelManager.ReelID.ReelRight) + "\n";

        

        text.text = buffer;
    }

    public void SetReelManager(ReelManager reelManager) => this.reel = reelManager;
}
