using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(Unit), true)]
    public class UnitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;
            var unit = (Unit)target;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("In Game Debug", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField("Current health", unit.CurrentHealth);
            EditorGUI.EndDisabledGroup();
        }
    }
}