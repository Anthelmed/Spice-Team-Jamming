using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(Locomotion))]
    public class LocomotionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;
            var locomotion = target as Locomotion;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Destination", locomotion.Destination, typeof(Transform), true);
            EditorGUILayout.ObjectField("Look At", locomotion.LookAtTarget, typeof(Transform), true);
            EditorGUI.EndDisabledGroup();
        }
    }
}