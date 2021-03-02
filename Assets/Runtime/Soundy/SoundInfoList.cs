using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JingYe.SelfIntro
{
    public class SoundInfoList : ScriptableObject, ISerializationCallbackReceiver
    {
        private static SoundInfoList s_Instance;
        public static SoundInfoList Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    if (Application.isPlaying)
                        s_Instance = Resources.Load<SoundInfoList>("SoundInfoList");
#if UNITY_EDITOR
                    else
                        s_Instance = ScriptableObjectHelper.GetOrCreate<SoundInfoList>("Assets/Resources", "SoundInfoList");
#endif
                }

                return s_Instance;
            }
        }

        [SerializeField]
        private List<SoundInfo> m_SoundInfoList;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("JingYe/SoundInfoList")]
        private static void SelectSoundInfoList()
        {
            UnityEditor.Selection.activeObject = Instance;
        }
#endif

        public IEnumerable<SoundInfo> GetSoundInfos()
        {
            if (m_SoundInfoList == null)
                return Enumerable.Empty<SoundInfo>();

            return m_SoundInfoList;
        }

        public bool TryGetSound(string name, out SoundInfo soundInfo)
        {
            return m_SoundInfoDict.TryGetValue(name, out soundInfo);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_SoundInfoList == null) return;

            foreach (var soundInfo in m_SoundInfoList)
            {
                m_SoundInfoDict[soundInfo.Name] = soundInfo;
            }
        }

        private readonly IDictionary<string, SoundInfo> m_SoundInfoDict = new Dictionary<string, SoundInfo>();
    } // END class
} // END namespace

