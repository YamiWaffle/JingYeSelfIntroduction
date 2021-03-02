using DG.Tweening;
using JingYe.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace JingYe.SelfIntro
{
    public class Avatar : Singleton<Avatar>
    {
        public event Action OnHatched;

        [SerializeField]
        private Image m_Mask;

        [SerializeField]
        private GameObject m_Egg;

        [SerializeField]
        private GameObject m_People;

        [SerializeField]
        private float m_HatchDuration = 6f;

        public void Hatch()
        {
            float duration = m_HatchDuration / 2;

            if (m_Egg != null)
            {
                m_Egg.transform.DOScale(Vector3.one * 2, duration)
                .OnComplete(() => {
                    m_Egg.gameObject.SetActive(false);

                    if (m_People != null)
                    {
                        m_People.gameObject.SetActive(true);
                        m_People.transform.position = m_Egg.transform.position;
                        m_People.transform.localScale = Vector3.one * 2;
                        m_People.transform.DOLocalMove(m_PeopleOrgPos, duration);
                        m_People.transform.DOScale(Vector3.one, duration).OnComplete(() => OnHatched?.Invoke());
                    }
                });
            }

            if (m_Mask != null)
            {
                Color trasnparentClr = new Color(0, 0, 0, 0);
                m_Mask.gameObject.SetActive(true);
                m_Mask.color = Color.black;
                m_Mask.DOColor(trasnparentClr, duration)
                    .SetDelay(duration)
                    .OnComplete(() => {
                        m_Mask.gameObject.SetActive(false);
                    });
            }
        }

        public void Walk()
        {
            // TODO
        }

        public void Smile()
        {
            // TODO
        }

        public void Pause()
        {
            // TODO
        }

        public void LevelUp()
        {
            // TODO

            // Play sound

            // ChangeClothes?
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Egg != null)
            {
                m_Egg.gameObject.SetActive(true);
            }

            if (m_People != null)
            {
                m_PeopleOrgPos = m_People.transform.localPosition;
                m_People.gameObject.SetActive(false);
            }
        }

        private Vector3 m_PeopleOrgPos;
    } // END class
} // END namespace
