using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Managers;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Used https://stackoverflow.com/a/54692864 as guidance.
    /// </summary>
    public abstract class ReorderableListEditor : Editor
    {
        private ReorderableList _list;

        private readonly Dictionary<string, ReorderableList> _innerListDict = new Dictionary<string, ReorderableList>();

        private readonly List<string> _listNames = new List<string>();

        private readonly List<string> _listDisplayNames = new List<string>();

        private string _currentList;

        /// <summary>
        /// Will be filled by calling <see cref="AddLabelField"/>, <see cref="AddPropertyField"/>
        /// or <see cref="AddRow"/>. Used setting up all the LabelFields and PropertyFields with their dimensions and
        /// labels/property names.
        /// </summary>
        private readonly Dictionary<string, List<FieldData>> _fields = new Dictionary<string, List<FieldData>>();

        /// <summary>
        /// Number of rows each element has. Used to define the height of an element.
        /// </summary>
        private readonly Dictionary<string, int> _customHeight = new Dictionary<string, int>();

        /// <summary>
        /// Easier access to the default LineHeight of the Editor (see: <see cref="EditorGUIUtility.singleLineHeight"/>). 
        /// </summary>
        private static readonly float LineHeight = EditorGUIUtility.singleLineHeight;

        private void OnEnable()
        {
            Setup();

            if (_listNames.Count == 0)
                return;

            _list = new ReorderableList(serializedObject,
                serializedObject.FindProperty(_listNames[0]),
                true, true, true, true);

            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                AddFieldsToList(_listNames[0], rect, _list.serializedProperty,
                    newRect => DrawElementCallback(newRect, index, _list, 1));
            };


            _list.drawHeaderCallback = rect => DrawHeaderCallback(rect, 0);

            _list.elementHeightCallback = index => ElementHeightCallback(index, _list, 0);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _list?.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void Setup();

        protected void AddList(string uniqueListName, string displayName)
        {
            if (_fields.ContainsKey(uniqueListName))
                throw new ArgumentException(
                    "A list with the same name was already added. Make sure the lists have unique names.");

            _listNames.Add(uniqueListName);
            _listDisplayNames.Add(displayName);
            _fields.Add(uniqueListName, new List<FieldData>());
            _customHeight.Add(uniqueListName, 0);

            _currentList = uniqueListName;
        }

        protected void AddListField(string listName)
        {
            CheckListName(listName);
            _fields[listName].Add(new FieldData
                {FieldType = FieldType.InnerList});
        }

        protected void AddListField() => AddListField(_currentList);

        /// <summary>
        /// Adds a new empty row to the element. Use this to add fields to other rows or to add space.
        /// <para>If fields have a height greater than 1, you need to add empty rows as well. Otherwise the field will be
        /// drawn on top of other fields.
        /// </para>
        /// </summary>
        protected void AddRow(string listName)
        {
            CheckListName(listName);
            _fields[listName].Add(new FieldData {FieldType = FieldType.Line});
            _customHeight[listName]++;
        }

        protected void AddRow() => AddRow(_currentList);

        /// <summary>
        /// Adds a label field to the element.
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="widthRatio">The width of the field regarding the whole width of the element (0-1).</param>
        /// <param name="labelText">The text to be displayed</param>
        /// <param name="heightFactor">The height of the field.
        /// Will be multiplied with EditorGUIUtility's <see cref="EditorGUIUtility.singleLineHeight"/>.</param>
        protected void AddLabelField(string listName, float widthRatio, string labelText, int heightFactor = 1)
        {
            CheckListName(listName);
            _fields[listName].Add(new FieldData
            {
                FieldType = FieldType.Label,
                WidthRatio = widthRatio,
                HeightFactor = heightFactor,
                LabelName = labelText
            });
        }

        protected void AddLabelField(float widthRatio, string labelText, int heightFactor = 1) =>
            AddLabelField(_currentList, widthRatio, labelText, heightFactor);

        /// <summary>
        /// Adds a property field to the element.
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="widthRatio">The width of the field regarding the whole width of the element (0-1).</param>
        /// <param name="propertyName">The name of the property that will be displayed.</param>
        /// <param name="heightFactor">The height of the field.
        /// Will be multiplied with EditorGUIUtility's <see cref="EditorGUIUtility.singleLineHeight"/>.</param>
        protected void AddPropertyField(string listName, string propertyName, float widthRatio, int heightFactor = 1)
        {
            CheckListName(listName);
            _fields[listName].Add(new FieldData
            {
                FieldType = FieldType.Property,
                PropertyName = propertyName,
                WidthRatio = widthRatio,
                HeightFactor = heightFactor
            });
        }

        protected void AddPropertyField(string propertyName, float widthRatio, int heightFactor = 1) =>
            AddPropertyField(_currentList, propertyName, widthRatio, heightFactor);

        [AssertionMethod]
        private void CheckListName(string listName)
        {
            if (!_listNames.Contains(listName))
                throw new ArgumentException("List Name not found. Make sure it matches.");
        }

        private void DrawElementCallback(Rect rect, int index, ReorderableList outerReorderableList, int level)
        {
            SerializedProperty outerElement = outerReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty innerList = outerElement.FindPropertyRelative(_listNames[level]);

            string listKey = outerElement.propertyPath;
            ReorderableList innerReorderableList;

            // Not in dictionary => New list, in dictionary => Grab reference (Needed for interaction e.g. dragging).
            if (!_innerListDict.ContainsKey(listKey))
            {
                innerReorderableList = new ReorderableList(outerElement.serializedObject, innerList,
                    true, true, true, true);
                _innerListDict.Add(listKey, innerReorderableList);
            }
            else
                innerReorderableList = _innerListDict[listKey];

            // Define recursive element callback.
            innerReorderableList.drawElementCallback = (innerRect, innerIndex, innerActive, innerFocus) =>
            {
                SerializedProperty innerElement =
                    innerReorderableList.serializedProperty.GetArrayElementAtIndex(innerIndex);
                AddFieldsToList(_listNames[level], innerRect, innerElement, InnerCallback);

                void InnerCallback(Rect newRect)
                {
                    if (level < _listNames.Count - 1)
                        DrawElementCallback(newRect, innerIndex, innerReorderableList, level + 1);
                }
            };
            innerReorderableList.drawHeaderCallback = headerRect => DrawHeaderCallback(headerRect, level);

            // Returns the size of the inner list and uses it for own size
            // Deepest list does not have an inner list => Only get own size.
            if (level < _listNames.Count - 1)
                innerReorderableList.elementHeightCallback = heightIndex =>
                    ElementHeightCallback(heightIndex, innerReorderableList, level);
            else
                innerReorderableList.elementHeightCallback =
                    heightIndex => _customHeight[_listNames[level]] * LineHeight + LineHeight;

            innerReorderableList.DoList(new Rect(rect.x, rect.y, rect.width, rect.height));
        }

        private void DrawHeaderCallback(Rect rect, int level) => EditorGUI.LabelField(rect, _listDisplayNames[level]);

        private float ElementHeightCallback(int index, ReorderableList outerReorderableList, int level)
        {
            SerializedProperty outerElement = outerReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            string listKey = outerElement.propertyPath;

            // If not in dictionary, skip.
            if (!_innerListDict.ContainsKey(listKey))
                return 0;

            // Otherwise get reference and size.
            ReorderableList innerReorderableList = _innerListDict[listKey];
            return innerReorderableList.GetHeight() + _customHeight[_listNames[level]] * LineHeight + LineHeight;
        }

        private void AddFieldsToList(string listName, Rect rect, SerializedProperty element,
            Action<Rect> innerDrawHeaderCallback)
        {
            float currentX = 0;
            int currentRow = 0;
            foreach (FieldData field in _fields[listName])
            {
                float x = rect.x + currentX;
                float y = rect.y + currentRow * LineHeight;

                float width = rect.width * field.WidthRatio;
                switch (field.FieldType)
                {
                    case FieldType.InnerList:
                        rect.y = y;
                        innerDrawHeaderCallback?.Invoke(rect);
                        break;
                    case FieldType.Label:
                        EditorGUI.LabelField(
                            new Rect(x, y, width, LineHeight * field.HeightFactor),
                            field.LabelName);
                        break;
                    case FieldType.Property:
                        SerializedProperty property = element.FindPropertyRelative(field.PropertyName);
                        if (property == null)
                            throw new ArgumentException(
                                $"{field.PropertyName} property could not be found in {element.name}.");
                        EditorGUI.PropertyField(
                            new Rect(x, y, width, LineHeight * field.HeightFactor), property, GUIContent.none);
                        break;
                    case FieldType.Line:
                        currentRow++;
                        currentX = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                currentX += width;
            }
        }

        #region INNER_CLASSES

        /// <summary>
        /// All different types of fields.
        /// </summary>
        private enum FieldType
        {
            InnerList,
            Label,
            Property,
            Line
        }

        /// <summary>
        /// Small struct for keeping track of all the parameters of a field.
        /// </summary>
        private struct FieldData
        {
            public FieldType FieldType;
            public string PropertyName;
            public float WidthRatio;
            public int HeightFactor;
            public string LabelName;
        }

        #endregion
    }
}