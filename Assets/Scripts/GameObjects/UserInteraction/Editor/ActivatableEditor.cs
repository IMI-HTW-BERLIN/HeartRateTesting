using UnityEditor;
using UnityEngine;

namespace GameObjects.UserInteraction.Editor
{
    [CustomEditor(typeof(Activatable), true)]
    public class ActivatableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            Activatable activatable = (Activatable) target;
            if (GUILayout.Button("Activate")) 
                activatable.Activate();

            if (GUILayout.Button("Deactivate")) 
                activatable.Deactivate();
            
            if (GUILayout.Button("Toggle")) 
                activatable.Toggle();

            
        }
    }
}