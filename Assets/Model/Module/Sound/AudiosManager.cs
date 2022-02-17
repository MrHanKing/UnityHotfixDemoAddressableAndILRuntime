using UnityEngine;
using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using Pools;
using System.Threading.Tasks;

namespace ETModel
{
    public class IAudioParams
    {
        /// <summary>
        /// 音量
        /// </summary>
        /// <value></value>
        public float volume { get; set; }
        /// <summary>
        /// 循环
        /// </summary>
        /// <value></value>
        public bool loop { get; set; }
    }
    public enum AudioPriority
    {
        BGM,
        Dialog,
        Effect,
    }

    public class UtilityFun
    {
        public static void PlayAudioByAudioSouce(AudioSource targetAudioSouce, AudioClip audio, IAudioParams audioParam = null)
        {
            targetAudioSouce.clip = audio;
            targetAudioSouce.clip.LoadAudioData();
            if (audioParam != null)
            {
                targetAudioSouce.loop = audioParam.loop;
                targetAudioSouce.volume = audioParam.volume;
            }
            targetAudioSouce.Play();
        }
    }


    public class AudioManager
    {
        private GameObject lifeObject;
        private AudioSource bgmAudioSource;
        public AudioSource GetBgmAudioSource { get => this.bgmAudioSource; }
        private AudioSource dialogAudioSource;
        public AudioSource GetDialogAudioSource { get => this.dialogAudioSource; }
        /// <summary>
        /// 声音通道对应的异步回调字典 被打断的会通知播放失败
        /// </summary>
        private Dictionary<AudioSource, TaskCompletionSource<bool>> runingTaskMap;

        #region effect 通道
        /// <summary>
        /// 效果音效的通道池子
        /// </summary>
        private CustomComponentPool<AudioSource> effectAudioSoucePool;
        /// 存放正在使用的effect音频组件
        /// </summary>
        private List<AudioSource> usedSoundAudioSourceList;
        #endregion

        public static readonly string lifeObjectName = "AudioManage";

        public void Init()
        {
            if (!this.lifeObject)
            {
                this.lifeObject = new GameObject(AudioManager.lifeObjectName);
                GameObject.DontDestroyOnLoad(this.lifeObject);
                bgmAudioSource = this.lifeObject.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;

                dialogAudioSource = this.lifeObject.AddComponent<AudioSource>();
                runingTaskMap = new Dictionary<AudioSource, TaskCompletionSource<bool>>();

                this.effectAudioSoucePool = new CustomComponentPool<AudioSource>(this.lifeObject, null, null);
            }

            if (usedSoundAudioSourceList == null)
            {
                usedSoundAudioSourceList = new List<AudioSource>();
            }
        }

        public Task<bool> PlayBGM(AudioClip clip, IAudioParams audioParam = null)
        {
            this.Interrupt(AudioPriority.BGM);
            return this.PlayAudio(AudioPriority.BGM, clip, audioParam);
        }

        public void StopBGM()
        {
            this.Interrupt(AudioPriority.BGM);
        }

        public Task<bool> PlayDialog(AudioClip clip, IAudioParams audioParam = null)
        {
            this.Interrupt(AudioPriority.Dialog);
            return this.PlayAudio(AudioPriority.Dialog, clip, audioParam);
        }

        public void StopDialog()
        {
            this.Interrupt(AudioPriority.Dialog);
        }

        public Task<bool> PlayEffect(AudioClip clip, IAudioParams audioParam = null)
        {
            return this.PlayAudio(AudioPriority.Effect, clip, audioParam);
        }

        /// <summary>
        /// 停止效果音频 若没有指定clip则会停止所有效果 制定clip则停止所有符合clip的音频
        /// </summary>
        /// <param name="clip"></param>
        public void StopEffect(AudioClip clip = null)
        {
            if (clip == null)
            {
                this.Interrupt(AudioPriority.Effect);
            }
            else
            {
                List<AudioSource> target = this.usedSoundAudioSourceList.FindAll(audioSource => audioSource.clip == clip);
                this.StopEffectAudioSouce(target);
            }
        }


        /// <summary>
        /// 播放3d音效 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position">世界坐标</param>
        public void Play3DSound(AudioClip audio, Vector3 position)
        {
            // 暂时没做管理
            AudioSource.PlayClipAtPoint(audio, position);
        }

        private bool GetAudioMuteConfig(AudioPriority level)
        {
            switch (level)
            {
                case AudioPriority.BGM:
                    return this._musicMute;
                case AudioPriority.Effect:
                    return this._soundMute;
                    // 旁白暂不会静音
            }

            return false;
        }

