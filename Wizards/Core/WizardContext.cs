using System;
using System.Collections.Generic;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Wizards.Core
{
    public class WizardContext
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public ProbeContext ProbeContext { get; }

        public Dictionary<string, object> Data => _data;

        public WizardContext(ProbeContext probeContext)
        {
            ProbeContext = probeContext ?? throw new ArgumentNullException(nameof(probeContext));
        }

        public void SetData(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            _data[key] = value;
        }

        public T GetData<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_data.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Cannot convert value for key '{key}' to type {typeof(T).Name}", ex);
                }
            }
            return default;
        }

        public bool TryGetData<T>(string key, out T value)
        {
            value = default;
            if (string.IsNullOrEmpty(key))
                return false;

            if (_data.TryGetValue(key, out var obj))
            {
                if (obj is T typedValue)
                {
                    value = typedValue;
                    return true;
                }
                try
                {
                    value = (T)Convert.ChangeType(obj, typeof(T));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool HasData(string key)
        {
            return !string.IsNullOrEmpty(key) && _data.ContainsKey(key);
        }

        public void RemoveData(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _data.Remove(key);
            }
        }

        public void ClearData()
        {
            _data.Clear();
        }
    }
}
