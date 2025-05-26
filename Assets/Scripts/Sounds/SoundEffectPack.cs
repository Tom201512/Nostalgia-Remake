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
		[SerializeField] private AudioClip riichiSound;
		[SerializeField] private AudioClip replay;
		[SerializeField] private AudioClip normalPayout;
		[SerializeField] private AudioClip maxpayout;
		[SerializeField] private AudioClip jacPayout;

		public AudioClip Bet { get { return bet; } }
		public AudioClip Wait { get { return wait; } }
		public AudioClip Start { get { return start; } }
		public AudioClip Stop { get { return stop; } }
		public AudioClip RiichiSound { get { return riichiSound; } }
		public AudioClip Replay { get { return replay; } }
		public AudioClip NormalPayout { get { return normalPayout; } }
		public AudioClip MaxPayout { get { return maxpayout; } }
		public AudioClip JacPayout { get { return jacPayout; } }
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

        public AudioClip RedStart { get { return redStart; } }
        public AudioClip RedBGM { get { return redBGM; } }
        public AudioClip RedJAC { get { return redJAC; } }
        public AudioClip RedEnd { get { return redEnd; } }

        // 青7
        [SerializeField] private AudioClip blueStart;
        [SerializeField] private AudioClip blueBGM;
        [SerializeField] private AudioClip blueJAC;
        [SerializeField] private AudioClip blueEnd;

        public AudioClip BlueStart { get { return blueStart; } }
        public AudioClip BlueBGM { get { return blueBGM; } }
        public AudioClip BlueJAC { get { return blueJAC; } }
        public AudioClip BlueEnd { get { return blueEnd; } }

        // BB7
        [SerializeField] private AudioClip blackStart;
        [SerializeField] private AudioClip blackBGM;
        [SerializeField] private AudioClip blackJAC;
        [SerializeField] private AudioClip blackEnd;

        public AudioClip BlackStart { get { return blackStart; } }
        public AudioClip BlackBGM { get { return blackBGM; } }
        public AudioClip BlackJAC { get { return blackJAC; } }
        public AudioClip BlackEnd { get { return blackEnd; } }

        // REG

        [SerializeField] private AudioClip regStart;
        [SerializeField] private AudioClip regJac;

        public AudioClip RegStart { get { return regStart; } }
        public AudioClip RegJAC { get { return regJac; } }
    }
}