using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class FloatDrawer : BaseDrawer<float>
    {
        public override ComparisonOperations AllowedOperations => 
            ComparisonOperations.Equal | 
            ComparisonOperations.NotEqual | 
            ComparisonOperations.Greater | 
            ComparisonOperations.Less | 
            ComparisonOperations.GreaterOrEqual | 
            ComparisonOperations.LessOrEqual;

        public FloatDrawer(object value, GUILayoutOption[] options) : base(value, options, EditorGUILayout.FloatField)
        {
        }

        public FloatDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options, EditorGUILayout.FloatField)
        {
        }
    }
}