using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Tutorial
{
    public class TutorialHandler : MonoBehaviour
    {
        [SerializeField] private TutorialData m_PopUpData;

        private void Awake()
        {
            if (GameplayData.s_DisplayTutorial)
            {
                enabled = false;
            }
            
            if (GameplayData.TutorialPartsHasBeenDisplayed == null)
            {
                GameplayData.TutorialPartsHasBeenDisplayed = new List<bool>();
                int count = Enum.GetNames(typeof(TutorialEvent)).Length;
                for (int i = 0; i < count; ++i)
                {
                    GameplayData.TutorialPartsHasBeenDisplayed.Add(false);   
                }    
            }
        }

        private void OnEnable()
        {
            GameStaticEvents.OnTutorialAsked += OnTutorialAsked;
        }

        private void OnDisable()
        {
            GameStaticEvents.OnTutorialAsked -= OnTutorialAsked;
        }

        private void OnTutorialAsked(TutorialEvent _event)
        {
            if (!ShouldDisplayTutorialFor(_event))
            {
                return;
            }
            
            GameplayData.TutorialPartsHasBeenDisplayed[(int) _event] = true;
            var data = m_PopUpData[(int) _event];
            
            GameStaticEvents.OnDisplayPopUp?.Invoke(data.Title, data.Text, data.Image);
        }

        private bool ShouldDisplayTutorialFor(TutorialEvent _event)
        {
            int index = (int)_event;
            if (index < 0 || index >= GameplayData.TutorialPartsHasBeenDisplayed.Count)
            {
                // TODO: assert
                Debug.LogError("Weird there");
                return false;
            }

            return !GameplayData.TutorialPartsHasBeenDisplayed[index];
        }
    }
    
    
}