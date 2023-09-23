using System;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class GameObjectDrawer : BaseDrawer<GameObject>
    {
        public override ComparisonOperations AllowedOperations => 
            ComparisonOperations.Equal | 
            ComparisonOperations.NotEqual;
        
        private static readonly Func<GameObject, GUILayoutOption[], GameObject> DrawFunc = (value, options) =>
        {
            var sprite = EditorGUILayout.ObjectField(value, typeof(GameObject), false, options) as GameObject;
            return sprite;
        };
        
        private static readonly Func<string, GameObject, GUILayoutOption[], GameObject> DrawFuncWithLabel = (label, value, options) =>
        {
            var sprite = EditorGUILayout.ObjectField(label, value, typeof(GameObject), false, options) as GameObject;
            return sprite;
        };
        
        public GameObjectDrawer(object value, GUILayoutOption[] options) : base(value, options, DrawFunc)
        {
        }

        public GameObjectDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options, DrawFuncWithLabel)
        {
        }

        protected override bool IsEqual(object value) => 
            (UnityEngine.Object)value == (UnityEngine.Object)Value;

        protected override bool IsNotEqual(object value) => 
            !(UnityEngine.Object)value == (UnityEngine.Object)Value;
    }
}