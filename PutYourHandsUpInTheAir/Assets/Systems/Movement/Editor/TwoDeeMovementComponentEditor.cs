using System.Globalization;
using Systems.Movement;
using UnityEditor;
using UnityEngine;

namespace Assets.Systems.Movement.Editor
{
    [CustomEditor(typeof(TwoDeeMovementComponent))]
    public class TwoDeeMovementComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var component = (TwoDeeMovementComponent) target;
            GUILayout.Label("Configure", EditorStyles.boldLabel);

            DrawSpeedSlider(component);
            GUILayout.Space(20f);
            DrawFrictionSlider(component);


            GUILayout.Space(20f);
            GUILayout.Label("Computed", EditorStyles.boldLabel);

            var projectedSpeed = component.Speed / component.Friction;
            GUILayout.Label("Projected Max Speed: " + projectedSpeed);
        }

        private static void DrawSpeedSlider(TwoDeeMovementComponent component)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Speed", GUILayout.Width(300));
            GUILayout.TextField(component.Speed.ToString());
            component.Speed = GUILayout.HorizontalSlider(component.Speed, 0.00001f, 100);
            GUILayout.EndVertical();
        }

        private static void DrawFrictionSlider(TwoDeeMovementComponent component)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Friction", GUILayout.Width(300));
            GUILayout.TextField(component.FrictionPercent.ToString());
            component.FrictionPercent = GUILayout.HorizontalSlider(component.FrictionPercent, 0.00001f, 1);
            component.Friction = component.FrictionPercent * component.Speed;
            GUILayout.EndVertical();
        }
    }
}
