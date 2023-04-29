using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(Unit), true)]
    public class UnitEditor : Editor
    {
        bool hitFoldout = false;
        float amount = 1f;
        Unit other;

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

            hitFoldout = EditorGUILayout.Foldout(hitFoldout, "Hit", true);
            if (hitFoldout)
            {
                EditorGUI.indentLevel++;

                amount = EditorGUILayout.FloatField("Damage", amount);
                other = EditorGUILayout.ObjectField("Other", other, typeof(Unit), true) as Unit;
                if (GUILayout.Button("Hit!"))
                    unit.TakeHit(amount, other, other.transform.position);

                EditorGUI.indentLevel--;
            }
        }
    }
}