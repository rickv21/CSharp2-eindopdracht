using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject.Models
{
    public class CheckersModel
    {
        private Dictionary<string, object> data;

        public CheckersModel()
        {
            data = new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            if(!data.ContainsKey(key))
            {
                data.Add(key, value);
            }
            else
            {
                data[key] = value;
            }
        }

        public void Set(string key, object value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        }

        public void Remove(string key)
        {
            data.Remove(key);
        }

        public void RemoveAll()
        {
            data.Clear();
        }

        public T Get<T>(string key)
        {
            if (data.TryGetValue(key, out object value))
            {
                if (value is T typedValue)
                    return typedValue;
                else
                    throw new InvalidCastException($"Value for key '{key}' is not of type {typeof(T)}");
            }
            else
            {
                return default; // Default value if key doesn't exist
            }
        }
    }
}
