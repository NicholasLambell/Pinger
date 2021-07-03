using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pinger.Annotations;

namespace Pinger {
    public class BindableBase : INotifyPropertyChanged {
        private Dictionary<string, Action<object>> PropertyValueChangedBindings { get; }

        protected BindableBase() {
            PropertyValueChangedBindings = new Dictionary<string, Action<object>>();
        }

        public void PropertyValueChanged(string propertyName, Action<object> action) {
            if (!PropertyValueChangedBindings.ContainsKey(propertyName)) {
                PropertyValueChangedBindings.Add(propertyName, null);
            }

            Action<object> bindingAction = PropertyValueChangedBindings[propertyName] += action;
            PropertyValueChangedBindings[propertyName] = bindingAction;
        }

        private void InvokePropertyValueChanged(object sender, string propertyName) {
            if (!PropertyValueChangedBindings.ContainsKey(propertyName)) {
                return;
            }

            object propValue = sender.GetType().GetProperty(propertyName)?.GetValue(sender);
            PropertyValueChangedBindings[propertyName].Invoke(propValue);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null) {
            if (Equals(storage, value)) {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            InvokePropertyValueChanged(this, propertyName);
        }
    }
}