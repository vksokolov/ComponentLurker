﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ComponentLurkerWindow : EditorWindow
{
    private MonoScript _script;

    private SerializedObject _so;
    private Type _type;
    private FieldInfo[] _fields;
    private object[] _values;
    private bool[] _equalityPredicates;
    
    private List<(GameObject obj, Component[] components)> _foundPrefabs;
    
    [MenuItem ("Utils/ComponentLurker")]
    public static void  ShowWindow () {
        GetWindow(typeof(ComponentLurkerWindow));
    }
    
    void OnGUI ()
    {
        _script = (MonoScript)EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), true);

        if (GUILayout.Button("RefreshFields"))
        {
            _so = new SerializedObject(_script);
            _type = _script.GetClass();
            _fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            _values = new object[_fields.Length];
            _equalityPredicates = new bool[_fields.Length];
        }
        
        DrawFields(_fields, FieldDrawHelper.GetDefaultDrawFunctions());

        if (GUILayout.Button("Search"))
        {
            _foundPrefabs = PrefabUtils.GetPrefabsWithComponent(_script.GetClass(), "Assets");
            foreach (var tuple in _foundPrefabs)
            {
                foreach (var component in tuple.components)
                {
                    bool ok = true;
                    for(int i=0;i<_fields.Length;i++)
                    {
                        if (_values[i] == null)
                            continue;

                        object value = _fields[i].GetValue(component);
                        bool equality = value.Equals(_values[i]);
                        if (equality != _equalityPredicates[i])
                        {
                            ok = false;
                            break;
                        }
                    }
                    
                    if (ok)
                        Debug.Log($"{tuple.obj}", tuple.obj);
                }
            }
        }
    }

    void DrawFields(FieldInfo[] fields, Dictionary<Type, Func<string, object, object>> drawFunctions)
    {
        if (fields == null)
            return;
        
        for(int i=0;i<fields.Length;i++)
        {
            var field = fields[i];
            var type = field.FieldType;
            if (drawFunctions.ContainsKey(type))
            {
                EditorGUILayout.BeginHorizontal();
                _values[i] = drawFunctions[type](field.Name, _values[i]);
                if (GUILayout.Button(_equalityPredicates[i] ? "=" : "≠"))
                {
                    _equalityPredicates[i] = !_equalityPredicates[i];
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _values[i] = null;
            }
        }
    }
}
