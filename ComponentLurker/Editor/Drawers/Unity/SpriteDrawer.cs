using System;
using UnityEditor;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public class SpriteDrawer : UnityObjectDrawer<Sprite>
    {
        public SpriteDrawer(object value, GUILayoutOption[] options) : base(value, options)
        {
        }

        public SpriteDrawer(string label, object value, GUILayoutOption[] options) : base(label, value, options)
        {
        }
    }
}