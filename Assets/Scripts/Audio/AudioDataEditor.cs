using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Audio
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioDataEditor : ReorderableListEditor
    {
        private const string Path = "Assets/Scripts/Audio/";


        protected override void Setup()
        {
            AddList(AudioManager.AudioTypeListName, "Audio Type");
            //AddPropertyField(AudioType.AudioDataName, 0.8f);
            AddLabelField(0.2f, "Enum Name");
            AddRow();
            AddListField();
            AddList(AudioType.AudioDataListName, "Audio Data");
            AddListField();
            AddLabelField(0.5f, "Clip");
            AddLabelField(0.5f, "Unique Name");
            AddRow();
            AddPropertyField(AudioData.AudioClipFieldName, 0.5f);
            AddPropertyField(AudioData.AudioNameFieldName, 0.5f);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!GUILayout.Button("SAVE")) return;

            AudioManager audioManager = (AudioManager) target;
            HashSet<string> uniqueNames = new HashSet<string>();
            if (audioManager.AudioTypes.Any(types => types.AudioData.Any(data => !uniqueNames.Add(data.AudioName))))
            {
                EditorUtility.DisplayDialog("Warning!",
                    "All audio names must be UNIQUE!", "OK");
                return;
            }

            EnumGenerator enumGenerator = new EnumGenerator(Path, "AudioEnum");
            enumGenerator.GenerateEnums(uniqueNames, "AudioEnum");
        }
    }
}