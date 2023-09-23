using System;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class StringDrawer : BaseDrawer<string>
    {
        public override ComparisonOperations AllowedOperations => 
            ComparisonOperations.Equal | 
            ComparisonOperations.NotEqual |
            ComparisonOperations.Contains |
            ComparisonOperations.Greater |
            ComparisonOperations.Less |
            ComparisonOperations.GreaterOrEqual |
            ComparisonOperations.LessOrEqual;
        
        public StringDrawer(object value, GUILayoutOption[] options) : base(value, options, EditorGUILayout.TextField)
        {
        }

        public StringDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options, EditorGUILayout.TextField)
        {
        }

        protected override bool IsGreater(object value)
        {
            return base.IsGreater(value);
        }
    }
}