using System;

namespace JingYe.SelfIntro
{
    public static class WorldEventSystem
    {
        /***** scene events *****/
        public static Action<string/*next scene*/> OnBeforeChangeScene;
        public static Action<string/*loading scene*/, float /*progress*/> OnUpdateChangeSceneProgress;
        public static Action<string/*current scene*/> OnAfterChangeScene;
    } // END class
} // END namespace
