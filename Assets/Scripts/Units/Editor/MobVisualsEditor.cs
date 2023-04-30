using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(MobVisuals), true)]
    public class MobVisualsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;
            var visuals = target as MobVisuals;

            if (GUILayout.Button("Idle"))
                visuals.SetSpeed(0f);
            if (GUILayout.Button("Walk"))
                visuals.SetSpeed(10f);
            if (GUILayout.Button("Attack"))
                visuals.TriggerAttack();
            if (GUILayout.Button("Hit"))
                visuals.TriggerHit();
            if (GUILayout.Button("Death"))
                visuals.TriggerDeath();
        }
    }
}