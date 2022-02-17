using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    // [RequireComponent(typeof(AudioSource))]
    public class SoundData : MonoBehaviour
    {
        //音频源控件
        // public new AudioSource audio;

        public AudioClip audioClip;

        /// <summary>
        /// 是否强制重新播放
        /// </summary>
        //[HideInInspector]
        public bool isForceReplay = false;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        //[HideInInspector]
        public bool isLoop = false;

        /// <summary>
        /// 音量
        /// </summary>
        //[HideInInspector]
        public float volume = 1;

        /// <summary>
        /// 延迟
        /// </summary>
        //[HideInInspector]
        public ulong delay = 0;

        // public AudioSource GetAudio()
        // {
        //     return audio;
        // }

        public bool IsPlaying
        {
            get;
            set;
        }
        public bool IsPause
        {
            get;
            set;
        }
        public void Dispose()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 音效类型
        /// </summary>
        public SoundType Sound { get; set; }

        public bool Mute
        {
            get;
            set;
        }

        public float Volume
        {
            get { return this.volume; }
            set { this.volume = value; }
        }
    }

    /// <summary>
    /// 音效类型
    /// </summary>
    public enum SoundType
    {
        Music,//长音乐
        Sound,//短音乐
    }
}