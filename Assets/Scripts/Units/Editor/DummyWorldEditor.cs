using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(DummyWorld))]
    public class DummyWorldEditor : Editor
    {
        private bool vegFoldout = false;
        private bool pawnFoldout = false;
        private bool knightFoldout = false;
        private bool playerFoldout = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("In Game Debug", EditorStyles.boldLabel);
            var world = target as DummyWorld;

            vegFoldout = EditorGUILayout.Foldout(vegFoldout, "Vegetation", true);
            if (vegFoldout) DrawList(world.Vegetation);

            pawnFoldout = EditorGUILayout.Foldout(pawnFoldout, "Pawns", true);
            if (pawnFoldout) DrawList(world.Pawns);

            knightFoldout = EditorGUILayout.Foldout(knightFoldout, "Knights", true);
            if (knightFoldout) DrawList(world.Knights);

            playerFoldout = EditorGUILayout.Foldout(playerFoldout, "Players", true);
            if (playerFoldout) DrawList(world.Players);
        }

        private void DrawList(List<Unit> list)
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(true);
            foreach (var unit in list)
            {
                EditorGUILayout.ObjectField(unit, typeof(Unit), true);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
        }
    }
}