using UnityEditor;
using UnityEngine;

namespace Units
{
    [CustomEditor(typeof(Mob), true)]
    public class MobEditor : UnitEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
                return;
            var mob = target as Mob;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("AI Debug", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("Current State", mob.CurrentState);
            EditorGUI.EndDisabledGroup();
        }
    }
}