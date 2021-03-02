using JingYe.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JingYe.SelfIntro
{
    public class Main : Singleton<Main>
    {
        public void ChangeNextScenario()
        {
            // Play audio
            // Play avatar walking
        }

        private void Start()
        {
            SceneMgr.Instance.ChangeScene("Landing");
        }
    } // END class
} // END namespace

