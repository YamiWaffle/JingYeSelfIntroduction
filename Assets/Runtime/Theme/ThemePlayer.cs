using DG.Tweening;
using JingYe.Common;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JingYe.SelfIntro
{
    public class ThemePlayer : Singleton<ThemePlayer>
    {
        public event Action OnChangeThemeBefore;
        public event Action OnChangeThemeAfter;

        [SerializeField]
        private string m_FirstTheme;

        [SerializeField]
        private float m_ChangeThemeDuration = 5f;

        [SerializeField]
        private Image m_CurrentBackground;
        
        [SerializeField]
        private Image m_NextBackground;

        [SerializeField]
        private TextMeshProUGUI m_Dialog;

        [SerializeField]
        private TextMeshProUGUI m_Description;

        private void Start()
        {
            if (m_CurrentBackground != null)
            {
                m_CurrentBgPos = m_CurrentBackground.rectTransform.anchoredPosition;
                m_PreviousBgPos = m_CurrentBgPos;
                m_PreviousBgPos.x -= m_CurrentBackground.rectTransform.rect.width;
                m_NextBgPos = m_CurrentBgPos;
                m_NextBgPos.x += m_CurrentBackground.rectTransform.rect.width;
            }

            _EntryFirstTheme();
        }

        private void Update()
        {
            if (m_IsDuringChangeTheme) return;

            bool checkInput = (m_ThemeClipCoroutine == null) || (m_ThemeClipCoroutine != null && m_CurrentThemeClip.AllowSkip);

            if (checkInput)
            {
                if (Input.touchCount > 0)
                    _NextClip();
                else if (Input.GetMouseButtonDown(0))
                    _NextClip();
            }
        }

        private void _EntryFirstTheme()
        {
            _NextTheme(m_FirstTheme, true);
        }

        private void _NextClip()
        {
            if (m_CurrentTheme == null) return;

            int nextClipIndex = m_CurrentThemeClipIndex + 1;
            if (0 > nextClipIndex || nextClipIndex >= m_CurrentTheme.Clips.Count) return;

            m_CurrentThemeClipIndex = nextClipIndex;
            m_CurrentThemeClip = m_CurrentTheme.Clips[m_CurrentThemeClipIndex];

            _StopClip();
            m_ThemeClipCoroutine = StartCoroutine(_Wait(m_CurrentThemeClip.DelayStartSeconds, () => {
                _PlayClip(m_CurrentThemeClip);

                m_ThemeClipCoroutine = StartCoroutine(_Wait(m_CurrentThemeClip.DelayEndSeconds, () => {
                    m_ThemeClipCoroutine = null;

                    if (m_CurrentThemeClip.AutoPlayNext)
                        _NextClip();
                }));
            }));
        }

        private void _StopClip()
        {
            if (m_ThemeClipCoroutine != null)
            {
                StopCoroutine(m_ThemeClipCoroutine);
                m_ThemeClipCoroutine = null;
            }
        }

        private void _PlayClip(ThemeClip clip)
        {
            if (clip == null || 0 == clip.Action) return;

            if ((ThemeClip.ClipAction.ChangeDialog & clip.Action) > 0)
            {
                _ChangeDialog(clip.DialogText);
            }

            if ((ThemeClip.ClipAction.ChangeDescription & clip.Action) > 0)
            {
                _ChangeDescription(clip.DescriptionText);
            }

            if ((ThemeClip.ClipAction.AvatarLevelUp & clip.Action) > 0)
            {
                _AvatarLevelUp();
            }

            if ((ThemeClip.ClipAction.NextTheme & clip.Action) > 0)
            {
                _NextTheme(clip.NextThemeName, false);
            }
        }

        private void _ChangeDialog(string text)
        {
            if (m_Dialog == null) return;

            m_Dialog.text = text;
        }

        private void _ChangeDescription(string text)
        {
            if (m_Description == null) return;

            m_Description.text = text;
        }

        private void _AvatarLevelUp()
        {

        }

        private void _ClearText()
        {
            _ChangeDialog(string.Empty);
            _ChangeDescription(string.Empty);
        }

        public void _NextTheme(string name, bool instant)
        {
            if (!ThemeDatas.Instance.TryGetTheme(name, out Theme theme)) return;

            m_IsDuringChangeTheme = true;
            OnChangeThemeBefore?.Invoke();

            _ClearText();
            m_CurrentTheme = theme;
            m_CurrentThemeClipIndex = -1;

            if (instant)
            {
                m_CurrentBackground.rectTransform.anchoredPosition = m_CurrentBgPos;
                m_NextBackground.rectTransform.anchoredPosition = m_NextBgPos;
                m_CurrentBackground.overrideSprite = theme.Background;

                _NextClip();

                m_IsDuringChangeTheme = false;
                OnChangeThemeAfter?.Invoke();
            }
            else
            {
                if (m_CurrentBackground != null && m_NextBackground != null)
                {
                    m_NextBackground.overrideSprite = theme.Background;

                    m_CurrentBackground.rectTransform.anchoredPosition = m_CurrentBgPos;
                    m_NextBackground.rectTransform.anchoredPosition = m_NextBgPos;

                    m_CurrentBackground.rectTransform
                        .DOAnchorPos(m_PreviousBgPos, m_ChangeThemeDuration)
                        .OnComplete(() => {
                            m_CurrentBackground.rectTransform.anchoredPosition = m_CurrentBgPos;
                            m_CurrentBackground.overrideSprite = theme.Background;
                        });

                    m_NextBackground.rectTransform
                        .DOAnchorPos(m_CurrentBgPos, m_ChangeThemeDuration)
                        .OnComplete(() => {
                            m_NextBackground.rectTransform.anchoredPosition = m_NextBgPos;
                        });
                }

                StartCoroutine(_Wait(m_ChangeThemeDuration + 0.05f, () => {
                    _NextClip();

                    m_IsDuringChangeTheme = false;
                    OnChangeThemeAfter?.Invoke(); 
                }));
            }
        }

        private IEnumerator _Wait(float seconds, Action onWaited)
        {
            if (seconds > 0f)
                yield return new WaitForSecondsRealtime(seconds);

            onWaited?.Invoke();
        }

        private Vector2 m_PreviousBgPos;
        private Vector2 m_CurrentBgPos;
        private Vector2 m_NextBgPos;

        private bool m_IsDuringChangeTheme;
        private Theme m_CurrentTheme;
        private ThemeClip m_CurrentThemeClip;
        private int m_CurrentThemeClipIndex;

        private Coroutine m_ThemeClipCoroutine;
    } // END class
} // END namespace
