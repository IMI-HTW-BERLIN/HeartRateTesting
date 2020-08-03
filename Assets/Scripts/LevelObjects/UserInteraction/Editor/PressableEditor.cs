using UnityEditor;
using UnityEngine;

namespace LevelObjects.UserInteraction.Editor
{
    [CustomEditor(typeof(Pressable), true)]
    public class PressableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Pressable pressable = (Pressable) target;
            if (GUILayout.Button("Press"))
                pressable.Press();
            if (GUILayout.Button("ShowInReach"))
                pressable.ShowInReach();
            if (GUILayout.Button("HideInReach"))
                pressable.HideInReach();
        }
    }
}