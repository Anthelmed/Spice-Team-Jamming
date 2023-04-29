using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class DummyWorld : MonoBehaviour
    {
        public static DummyWorld Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}