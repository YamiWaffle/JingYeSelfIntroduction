using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JingYe.SelfIntro
{
    public class ThemeDatas : ScriptableObject, ISerializationCallbackReceiver
    {
        private static ThemeDatas s_Instance;
        public static ThemeDatas Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    if (Application.isPlaying)
                        s_Instance = Resources.Load<ThemeDatas>("ThemeDatas");
#if UNITY_EDITOR
                    else
                        s_Instance = ScriptableObjectHelper.GetOrCreate<ThemeDatas>("Assets/Resources", "ThemeDatas");
#endif
                }

                return s_Instance;
            }
        }

        [SerializeField]
        private List<Theme> m_ThemeList;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("JingYe/ThemeDatas")]
        private static void SelectThemeDatasList()
        {
            UnityEditor.Selection.activeObject = Instance;
        }
#endif

        public IEnumerable<Theme> GetThemeDatas()
        {
            if (m_ThemeList == null)
                return Enumerable.Empty<Theme>();

            return m_ThemeList;
        }

        public bool TryGetTheme(string name, out Theme theme)
        {
            return m_ThemeDict.TryGetValue(name, out theme);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_ThemeList == null) return;

            foreach (var theme in m_ThemeList)
            {
                m_ThemeDict[theme.Name] = theme;
            }
        }

        private readonly IDictionary<string, Theme> m_ThemeDict = new Dictionary<string, Theme>();
    } // END class
} // END namespace


