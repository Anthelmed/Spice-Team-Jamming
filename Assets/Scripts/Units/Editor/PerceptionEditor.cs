using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(Perception))]
    public class PerceptionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;
            var perception = target as Perception;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Query Targets", perception.QueryTargetEnabled);
            EditorGUILayout.ObjectField("Target", perception.Target, typeof(Perception), true);
            EditorGUI.EndDisabledGroup();
        }
    }
}