        private Task<bool> PlayAudio(AudioPriority level, AudioClip audio, IAudioParams audioParam = null)
        {
            AudioSource targetAudioSouce = this.GetAudioSource(level);

            if (targetAudioSouce)
            {
                UtilityFun.PlayAudioByAudioSouce(targetAudioSouce, audio, audioParam);
                var mute = this.GetAudioMuteConfig(level);
                targetAudioSouce.mute = mute;
            }

            TaskCompletionSource<bool> newTask = new TaskCompletionSource<bool>();

            this.AddTaskToMap(targetAudioSouce, newTask);

            Task.Delay((int)(audio.length * 1000)).ContinueWith((task) =>
            {
                // 正常播放完 放心TrySetResult 如果已经被打断 则newTask已经返回结果
                try
                {
                    newTask.TrySetResult(targetAudioSouce.clip == audio);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }

                this.TryReleaseAudioSouce(targetAudioSouce, level);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return newTask.Task;
        }

        /// <summary>
        /// 打断对应级别的音效
        /// </summary>
        /// <param name="level"></param>
        private void Interrupt(AudioPriority level)
        {
            switch (level)
            {
                case AudioPriority.BGM:
                    this.StopAudioSouce(this.bgmAudioSource);
                    break;
                case AudioPriority.Dialog:
                    this.StopAudioSouce(this.dialogAudioSource);
                    break;
                case AudioPriority.Effect:
                    this.StopEffectAudioSouce(this.usedSoundAudioSourceList);
                    break;
            }
        }

        /// <summary>
        /// 释放AudioSouce
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="level"></param>
        private void TryReleaseAudioSouce(AudioSource audioSource, AudioPriority level)
        {
            if (level == AudioPriority.Effect)
            {
                this.UsedToUnused(audioSource);
            }
        }

        /// <summary>
        /// 暂停AudioSouce
        /// </summary>
        /// <param name="targetAudioSouce"></param>
        private void StopAudioSouce(AudioSource targetAudioSouce)
        {
            this.RemoveOldTaskToMap(targetAudioSouce);
            targetAudioSouce.Stop();
        }

        /// <summary>
        /// 特殊 效果音频被池子管理
        /// </summary>
        /// <param name="targetAudioSouce"></param>
        private void StopEffectAudioSouce(List<AudioSource> targetAudioSouces)
        {
            if (targetAudioSouces.Count <= 0)
            {
                return;
            }

            for (int i = targetAudioSouces.Count - 1; i >= 0; i -= 1)
            {
                AudioSource target = this.usedSoundAudioSourceList[i];
                this.StopAudioSouce(target);
                this.UsedToUnused(target);
            }
        }

        /// <summary>
        /// 在map中储备通道异步任务
        /// </summary>
        /// <param name="targetAudioSouce"></param>
        /// <param name="newTask"></param>
        private void AddTaskToMap(AudioSource targetAudioSouce, TaskCompletionSource<bool> newTask)
        {
            this.RemoveOldTaskToMap(targetAudioSouce);
            this.runingTaskMap.Add(targetAudioSouce, newTask);
        }

        /// <summary>
        /// 结束可能在等待的异步
        /// </summary>
        /// <param name="targetAudioSouce"></param>
        private void RemoveOldTaskToMap(AudioSource targetAudioSouce)
        {
            if (this.runingTaskMap.TryGetValue(targetAudioSouce, out TaskCompletionSource<bool> oldTask))
            {
                this.runingTaskMap.Remove(targetAudioSouce);
                // 逻辑返回音频播放完成 结果失败的异步结果
                oldTask.TrySetResult(false);
            }
        }


        /// <summary>
        /// 获取音频播放器载体
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private AudioSource GetAudioSource(AudioPriority level)
        {
            switch (level)
            {
                case AudioPriority.BGM:
                    return this.bgmAudioSource;
                case AudioPriority.Dialog:
                    return this.dialogAudioSource;
                case AudioPriority.Effect:
                    return this.GetEffectAudioSouce();

            }
            return null;
        }

        /// <summary>
        /// 获得音效闲置池子中的AudioSouce
        /// </summary>
        private AudioSource GetEffectAudioSouce()
        {
            return UnusedToUsed();
        }

        /// <summary>
        /// 将未使用的音频组件移至已使用集合里
        /// </summary>
        /// <returns></returns>
        private AudioSource UnusedToUsed()
        {
            AudioSource audioSource = this.effectAudioSoucePool.Get();
            usedSoundAudioSourceList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 将使用完的音频组件移至未使用集合里
        /// </summary>
        /// <param name="audioSource"></param>
        private void UsedToUnused(AudioSource audioSource)
        {
            if (usedSoundAudioSourceList.Contains(audioSource))
            {
                usedSoundAudioSourceList.Remove(audioSource);
            }
            if (audioSource != null)
            {
                this.effectAudioSoucePool.Release(audioSource);
            }
        }

        public AudioSource BorrowEffectAudioSource()
        {
            return this.GetEffectAudioSouce();
        }

        public void RemandEffectAudioSouce(AudioSource borrowAudioSource)
        {
            this.TryReleaseAudioSouce(borrowAudioSource, AudioPriority.Effect);
        }

        public void Dispose()
        {
            this.effectAudioSoucePool?.Dispose();
            this.effectAudioSoucePool = null;
            this.usedSoundAudioSourceList.Clear();
        }

        #region ILRuntime适配
        public static AudioManager instance;
        /// <summary>
        /// 控制游戏全局音量
        /// </summary>
        public float SoundVolume
        {
            get
            {
                return _soundVolume;
            }
            set
            {
                _soundVolume = Mathf.Clamp(value, 0, 1);
                foreach (SoundData clip in abSounds.Values)
                {
                    clip.Volume = _soundVolume * clip.volume;
                }
            }
        }
        private float _soundVolume = 1f;
        /// <summary>
        /// 背景音乐静音
        /// </summary>
        public bool MusicMute
        {
            get { return _musicMute; }
            set
            {
                _musicMute = value;
                foreach (var soundData in abSounds.Values)
                {
                    soundData.Mute = _musicMute;
                }
                PlayerPrefs.SetInt("MusicMute", value ? 1 : 0);
                this.bgmAudioSource.mute = value;
            }
        }
        private bool _musicMute = false;

        /// <summary>
        /// 音效静音
        /// </summary>
        public bool SoundMute
        {
            get { return _soundMute; }
            set
            {
                _soundMute = value;
                foreach (var soundData in abSounds.Values)
                {
                    soundData.Mute = _soundMute;
                }
                PlayerPrefs.SetInt("SoundMute", value ? 1 : 0);

                foreach (var item in this.usedSoundAudioSourceList)
                {
                    item.mute = value;
                }
            }
        }
        private bool _soundMute = false;
        //catch ab资源
        private static Dictionary<string, SoundData> abSounds = new Dictionary<string, SoundData>();
        public void ILInit()
        {
            this.Init();
            _musicMute = PlayerPrefs.GetInt("MusicMute", 0) == 1;
            _soundMute = PlayerPrefs.GetInt("SoundMute", 0) == 1;

            // ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            // resourcesComponent.LoadBundle("sound.unity3d");
            instance = this;
        }

        //加载声音
        private async Task<SoundData> LoadSound(string soundName)
        {
            // ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            if (!abSounds.ContainsKey(soundName) || abSounds[soundName] == null)
            {
                // await resourcesComponent.LoadBundleAsync("sound.unity3d");
                var target = (await AddressableComponent.Instance.InstantiateSublevelAsync("Audios", soundName, this.lifeObject.transform)).GetComponent<SoundData>();
                abSounds.Add(soundName, target);
                // resourcesComponent.UnloadBundle("sound.unity3d");
                // target.transform.SetParent(this.lifeObject.transform);
            }
            return abSounds[soundName];
        }

        private async Task<SoundData> GetSoundData(string clipName)
        {
            var name = clipName.ToLower();
            SoundData sd = await LoadSound(name);
            if (sd == null)
            {
                Log.Error($"没有此音效 ={ clipName}");
            }
            return sd;
        }
        private IAudioParams GetResultAudioParams(SoundData soundData, IAudioParams audioParam = null)
        {
            var data = new IAudioParams();
            if (audioParam != null)
            {
                data.volume = audioParam.volume * SoundVolume;
                data.loop = audioParam.loop;
            }
            else
            {
                data.volume = soundData.volume * SoundVolume;
                data.loop = soundData.isLoop;
            }
            return data;
        }

        public async Task<float> GetAudioClipLength(string clipName)
        {
            var sound = await this.GetSoundData(clipName);
            return sound.audioClip.length;
        }

        public async Task<bool> PlayBGM(string clipName, IAudioParams audioParam = null)
        {
            var sound = await this.GetSoundData(clipName);
            var resultParam = this.GetResultAudioParams(sound, audioParam);

            return await this.PlayBGM(sound.audioClip, resultParam);
        }

        public async Task<bool> PlayDialog(string clipName, IAudioParams audioParam = null)
        {
            var sound = await this.GetSoundData(clipName);
            var resultParam = this.GetResultAudioParams(sound, audioParam);
            return await this.PlayDialog(sound.audioClip, resultParam);
        }

        public async Task<bool> PlayEffect(string clipName, IAudioParams audioParam = null)
        {
            var sound = await this.GetSoundData(clipName);
            var resultParam = this.GetResultAudioParams(sound, audioParam);
            return await this.PlayEffect(sound.audioClip, resultParam);
        }

        public async Task<AudioClip> PlayDialogGetAudioClip(string clipName, IAudioParams audioParam = null)
        {
            var sound = await this.GetSoundData(clipName);
            var resultParam = this.GetResultAudioParams(sound, audioParam);
            this.PlayDialog(sound.audioClip, resultParam);
            return sound.audioClip;
        }
        #endregion
    }
}