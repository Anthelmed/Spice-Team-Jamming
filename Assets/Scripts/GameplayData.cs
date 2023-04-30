using System.Collections.Generic;
using _3C.Player;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public static class GameplayData
    {
        public static PlayerStateHandler s_PlayerStateHandler;
        public static PlayerInputs s_PlayerInputs;
        public static bool s_DisplayTutorial = true;
        public static List<bool> TutorialPartsHasBeenDisplayed;
#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void InitAll()
        {
            s_PlayerInputs = null;
        }
#endif
    }
}