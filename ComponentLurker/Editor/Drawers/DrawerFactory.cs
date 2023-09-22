using System;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public static class DrawerFactory
    {
        public static BaseDrawer Create(Type type, GUILayoutOption[] options)
        {
            var constructor = type.GetConstructor(new[] {typeof(object), typeof(GUILayoutOption[])});
            if (constructor == null)
                throw new Exception($"Constructor for type {type} is not implemented");
            
            return (BaseDrawer)constructor.Invoke(new object[] {default, options});
        }
        
        public static BaseDrawer Create(Type type, string label, GUILayoutOption[] options)
        {
            var constructor = type.GetConstructor(new[] {typeof(string), typeof(object), typeof(GUILayoutOption[])});
            if (constructor == null)
                throw new Exception($"Constructor for type {type} is not implemented");
            
            return (BaseDrawer)constructor.Invoke(new object[] {label, default, options});
        }
    }
}