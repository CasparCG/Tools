using System;
using System.Collections.Generic;

namespace Bespoke.Common
{
    public static class ObjectRegistry
    {
        public static object[] Objects
        {
            get
            {
                object[] objects = new object[sObjects.Values.Count];
                sObjects.Values.CopyTo(objects, 0);

                return objects;
            }
        }

        static ObjectRegistry()
        {
            sObjects = new Dictionary<string, object>();
        }

        public static T GetRegisteredObject<T>(string name)
        {
            object value;

            if (sObjects.ContainsKey(name))
            {
                value = sObjects[name];
                if ((value != null) && (value is T == false))
                {
                    throw new InvalidCastException("Variable [" + name.ToString() + "] does not store type: " + typeof(T).ToString());
                }
            }
            else
            {
                value = null;
            }

            return (T)value;
        }

        public static T GetRegisteredObject<T>()
        {
            Type type = typeof(T);
            return GetRegisteredObject<T>(type.Name);
        }

        public static void RegisterObject(object value)
        {
            string typeName = value.GetType().Name;
            RegisterObject(typeName, value);
        }

        public static void RegisterObject(string name, object value)
        {
            if (sObjects.ContainsKey(name))
            {
                sObjects[name] = value;
            }
            else
            {
                sObjects.Add(name, value);
            }
        }

        public static void UnregisterObject(string name)
        {
            sObjects.Remove(name);
        }

        private static Dictionary<string, object> sObjects;
    }
}
