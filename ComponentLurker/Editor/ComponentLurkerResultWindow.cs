using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker
{
    public class ComponentLurkerResultWindow : EditorWindow
    {
        private List<(GameObject prefab, Component component)> _foundPrefabs;
        private Vector2 _foundPrefabsScrollPos;
        
        public static void ShowWindow(List<(GameObject prefab, Component component)> result)
        {
            var window = GetWindow<ComponentLurkerResultWindow>("Diff Result");
            window._foundPrefabs = result;
            window.Show();
        }

        private void OnGUI()
        {
            if (_foundPrefabs == null || _foundPrefabs.Count == 0) 
                return;
            
            EditorGUILayout.LabelField("Found prefabs");
            _foundPrefabsScrollPos = EditorGUILayout.BeginScrollView(_foundPrefabsScrollPos, GUILayout.Width(position.width),
                GUILayout.Height(position.height));
            foreach (var (prefab, component) in _foundPrefabs)
            {
                var width = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.ObjectField(prefab, typeof(GameObject), false, GUILayout.Width(width * .2f));
                    EditorGUILayout.ObjectField(component, typeof(Component), false, GUILayout.Width(width * .2f));
                    EditorGUILayout.LabelField(GetFullPath(component), GUILayout.Width(width * .55f));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        
        private string GetFullPath(Component component)
        {
            var path = component.name;
            var parent = component.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}