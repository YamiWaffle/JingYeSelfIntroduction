using JingYe.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace JingYe.SelfIntro
{
    public class Soundy : Singleton<Main>
    {
        [SerializeField]
        private AudioMixerGroup audioMixerGroup = null;

        [SerializeField]
        private AudioSource m_BGM;

        [SerializeField]
        private int m_NumberOfChannels = 10;

        public void PlayBGM(string audioClipName)
        {
            if (string.IsNullOrEmpty(audioClipName)) return;

            if (SoundInfoList.Instance.TryGetSound(audioClipName, out SoundInfo soundInfo))
                PlayBGM(soundInfo.Clip);
        }

        public void PlayBGM(AudioClip clip)
        {
            if (m_BGM == null || clip == null) return;

            m_BGM.clip = clip;
            m_BGM.loop = true;
            m_BGM.Play();
        }

        public void StopBGM()
        {
            if (m_BGM == null) return;

            m_BGM.Stop();
        }

        public void PlaySound(string audioClipName)
        {
            if (string.IsNullOrEmpty(audioClipName)) return;

            if (SoundInfoList.Instance.TryGetSound(audioClipName, out SoundInfo soundInfo))
                PlaySound(soundInfo.Clip);
        }

        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null) return;

            var source = _GetChannel();
            if (source == null) return;

            source.clip = audioClip;
            source.Play();
        }

        protected override void Awake()
        {
            base.Awake();

            CreateSoundChannels(m_NumberOfChannels);
        }

        private void CreateSoundChannels(int channelCount = 20)
        {
            m_ChannelsHolder = new GameObject("SoundChannels");
            m_ChannelsHolder.transform.SetParent(transform);
            m_Channels = new List<AudioSource>();
            for (int i = 0; i < m_NumberOfChannels; ++i)
            {
                m_Channels.Add(m_ChannelsHolder.AddComponent<AudioSource>());
                m_Channels[i].playOnAwake = false;
                m_Channels[i].volume = 1;
                if (audioMixerGroup != null)
                {
                    m_Channels[i].outputAudioMixerGroup = audioMixerGroup;
                }
            }
        }

        private AudioSource _GetChannel()
        {
            for (int i = 0; i < m_Channels.Count; ++i)
            {
                if (!m_Channels[i].isPlaying)
                    return m_Channels[i];
            }
            return null;
        }

        private GameObject m_ChannelsHolder = null;
        private List<AudioSource> m_Channels = new List<AudioSource>();
    } // END class
} // END namespace
