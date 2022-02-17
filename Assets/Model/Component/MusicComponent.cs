using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class MusicComponentSystem : AwakeSystem<MusicComponent>
    {
        public override void Awake(MusicComponent self)
        {
            self.Awake();
        }
    }


    public class MusicComponent : Component
    {
        public static MusicComponent Instance { get; private set; }
        private AudioManager audoSystem;
        public void Awake()
        {
            Instance = this;
            audoSystem = new AudioManager();
            audoSystem.ILInit();
        }

        /// <summary>
        /// 播放音效 到时间后消失
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public void PlayAudioSource(string name, float time)
        {
            audoSystem.PlayEffect(name);
        }

        public async Task<float> PlayAudioSourceGetLength(string name, float time)
        {
            var result = await audoSystem.GetAudioClipLength(name);
            audoSystem.PlayEffect(name);
            return result;
        }

        /// <summary>
        /// 播放是否循环得音乐
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLoop"></param>
        public void PlayBGAudioSource(string name, bool isLoop)
        {
            audoSystem.PlayBGM(name);
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        public void GoOnBGAudioSource()
        {
            // AudioSource?.UnPause();
            audoSystem?.GetBgmAudioSource?.UnPause();
        }

        /// <summary>
        /// 音乐暂停
        /// </summary>
        public void StopBGAudioSource()
        {
            audoSystem?.GetBgmAudioSource?.Pause();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
            Instance = null;
            this.audoSystem.Dispose();
        }
    }
}
