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
            _foundPrefabsScrollPos = EditorGUILayout.BeginScrollView(_foundPrefabsScrollPos,
                GUILayout.Width(position.width),
                GUILayout.Height(position.height));
            foreach (var (prefab, component) in _foundPrefabs)
            {
                var width = EditorGUIUtility.currentViewWidth;
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.ObjectField(prefab, typeof(GameObject), false, GUILayout.Width(width * .75f));
                    if (GUILayout.Button("Open", GUILayout.Width(width * .2f)))
                        OpenPrefabAndSelectComponent(component);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        public static void OpenPrefabAndSelectComponent(Component component)
        {
            string prefabPath = AssetDatabase.GetAssetPath(component.gameObject);
            if (!string.IsNullOrEmpty(prefabPath))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));

                EditorApplication.delayCall += () =>
                {
                    string componentPathWithIndices = ComputePathWithIndices(component);
                    GameObject openedPrefabRoot = Selection.activeGameObject;

                    if (openedPrefabRoot)
                    {
                        Transform targetTransform =
                            ResolvePathWithIndices(openedPrefabRoot.transform, componentPathWithIndices);
                        if (targetTransform)
                        {
                            Component targetComponent = targetTransform.GetComponent(component.GetType());
                            if (targetComponent)
                                Selection.activeGameObject = targetComponent.gameObject;
                        }
                    }
                };
            }
        }

        private static string ComputePathWithIndices(Component component)
        {
            Transform currentTransform = component.transform;
            string path = currentTransform.name + "[" + currentTransform.GetSiblingIndex() + "]";

            while (currentTransform.parent != null && currentTransform.parent.parent != null)
            {
                currentTransform = currentTransform.parent;
                path = currentTransform.name + "[" + currentTransform.GetSiblingIndex() + "]" + "/" + path;
            }

            return path;
        }

        private static Transform ResolvePathWithIndices(Transform root, string pathWithIndices)
        {
            string[] parts = pathWithIndices.Split('/');
            if (parts.Length <= 1) 
                return root;
            
            Transform current = root;

            foreach (var part in parts)
            {
                int idxStart = part.IndexOf('[');
                int idxEnd = part.IndexOf(']');

                if (idxStart < 0 || idxEnd < 0) return null;

                string name = part.Substring(0, idxStart);
                int index = int.Parse(part.Substring(idxStart + 1, idxEnd - idxStart - 1));

                current = current.GetChild(index);
                if (!current || current.name != name) return null;
            }

            return current;
        }
    }
}