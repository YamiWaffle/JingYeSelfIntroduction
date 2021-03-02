using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JingYe.SelfIntro
{
    public class Game : MonoBehaviour
    {
        [SerializeField]
        private Image m_MainScreenBg1;

        [SerializeField]
        private Image m_MainScreenBg2;

        [SerializeField]
        private TextMeshProUGUI m_Dialog;

        public void NextClip()
        {
            // TODO
            Debug.Log("NextClip");
        }

        public void ChangeDialogText(string text)
        {
            if (m_Dialog == null) return;

            m_Dialog.text = text;
        }

        //private void Update()
        //{
        //    if (m_AllowTap)
        //    {
        //        if (Input.touchCount > 0) 
        //            NextClip();
        //        else if (Input.GetMouseButtonDown(0))
        //            NextClip();
        //    }
        //}

        //private bool m_AllowTap;
    } // END class
} // END namespace
