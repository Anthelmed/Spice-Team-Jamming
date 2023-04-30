using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DefaultNamespace
{
    public static class PlayerStaticEvents
    {
        public static Action<float> s_PlayerHealthChanged;
        public static Action<float> s_PlayerManaChanged;
        public static Action s_OnActionImpossibleBecauseOfMana;
        
#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void InitAll()
        {
            s_PlayerHealthChanged = null;
            s_PlayerManaChanged = null;
            s_OnActionImpossibleBecauseOfMana = null;
        }
#endif
    }
}