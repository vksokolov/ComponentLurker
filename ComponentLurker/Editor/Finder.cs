using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ComponentLurker.Drawers;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker
{
    public static class Finder
    {
        public static HashSet<(Type drawerType, Type type)> FindAllDrawableTypes()
        {
            var baseType = typeof(BaseDrawer);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            var result = new List<(Type drawerType, Type type)>();
            foreach (var type in types)
            {
                if (type.IsAbstract || !type.IsSubclassOf(baseType) || type.IsGenericType) 
                    continue;

                if (type.BaseType == null || type.BaseType.BaseType != baseType)
                {
                    Debug.LogWarning($"Drawer {type} is not inherited from {baseType} directly");
                    continue;
                }
                
                result.Add((type, type.BaseType.GetGenericArguments()[0]));
            }

            return result.ToHashSet();
        }
        
        public static List<FieldInfo> GetDrawableFields(Type type, HashSet<Type> allowedTypes) =>
            type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => allowedTypes.Contains(field.FieldType))
                .ToList();

        private static IEnumerable<GameObject> FindAllPrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");
            var result = new List<GameObject>(guids.Length);
            result.AddRange(
                guids
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<GameObject>));

            return result;
        }

        public static List<(GameObject prefab, Component component)> FindAllPrefabs(Type getClass, List<SelectedFieldData> selectedFields)
        {
            var allPrefabs = FindAllPrefabs();
            var prefabsWithComponent = allPrefabs
                .SelectMany(prefab =>
                {
                    var components = prefab.GetComponentsInChildren(getClass, true);
                    return components.Select(component => (prefab, component));
                })
                .Where(entry => entry.Item2 != null)
                .ToList();
            
            var filteredPrefabs = prefabsWithComponent
                .Where(entry => 
                    selectedFields.All(selectedField =>
                    {
                        var operation = selectedField.ComparisonOperation;
                        var value = selectedField.Field.GetValue(entry.Item2);
                        return selectedField.Drawer.Compare(operation, value);
                    }))
                .ToList();
            
            return filteredPrefabs;
        }
    }
}