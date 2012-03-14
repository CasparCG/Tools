using System;
using System.Collections;
using System.Collections.Generic;

namespace Bespoke.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubArray<T> : IEnumerable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get;

            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return mOriginal[mStart + index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public SubArray(T[] original, int start, int length)
        {
            mOriginal = original;
            mStart = start;
            Length = length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return mOriginal[mStart + i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[Length];
            Array.Copy(mOriginal, mStart, result, 0, Length);
            return result;
        }

        private T[] mOriginal;
        private int mStart;
    }
}
