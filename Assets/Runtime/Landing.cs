using TMPro;
using UnityEngine;
using DG.Tweening;

namespace JingYe.SelfIntro
{
    public class Landing : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_Subtitle;

        [SerializeField]
        private Ease m_SubtitleAnimEase = Ease.InCubic;

        [SerializeField]
        private float m_SubtitleAnimDuration = 0.8f;

        private void Awake()
        {
            if (m_Subtitle != null)
            {
                var dstColor = m_Subtitle.color;
                dstColor.a = 0.2f;

                m_SubtitleTween = m_Subtitle.DOColor(dstColor, m_SubtitleAnimDuration)
                    .SetAutoKill(false)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(m_SubtitleAnimEase)
                    .Pause();
            }
        }

        private void OnEnable()
        {
            if (m_SubtitleTween != null)
                m_SubtitleTween.Play();
        }

        private void OnDisable()
        {
            if (m_SubtitleTween != null)
                m_SubtitleTween.Pause();
        }

        private void Update()
        {
            if (Input.touchCount > 0)
                _GoToGameScene();
            else if (Input.GetMouseButtonDown(0))
                _GoToGameScene();
        }

        private void _GoToGameScene()
        {
            Avatar.Instance.Hatch();
            SceneMgr.Instance.ChangeScene("Game");
        }

        private Tween m_SubtitleTween;
    } // END class
} // END namespace
