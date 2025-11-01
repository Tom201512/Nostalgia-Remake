using System;
using UnityEngine;

namespace ReelSpinGame_Reels.Array
{
    public class ReelArrayModel
    {
        // ���[���z��̃��f��

        // const
        // ���[���z��
        public const int MaxReelArray = 21;
        // �}��
        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay }
        // ���[���ʒu���ʗp
        public enum ReelPosID { Lower2nd = -1, Lower, Center, Upper, Upper2nd }

        // var
        // ���[���z��
        public byte[] ReelArray { get; set; }
    }
}