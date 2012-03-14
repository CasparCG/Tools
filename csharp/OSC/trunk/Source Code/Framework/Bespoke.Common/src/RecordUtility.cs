using System;
using System.Reflection;
using System.Collections;

namespace Bespoke.Common.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class RecordUtility
    {
        /// <summary>
        /// Tests for equality between record objects using the object's properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns></returns>
        public static bool Equals<T>(T lhs, T rhs)
        {
            if (lhs == null || rhs == null)
            {
                return object.ReferenceEquals(lhs, rhs);
            }
            else
            {
                Type type = lhs.GetType();
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    EqualityExclusionAttribute[] equalityExclusionAttributes = (EqualityExclusionAttribute[])property.GetCustomAttributes(typeof(EqualityExclusionAttribute), true);
                    if (equalityExclusionAttributes.Length == 0)
                    {
                        object lhsValue = property.GetValue(lhs, null);
                        object rhsValue = property.GetValue(rhs, null);

                        if (lhsValue is Array)
                        {
                            Array lhsValueArray = (Array)lhsValue;
                            Array rhsValueArray = (Array)rhsValue;

                            if (lhsValueArray.Length != rhsValueArray.Length)
                            {
                                return false;
                            }
                            else
                            {
                                IEnumerator lhsEnumerator = lhsValueArray.GetEnumerator();
                                IEnumerator rhsEnumerator = rhsValueArray.GetEnumerator();

                                while (lhsEnumerator.MoveNext() && rhsEnumerator.MoveNext())
                                {
                                    if (ValueEquals(lhsEnumerator.Current, rhsEnumerator.Current) == false)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (ValueEquals(lhsValue, rhsValue) == false)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        private static bool ValueEquals(object lhsValue, object rhsValue)
        {
            if (lhsValue == null || rhsValue == null)
            {
                return object.ReferenceEquals(lhsValue, rhsValue);
            }
            else
            {
                return lhsValue.Equals(rhsValue);
            }
        }
    }
}
