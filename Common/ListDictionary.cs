using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class ListDictionary<S, T>
    {
        Dictionary<S, List<T>> dictionary = new Dictionary<S, List<T>>();

        public List<T> this[S key]
        {
            get
            {
                List<T> ret;
                if (!dictionary.TryGetValue(key, out ret))
                {
                    ret = new List<T>();
                    dictionary.Add(key, ret);
                }
                return ret;
            }
        }

        public Dictionary<S, List<T>>.Enumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }

    public class ListDictionaryUsingEqualityTest<S, T>
    {
        public delegate bool EqualityTest(S s1, S s2);
        Dictionary<S, List<T>> dictionary = new Dictionary<S, List<T>>();
        EqualityTest test;

        public ListDictionaryUsingEqualityTest(EqualityTest test)
        {
            this.test = test;
        }

        public List<T> this[S key]
        {
            get
            {
                List<T> ret = null;
                Dictionary<S, List<T>>.Enumerator e = dictionary.GetEnumerator();
                while (ret == null && e.MoveNext())
                {
                    if (test(e.Current.Key, key))
                        ret = e.Current.Value;
                }
                if (ret == null)
                {
                    ret = new List<T>();
                    dictionary.Add(key, ret);
                }
                return ret;
            }
        }

        public Dictionary<S, List<T>>.Enumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Clear()
        {
            dictionary.Clear();
        }
    }

}
