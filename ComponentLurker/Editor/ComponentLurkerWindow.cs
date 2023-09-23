using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ComponentLurker.Drawers;
using ComponentLurker.Utils;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker
{
    public class ComponentLurkerWindow : EditorWindow
    {
        private MonoScript _selectedScript;
        private MonoScript _prevScript;
        
        private static readonly Dictionary<Type, string[]> ComparisonOperationsNamesByDrawerType = new();
        private static readonly HashSet<Type> ChosenDrawableTypes = new(20);

        private readonly List<SelectedFieldData> _selectedFields = new(20);
        private readonly List<SelectedFieldData> _elementsToRemove = new(20);

        private static HashSet<(Type drawerType, Type type)> _eligibleDrawableTypes;
        private static HashSet<(Type drawerType, Type type)> _allDrawableTypes;
        private bool _showChosenTypes;
        private List<FieldInfo> _allComponentFields;
        private List<FieldInfo> _eligibleFields;
        private Vector2 _fieldsScrollPos;
        private bool _chosenDrawableTypesChanged;

        private bool _isSelectionInProcess;
        private FieldInfo _selectedField;
        private int _selectedFieldIndex;
        private string[] _fieldsNames;

        private List<(GameObject prefab, Component component)> _foundPrefabs;

        [MenuItem("Utils/ComponentLurker")]
        public static void ShowWindow()
        {
            UpdateAllDrawableTypes();
            GetWindow<ComponentLurkerWindow>("ComponentLurker");
        }

        private static void UpdateAllDrawableTypes() => 
            _allDrawableTypes ??= Finder.FindAllDrawableTypes();

        private void OnGUI()
        {
            _selectedScript = (MonoScript) EditorGUILayout.ObjectField(_selectedScript, typeof(MonoScript), true);

            if (NotComponentScript())
            {
                EditorGUILayout.LabelField("Choose component script");
                return;
            }

            EditorGUILayout.BeginVertical();
            {
                DrawChosenTypes();
                DrawFieldsScroll();
                DrawFindButton();
            }
            EditorGUILayout.EndVertical();
        }

        private bool NotComponentScript() => 
            _selectedScript == null || _selectedScript.GetClass().IsSubclassOf(typeof(Component)) == false;

        private void DrawFindButton()
        {
            if (!GUILayout.Button("Find")) 
                return;
            
            _foundPrefabs = Finder.FindAllPrefabs(_selectedScript.GetClass(), _selectedFields);
            ComponentLurkerResultWindow.ShowWindow(_foundPrefabs);
        }

        private void DrawChosenTypes()
        {
            UpdateFields();
            
            EditorGUILayout.BeginHorizontal();
            _showChosenTypes = EditorGUILayout.Foldout(_showChosenTypes, "Search for");
            
            EditorGUILayout.EndHorizontal();
            if (!_showChosenTypes) 
                return;
            
            foreach (var (_, type) in _eligibleDrawableTypes)
            {
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.Toggle(type.Name, ChosenDrawableTypes.Contains(type)))
                {
                    if (ChosenDrawableTypes.Add(type))
                        _chosenDrawableTypesChanged = true;
                }
                else
                {
                    if (ChosenDrawableTypes.Remove(type))
                        _chosenDrawableTypesChanged = true;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawFieldsScroll()
        {
            if (_eligibleFields == null || (_eligibleFields.Count == 0 && _selectedFields.Count == 0)) 
                return;
            
            _fieldsScrollPos = EditorGUILayout.BeginScrollView(_fieldsScrollPos);
            {
                DrawSelectedFields();
                DrawSelectionBox();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
        }

        private void DrawSelectedFields()
        {
            foreach (var fieldData in _selectedFields)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("x", GUILayout.Width(20)))
                    _elementsToRemove.Add(fieldData);

                EditorGUILayout.LabelField(fieldData.Field.Name, GUILayout.Width(100));
                DrawComparisonOperationSelector(fieldData);
                fieldData.Drawer.Value = fieldData.Drawer.Draw();
                EditorGUILayout.EndHorizontal();
            }

            foreach (var fieldData in _elementsToRemove)
            {
                _selectedFields.Remove(fieldData);
                _eligibleFields.Add(fieldData.Field);
            }

            _elementsToRemove.Clear();
        }

        private static void DrawComparisonOperationSelector(SelectedFieldData fieldData)
        {
            if (!ComparisonOperationsNamesByDrawerType.TryGetValue(fieldData.Drawer.GetType(), out var names))
            {
                var allowedOperations = fieldData.Drawer.AllowedOperations;
                names = allowedOperations.ToString().Split(',').Select(x => x.Trim()).ToArray();
                ComparisonOperationsNamesByDrawerType.Add(fieldData.Drawer.GetType(), names);
            }
            
            var oldIndex = Array.IndexOf(names, fieldData.ComparisonOperation.ToString());
            
            var index = EditorGUILayout.Popup(oldIndex, names, GUILayout.Width(50));
            
            // If the operation is not allowed for this drawer, we choose the first one that is allowed
            if (index == -1)
                index = 0;
            
            if (index == oldIndex) 
                return;
            
            fieldData.ComparisonOperation = EnumParser<BaseDrawer.ComparisonOperations>.Convert(names[index]);
        }

        private void DrawSelectionBox()
        {
            if (_eligibleFields.Count == 0)
                return;
            
            EditorGUILayout.BeginHorizontal();
            if (!_isSelectionInProcess && GUILayout.Button("+", GUILayout.Width(20))) 
                _isSelectionInProcess = true;

            if (_isSelectionInProcess)
            {
                if (_fieldsNames == null)
                {
                    _fieldsNames = new string[_eligibleFields.Count];
                    for (var i = 0; i < _eligibleFields.Count; i++)
                        _fieldsNames[i] = _eligibleFields[i].Name;
                }
                _selectedFieldIndex = EditorGUILayout.Popup(_selectedFieldIndex, _fieldsNames);
                
                if (GUILayout.Button("Add"))
                {
                    _selectedField = _eligibleFields[_selectedFieldIndex];
                    _eligibleFields.RemoveAt(_selectedFieldIndex);
                    var drawableType = _allDrawableTypes.First(x => x.type == _selectedField.FieldType);
                    _selectedFields.Add(new SelectedFieldData
                    {
                        Field = _selectedField,
                        Drawer = DrawerFactory.Create(drawableType.drawerType, null)
                    });
                    _isSelectionInProcess = false;
                    _fieldsNames = null;
                    _selectedFieldIndex = 0;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void UpdateFields()
        {
            var sameClass = _prevScript == _selectedScript;
            if (!sameClass || _eligibleDrawableTypes == null)
                Reset();

            if (sameClass && !_chosenDrawableTypesChanged)
                return;
            
            _chosenDrawableTypesChanged = false;
            _prevScript = _selectedScript;
            
            _eligibleFields = Finder.GetDrawableFields(_selectedScript.GetClass(), ChosenDrawableTypes);
            _eligibleFields.RemoveAll(x =>
                _selectedFields.FirstOrDefault(selected => selected.Field.Name == x.Name) != null);
        }

        private void Reset()
        {
            _eligibleFields = null;
            _selectedFields.Clear();
            ChosenDrawableTypes.Clear();
            _showChosenTypes = false;
            _isSelectionInProcess = false;
            _selectedField = null;
            _selectedFieldIndex = 0;
            _fieldsNames = null;
            _foundPrefabs = null;
            _fieldsScrollPos = Vector2.zero;
            UpdateAllDrawableTypes();
            UpdateEligibleTypes();
        }

        private void UpdateEligibleTypes()
        {
            if (_selectedScript == null)
                return;
            
            _allComponentFields = Finder.GetDrawableFields(_selectedScript.GetClass(), _allDrawableTypes.Select(x => x.type).ToHashSet());
            
            // Among all drawable types we choose only those which are present in the component
            _eligibleDrawableTypes = _allDrawableTypes
                .Where(x => _allComponentFields.FirstOrDefault(fieldInfo => x.type == fieldInfo.FieldType) != null)
                .ToHashSet();

            _eligibleFields = Finder.GetDrawableFields(
                _selectedScript.GetClass(),
                _eligibleDrawableTypes.Select(x => x.type).ToHashSet());
        }
    }

    public class SelectedFieldData
    {
        public FieldInfo Field;
        public BaseDrawer.ComparisonOperations ComparisonOperation;
        public BaseDrawer Drawer;
    }
}