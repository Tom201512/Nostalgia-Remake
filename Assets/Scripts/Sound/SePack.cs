using System;
using UnityEngine;

namespace ReelSpinGame_Sound.SE
{
    // 効果音のデータセット
    [Serializable]
	public class SePack : ScriptableObject
	{
        // const
		// var
        [SerializeField] private SeFile bet;
		[SerializeField] private SeFile wait;
		[SerializeField] private SeFile start;
		[SerializeField] private SeFile spStart;
		[SerializeField] private SeFile stop;
		[SerializeField] private SeFile redRiichiSound;
		[SerializeField] private SeFile blueRiichiSound;
		[SerializeField] private SeFile bb7RiichiSound;
		[SerializeField] private SeFile replay;
		[SerializeField] private SeFile normalPayout;
		[SerializeField] private SeFile maxPayout;
		[SerializeField] private SeFile jacPayout;
        [SerializeField] private SeFile redStart;
        [SerializeField] private SeFile redEnd;
        [SerializeField] private SeFile blueStart;
        [SerializeField] private SeFile blueEnd;
        [SerializeField] private SeFile blackStart;
        [SerializeField] private SeFile blackEnd;
        [SerializeField] private SeFile regStart;

        public SeFile Bet
        {
            get { return bet; }
#if UNITY_EDITOR
            set { bet = value; }
#endif
        }

        public SeFile Wait
        {
            get { return wait; }
#if UNITY_EDITOR
            set { wait = value; }
#endif
        }

        public SeFile Start
        {
            get { return start; }
#if UNITY_EDITOR
            set { start = value; }
#endif
        }

        public SeFile SpStart
        {
            get { return spStart; }
#if UNITY_EDITOR
            set { spStart = value; }
#endif
        }

        public SeFile Stop
        {
            get { return stop; }
#if UNITY_EDITOR
            set { stop = value; }
#endif
        }

        public SeFile RedRiichiSound
        {
            get { return redRiichiSound; }
#if UNITY_EDITOR
            set { redRiichiSound = value; }
#endif
        }

        public SeFile BlueRiichiSound
        {
            get { return blueRiichiSound; }
#if UNITY_EDITOR
            set { blueRiichiSound = value; }
#endif
        }

        public SeFile BB7RiichiSound
        {
            get { return bb7RiichiSound; }
#if UNITY_EDITOR
            set { bb7RiichiSound = value; }
#endif
        }

        public SeFile Replay
        {
            get { return replay; }
#if UNITY_EDITOR
            set { replay = value; }
#endif
        }

        public SeFile NormalPayout
        {
            get { return normalPayout; }
#if UNITY_EDITOR
            set { normalPayout = value; }
#endif
        }

        public SeFile MaxPayout
        {
            get { return maxPayout; }
#if UNITY_EDITOR
            set { maxPayout = value; }
#endif
        }

        public SeFile JacPayout
        {
            get { return jacPayout; }
#if UNITY_EDITOR
            set { jacPayout = value; }
#endif
        }

        public SeFile RedStart
        {
            get { return redStart; }
#if UNITY_EDITOR
            set { redStart = value; }
#endif
        }

        public SeFile RedEnd
        {
            get { return redEnd; }
#if UNITY_EDITOR
            set { redEnd = value; }
#endif
        }

        public SeFile BlueStart
        {
            get { return blueStart; }
#if UNITY_EDITOR
            set { blueStart = value; }
#endif
        }

        public SeFile BlueEnd
        {
            get { return blueEnd; }
#if UNITY_EDITOR
            set { blueEnd = value; }
#endif
        }

        public SeFile BlackStart
        {
            get { return blackStart; }
#if UNITY_EDITOR
            set { blackStart = value; }
#endif
        }

        public SeFile BlackEnd
        {
            get { return blackEnd; }
#if UNITY_EDITOR
            set { blackEnd = value; }
#endif
        }

        public SeFile RegStart
        {
            get { return regStart; }
#if UNITY_EDITOR
            set { regStart = value; }
#endif
        }

        // Nullチェック
        public bool NullCheck()
        {
            SeFile[] soundLists = new SeFile[]
            {
                bet,
                wait,
                start,
                spStart,
                stop,
                redRiichiSound,
                blueRiichiSound,
                bb7RiichiSound,
                replay,
                normalPayout,
                maxPayout,
                jacPayout,
                redStart,
                redEnd,
                blueStart,
                blueEnd,
                blackStart,
                blackEnd,
                regStart
            };

            foreach (SeFile se in soundLists)
            {
                if (se == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}