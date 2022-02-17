using UnityEngine;
using ETModel;
using System.Threading.Tasks;

namespace ETHotfix
{
    /// <summary>
    /// 声音系统使用案例和热更层参考接口 所有接口都会调用一遍防止binding没有生成
    /// </summary>
    public static class AudioManagerUseSimple
    {
        public static async Task<bool> PlayBGM(AudioClip clip, IAudioParams audioParam = null)
        {
            return await AudioManager.instance.PlayBGM(clip, audioParam);
        }
        public static async Task<bool> PlayBGM(string clipName, IAudioParams audioParam = null)
        {
            return await AudioManager.instance.PlayBGM(clipName, audioParam);
        }

        public static async Task<bool> PlayDialog(AudioClip clip, IAudioParams audioParam = null)
        {
            return await AudioManager.instance.PlayDialog(clip, audioParam);
        }
        public static async Task<bool> PlayDialog(string clipName, IAudioParams audioParam = null)
        {
            return await AudioManager.instance.PlayDialog(clipName, audioParam);
        }

        public static async Task<bool> PlayEffect(AudioClip clip, IAudioParams audioParam = null)
        {
            return await AudioManager.instance.PlayEffect(clip, audioParam);
        }
        public static async Task<bool> PlayEffect(string clipName, IAudioParams audioParam = null)
        {
            return await AudioManager.instance.PlayEffect(clipName, audioParam);
        }


        public static void StopBGM()
        {
            AudioManager.instance.StopBGM();
        }
        public static void StopDialog()
        {
            AudioManager.instance.StopDialog();
        }
        public static void StopEffect()
        {
            AudioManager.instance.StopEffect();
        }


        public static void PauseBGM()
        {
            var source = AudioManager.instance.GetBgmAudioSource;
            source?.Pause();
        }
        public static void ResumeBGM()
        {
            var source = AudioManager.instance.GetBgmAudioSource;
            source?.UnPause();
        }

        public static void PauseDialog()
        {
            var source = AudioManager.instance.GetDialogAudioSource;
            source?.Pause();
        }
        public static void ResumeDialog()
        {
            var source = AudioManager.instance.GetDialogAudioSource;
            source?.UnPause();
        }

        public static void SetBGMMute(bool status)
        {
            AudioManager.instance.MusicMute = status;
        }
        public static void SetEffectMute(bool status)
        {
            AudioManager.instance.SoundMute = status;
        }
        public static void SetSoundVol(float vol)
        {
            AudioManager.instance.SoundVolume = vol;
        }
    }
}