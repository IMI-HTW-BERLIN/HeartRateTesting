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
            AddLabelField(0.5f, "Unique Audio Type Name");
            AddPropertyField(AudioType.AudioDataFieldName, 0.5f);
            AddRow();
            AddListField();
            AddList(AudioType.AudioDataListFieldName, "Audio Data");
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
            List<HashSet<string>> uniqueLists = new List<HashSet<string>>();
            HashSet<string> enumNames = new HashSet<string>();

            foreach (AudioType audioType in audioManager.AudioTypes)
            {
                if (!enumNames.Add(audioType.AudioDataName))
                {
                    EditorUtility.DisplayDialog("Warning!",
                        "All audio type names must be UNIQUE!", "OK");
                    return;
                }

                HashSet<string> uniqueNames = new HashSet<string>();
                if (audioType.AudioData.Any(audioData => !uniqueNames.Add(audioData.AudioName)))
                {
                    EditorUtility.DisplayDialog("Warning!",
                        "All audio names must be UNIQUE!", "OK");
                    return;
                }

                uniqueLists.Add(uniqueNames);
            }

            EnumGenerator enumGenerator = new EnumGenerator(Path, "AudioEnum");
            enumGenerator.GenerateEnums(uniqueLists, "AudioEnum", enumNames.ToArray());
        }
    }
}