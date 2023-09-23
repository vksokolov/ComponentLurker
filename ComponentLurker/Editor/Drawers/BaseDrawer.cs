using System;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentLurker.Drawers
{
    public abstract class BaseDrawer<T> : BaseDrawer
    {
        private readonly Func<T, GUILayoutOption[], T> _drawFunc;
        private readonly Func<string, T, GUILayoutOption[], T> _drawFuncWithLabel;
        private readonly GUILayoutOption[] _options;
        private readonly string _label;
        
        protected BaseDrawer(object value, GUILayoutOption[] options, Func<T, GUILayoutOption[], T> drawFunc) : base(value)
        {
            _drawFunc = drawFunc;
            _options = options;
        }
        
        protected BaseDrawer(string label, object value, GUILayoutOption[] options, Func<string, T, GUILayoutOption[], T> drawFuncWithLabel) : base(value)
        {
            _drawFuncWithLabel = drawFuncWithLabel;
            _options = options;
            _label = label;
        }
        
        public override object Draw() => 
            DrawInternal();

        private T DrawInternal()
        {
            T value;
            if (Value is T val)
                value = val;
            else
                value = default;
            
            if (_drawFunc != null)
                return _drawFunc.Invoke(value, _options);

            if (_drawFuncWithLabel != null)
                return _drawFuncWithLabel.Invoke(_label, value, _options);
            
            throw new Exception($"Drawing function for type {typeof(T)} is not implemented");
        }
    }
    
    public abstract class BaseDrawer : IComparisonOperationsKeeper
    {
        [Flags]
        public enum ComparisonOperations
        {
            Equal = 1 << 0,
            NotEqual = 1 << 1,
            Greater = 1 << 2,
            Less = 1 << 3,
            GreaterOrEqual = 1 << 4,
            LessOrEqual = 1 << 5,
            Contains = 1 << 6,
        }

        public static Dictionary<ComparisonOperations, string> ComparisonOperationsNames = new()
        {
            {ComparisonOperations.Equal, "=="}, 
            {ComparisonOperations.NotEqual, "!="}, 
            {ComparisonOperations.Greater, ">"}, 
            {ComparisonOperations.Less, "<"}, 
            {ComparisonOperations.GreaterOrEqual, ">="}, 
            {ComparisonOperations.LessOrEqual, "<="},
            {ComparisonOperations.Contains, "Has"},
        };

        public object Value;
        
        public BaseDrawer(object value)
        {
            Value = value;
        }
        
        public abstract object Draw();
        public abstract ComparisonOperations AllowedOperations { get; }
        
        private bool IsComparisonOperationAllowed(ComparisonOperations operation) => (AllowedOperations & operation) == operation && operation != 0;
        
        public bool Compare(ComparisonOperations operation, object value)
        {
            if (!IsComparisonOperationAllowed(operation))
                throw new Exception($"Operation {operation} is not allowed for type {Value.GetType()}");

            return operation switch
            {
                ComparisonOperations.Equal => IsEqual(value),
                ComparisonOperations.NotEqual => IsNotEqual(value),
                ComparisonOperations.Greater => IsGreater(value),
                ComparisonOperations.Less => IsLess(value),
                ComparisonOperations.GreaterOrEqual => IsGreaterOrEqual(value),
                ComparisonOperations.LessOrEqual => IsLessOrEqual(value),
                ComparisonOperations.Contains => Contains(value),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }
        
        protected virtual bool IsEqual(object value)
        {
            if (Value == null)
                return value == null;
            
            return value.Equals(Value);
        }

        protected virtual bool IsNotEqual(object value) => 
            !Value.Equals(value);
        
        protected virtual bool IsGreater(object value)
        {
            if (Value == null) return false;
            if (value == null) return true;
            
            return Convert.ToSingle(value) > Convert.ToSingle(Value);
        }

        protected virtual bool IsLess(object value)
        {
            if (Value == null) return false;
            if (value == null) return true;
            
            return Convert.ToSingle(value) < Convert.ToSingle(Value);
        }

        private bool IsGreaterOrEqual(object value) => 
            Equals(value) || IsGreater(value);

        private bool IsLessOrEqual(object value) => 
            Equals(value) || IsLess(value);

        protected virtual bool Contains(object value)
        {
            if (Value == null)
            {
                return value == null;
            }
            if (value == null) return true;
            
            return ((string)value).Contains((string)Value);
        }
    }
    
    public interface IComparisonOperationsKeeper
    {
        BaseDrawer.ComparisonOperations AllowedOperations { get; }
    }
}