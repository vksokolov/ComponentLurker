using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class IntDrawer : BaseDrawer<int>
    {
        public override ComparisonOperations AllowedOperations => 
            ComparisonOperations.Equal | 
            ComparisonOperations.NotEqual | 
            ComparisonOperations.Greater | 
            ComparisonOperations.Less | 
            ComparisonOperations.GreaterOrEqual | 
            ComparisonOperations.LessOrEqual;
        
        public IntDrawer(object value, GUILayoutOption[] options) : base(value, options, EditorGUILayout.IntField)
        {
        }
        
        public IntDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options, EditorGUILayout.IntField)
        {
        }
    }
}