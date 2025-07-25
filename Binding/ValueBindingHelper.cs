﻿using System;
using Colossal.UI.Binding;

namespace StationPylon.Binding
{
    
    public class ValueBindingHelper<T>
    {
        private readonly Action<T> _updateCallBack;

        public ValueBinding<T> Binding { get; }

        public T Value { get => Binding.value; set => Binding.Update(value); }

        public ValueBindingHelper(ValueBinding<T> binding, Action<T> updateCallBack = null)
        {
            Binding = binding;
            _updateCallBack = updateCallBack;
        }

        public void UpdateCallback(T value)
        {
            Binding.Update(value);
            _updateCallBack?.Invoke(value);
        }

        public static implicit operator T(ValueBindingHelper<T> helper)
        {
            return helper.Binding.value;
        }
    }
}
