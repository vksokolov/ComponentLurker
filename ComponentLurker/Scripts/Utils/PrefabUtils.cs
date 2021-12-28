using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class PrefabUtils
{
    public static List<(GameObject, Component[])> GetPrefabsWithComponent(Type type, string path)
    {
        List<(GameObject, Component[])> result = new List<(GameObject, Component[])>();
 
        var allFiles = AssetDatabase.FindAssets("t:prefab", new string[]{path});
        foreach (var objGuid in allFiles)
        {
            var objPath = AssetDatabase.GUIDToAssetPath(objGuid);
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(objPath);
            if (obj is GameObject go)
            {
                var components = go.GetComponentsInChildren(type); 
                if (components != null)
                {
                    result.Add((go, components));
                }
            }
        }
        return result;
    }
}