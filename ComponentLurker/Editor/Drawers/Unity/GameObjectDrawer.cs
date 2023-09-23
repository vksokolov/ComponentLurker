using System;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class GameObjectDrawer : UnityObjectDrawer<GameObject>
    {
        public GameObjectDrawer(object value, GUILayoutOption[] options) : base(value, options)
        {
        }

        public GameObjectDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options)
        {
        }
    }
}