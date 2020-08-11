using UnityEditor;
using UnityEngine;

namespace LevelObjects.Editor
{
    [CustomEditor(typeof(Highlightable))]
    public class HighlightableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Highlightable highlightable = (Highlightable) target;
            if (GUILayout.Button("ShowHighlight"))
                highlightable.ShowHighlight();
            if (GUILayout.Button("HideHighlight"))
                highlightable.HideHighlight();
        }
    }
}