using System;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class SpriteDrawer : BaseDrawer<Sprite>
    {
        public override ComparisonOperations AllowedOperations => 
            ComparisonOperations.Equal | 
            ComparisonOperations.NotEqual;
        
        private static readonly Func<Sprite, GUILayoutOption[], Sprite> DrawFunc = (value, options) =>
        {
            var sprite = EditorGUILayout.ObjectField(value, typeof(Sprite), false, options) as Sprite;
            return sprite;
        };
        
        private static readonly Func<string, Sprite, GUILayoutOption[], Sprite> DrawFuncWithLabel = (label, value, options) =>
        {
            var sprite = EditorGUILayout.ObjectField(label, value, typeof(Sprite), false, options) as Sprite;
            return sprite;
        };
        
        public SpriteDrawer(object value, GUILayoutOption[] options) : base(value, options, DrawFunc)
        {
        }

        public SpriteDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options, DrawFuncWithLabel)
        {
        }

        protected override bool IsEqual(object value) => 
            (UnityEngine.Object)value == (UnityEngine.Object)Value;

        protected override bool IsNotEqual(object value) => 
            !(UnityEngine.Object)value == (UnityEngine.Object)Value;
    }
}