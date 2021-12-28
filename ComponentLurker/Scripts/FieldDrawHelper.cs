using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class FieldDrawHelper
{
    public static Dictionary<Type, Func<string, object, object>> GetDefaultDrawFunctions()
    {
        Dictionary<Type, Func<string, object, object>> result = new Dictionary<Type, Func<string, object, object>>()
        {
            {typeof(string), DrawText}
        };

        return result;
    }

    private static object DrawText(string label, object value)
    {
        return EditorGUILayout.TextField(label, (string)value);
    }
}