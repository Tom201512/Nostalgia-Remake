using UnityEngine;
using System.Collections;
using System;

namespace ReelSpinGame_Sound
{
	// 効果音のデータセット
	[Serializable]
	public class SoundEffectPack
	{
        // const
		// var
        [SerializeField] private AudioClip bet;
		[SerializeField] private AudioClip wait;
		[SerializeField] private AudioClip start;
		[SerializeField] private AudioClip stop;
		[SerializeField] private AudioClip redRiichiSound;
		[SerializeField] private AudioClip blueRiichiSound;
		[SerializeField] private AudioClip bb7RiichiSound;
		[SerializeField] private AudioClip replay;
		[SerializeField] private AudioClip normalPayout;
		[SerializeField] private AudioClip maxpayout;
		[SerializeField] private AudioClip jacPayout;

		public AudioClip Bet 
        { 
            get { return bet; }
#if UNITY_EDITOR
            set { bet = value; }
#endif
        }

        public AudioClip Wait
        {
            get { return wait; }
#if UNITY_EDITOR
            set { wait = value; }
#endif
        }

        public AudioClip Start
        {
            get { return start; }
#if UNITY_EDITOR
            set { start = value; }
#endif
        }

        public AudioClip Stop
        {
            get { return stop; }
#if UNITY_EDITOR
            set { stop = value; }
#endif
        }

        public AudioClip RedRiichiSound
        {
            get { return redRiichiSound; }
#if UNITY_EDITOR
            set { redRiichiSound = value; }
#endif
        }

        public AudioClip BlueRiichiSound
        {
            get { return blueRiichiSound; }
#if UNITY_EDITOR
            set { blueRiichiSound = value; }
#endif
        }

        public AudioClip BB7RiichiSound
        {
            get { return bb7RiichiSound; }
#if UNITY_EDITOR
            set { bb7RiichiSound = value; }
#endif
        }

        public AudioClip Replay
        {
            get { return replay; }
#if UNITY_EDITOR
            set { replay = value; }
#endif
        }

        public AudioClip NormalPayout
        {
            get { return normalPayout; }
#if UNITY_EDITOR
            set { normalPayout = value; }
#endif
        }

        public AudioClip MaxPayout
        {
            get { return maxpayout; }
#if UNITY_EDITOR
            set { maxpayout = value; }
#endif
        }

        public AudioClip JacPayout
        {
            get { return jacPayout; }
#if UNITY_EDITOR
            set { jacPayout = value; }
#endif
        }
	}

    // BGMのデータセット
    [Serializable]
    public class MusicPack
    {
        // var

		// 赤7
        [SerializeField] private AudioClip redStart;
        [SerializeField] private AudioClip redBGM;
        [SerializeField] private AudioClip redJAC;
        [SerializeField] private AudioClip redEnd;

        public AudioClip RedStart 
        { 
            get { return redStart; }
#if UNITY_EDITOR
            set { redStart = value; }
#endif
        }

        public AudioClip RedBGM
        {
            get { return redBGM; }
#if UNITY_EDITOR
            set { redBGM = value; }
#endif
        }

        public AudioClip RedJAC
        {
            get { return redJAC; }
#if UNITY_EDITOR
            set { redJAC = value; }
#endif
        }

        public AudioClip RedEnd
        {
            get { return redEnd; }
#if UNITY_EDITOR
            set { redEnd = value; }
#endif
        }

        // 青7
        [SerializeField] private AudioClip blueStart;
        [SerializeField] private AudioClip blueBGM;
        [SerializeField] private AudioClip blueJAC;
        [SerializeField] private AudioClip blueEnd;

        public AudioClip BlueStart
        {
            get { return blueStart; }
#if UNITY_EDITOR
            set { blueStart = value; }
#endif
        }

        public AudioClip BlueBGM
        {
            get { return blueBGM; }
#if UNITY_EDITOR
            set { blueBGM = value; }
#endif
        }

        public AudioClip BlueJAC
        {
            get { return blueJAC; }
#if UNITY_EDITOR
            set { blueJAC = value; }
#endif
        }

        public AudioClip BlueEnd
        {
            get { return blueEnd; }
#if UNITY_EDITOR
            set { blueEnd = value; }
#endif
        }

        // BB7
        [SerializeField] private AudioClip blackStart;
        [SerializeField] private AudioClip blackBGM;
        [SerializeField] private AudioClip blackJAC;
        [SerializeField] private AudioClip blackEnd;

        public AudioClip BlackStart
        {
            get { return blackStart; }
#if UNITY_EDITOR
            set { blackStart = value; }
#endif
        }

        public AudioClip BlackBGM
        {
            get { return blackBGM; }
#if UNITY_EDITOR
            set { blackBGM = value; }
#endif
        }

        public AudioClip BlackJAC
        {
            get { return blackJAC; }
#if UNITY_EDITOR
            set { blackJAC = value; }
#endif
        }

        public AudioClip BlackEnd
        {
            get { return blackEnd; }
#if UNITY_EDITOR
            set { blackEnd = value; }
#endif
        }

        // REG

        [SerializeField] private AudioClip regStart;
        [SerializeField] private AudioClip regJac;

        public AudioClip RegStart
        {
            get { return regStart; }
#if UNITY_EDITOR
            set { regStart = value; }
#endif
        }

        public AudioClip RegJAC
        {
            get { return regJac; }
#if UNITY_EDITOR
            set { regJac = value; }
#endif
        }
    }
}