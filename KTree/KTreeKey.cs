using KTreeDataTypes;
using System;
using System.Collections.Generic;

namespace KTree
{
    [Serializable]
    public class KTreeKey<T> : IComparable, ICloneable where T : KTreeDataType<T>
    {
        internal KTree<T> Child { get; set; }

        public T Key { get; set; }

        public KTreeKey(T key)
        {
            Key = key;
        }

        public KTreeKey(T key, KTree<T> child)
        {
            Key = key;
            Child = child;
        }

        public KTreeKey(KTree<T> node, T key, List<KTreeKey<T>> childKeys)
        {
            Key = key;
            Child = new(node, childKeys);
        }

        public object Clone()
        {
            return new KTreeKey<T>(Key, Child);
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                return obj is KTreeKey<T> otherKTreeKey ? Key.CompareTo(otherKTreeKey.Key) : throw new ArgumentException("Object is not a KTreeKey");
            }
            else
            {
                return 1;
            }
        }
    }
}