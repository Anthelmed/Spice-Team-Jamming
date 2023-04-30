using System;
using UnityEngine;

namespace DefaultNamespace.Tutorial
{
    public enum TutorialEvent
    {
        
    }

    [Serializable]
    public struct TutorialItem
    {
        public string Title;
        public string Text;
        public Sprite Image;
    }
    
    [CreateAssetMenu(fileName = "TutorialData", menuName = "TutorialData", order = 0)]
    public class TutorialData : ScriptableObject
    {
        [SerializeField] private TutorialItem[] m_PopUpData;
        
        public int Length => m_PopUpData.Length;
        public TutorialItem this[int _index] => m_PopUpData[_index];
    }
}