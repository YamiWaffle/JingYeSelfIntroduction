using System;

namespace JingYe.SelfIntro
{
    [Serializable]
    public class ThemeClip
    {
        [Flags]
        public enum ClipAction
        {
            //None = 0,
            ChangeDialog = 1 << 0,
            ChangeDescription = 1 << 1,
            AvatarLevelUp = 1 << 2,
            NextTheme = 1 << 3,
        }

        public ClipAction Action;
        public string DialogText;
        public string DescriptionText;
        public string NextThemeName;
        public float DelayStartSeconds;
        public float DelayEndSeconds;
        public bool AutoPlayNext;
        public bool AllowSkip;
    } // END class
} // END namespace
