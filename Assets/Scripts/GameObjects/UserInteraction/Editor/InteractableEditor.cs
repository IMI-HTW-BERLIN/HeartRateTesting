using UnityEditor;
using UnityEngine;

namespace GameObjects.UserInteraction.Editor
{
    [CustomEditor(typeof(Interactable), true)]
    public class InteractableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Interactable interactable = (Interactable) target;
            if (GUILayout.Button("Interact")) 
                interactable.Interact();
        }
    }
}