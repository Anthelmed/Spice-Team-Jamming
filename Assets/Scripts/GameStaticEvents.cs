using System;
using DefaultNamespace.Tutorial;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameStaticEvents
    {
        public static Action<TutorialEvent> OnTutorialAsked;
        public static Action<string, string, Sprite> OnDisplayPopUp;
    }
}