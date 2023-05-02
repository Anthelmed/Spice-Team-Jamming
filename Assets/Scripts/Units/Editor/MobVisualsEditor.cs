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
                visuals.SetAnimation(MobVisuals.AnimationID.Idle);
            if (GUILayout.Button("Walk"))
                visuals.SetAnimation(MobVisuals.AnimationID.Walk);
            if (GUILayout.Button("Attack"))
                visuals.SetAnimation(MobVisuals.AnimationID.Attack);
            if (GUILayout.Button("Ranged Attack"))
                visuals.SetAnimation(MobVisuals.AnimationID.RangedAttack);
            if (GUILayout.Button("Hit"))
                visuals.SetAnimation(MobVisuals.AnimationID.Hit);
            if (GUILayout.Button("Death"))
                visuals.SetAnimation(MobVisuals.AnimationID.Death);
        }
    }
}