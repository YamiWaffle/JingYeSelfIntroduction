using System;
using System.Collections.Generic;
using UnityEngine;

namespace JingYe.SelfIntro
{
    [Serializable]
    public class Theme
    {
        public string Name;
        public Sprite Background;
        public List<ThemeClip> Clips;

        // For editor
        public bool FoldoutClips;
    } // END class
} // END namespace
