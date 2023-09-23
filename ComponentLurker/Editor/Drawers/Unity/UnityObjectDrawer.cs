using System;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class UnityObjectDrawer<T> : BaseDrawer<T> where T : UnityEngine.Object
    {
        public override ComparisonOperations AllowedOperations => 
            ComparisonOperations.Equal | 
            ComparisonOperations.NotEqual;
        
        private static readonly Func<T, GUILayoutOption[], T> DrawFunc = (value, options) =>
        {
            var sprite = EditorGUILayout.ObjectField(value, typeof(T), false, options) as T;
            return sprite;
        };
        
        private static readonly Func<string, T, GUILayoutOption[], T> DrawFuncWithLabel = (label, value, options) =>
        {
            var sprite = EditorGUILayout.ObjectField(label, value, typeof(T), false, options) as T;
            return sprite;
        };
        
        public UnityObjectDrawer(object value, GUILayoutOption[] options) : base(value, options, DrawFunc)
        {
        }

        public UnityObjectDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options, DrawFuncWithLabel)
        {
        }

        protected override bool IsEqual(object value) => 
            (UnityEngine.Object)value == (UnityEngine.Object)Value;

        protected override bool IsNotEqual(object value) => 
            !(UnityEngine.Object)value == (UnityEngine.Object)Value;
    }
}